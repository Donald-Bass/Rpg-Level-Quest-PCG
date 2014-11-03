//holds the state of the entire world.

using PCG_GUI.WorldModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PCG_GUI.Facts
{
    public class World
    {
        private List<Fact> roomFacts; //facts necessary to build the level
        private List<Fact> edgeFacts; //facts necessary to build the level

        const int LEVEL_X_DIMENSION = 10;
        const int LEVEL_Y_DIMENSION = 10;

        public int numLevels { get; private set; } //how many different levels are there
        int[,] levelDimensions; //dimensions of each level. First index is level number second is x/y (0 is x dimension 1 is y dimension)
        private List<Level> allLevels; //all levels

        public World()
        {
            numLevels = 0;
            roomFacts = new List<Fact>();
            edgeFacts = new List<Fact>();
            allLevels = new List<Level>();

        }

        public void parseClingoFile(System.IO.StreamReader file)
        {
            Fact curFact;
            
            file.ReadLine(); //the first fours line are information we can skip
            file.ReadLine();
            file.ReadLine();
            file.ReadLine(); 

            String[] StringFacts = file.ReadLine().Trim().Split(' '); //the second line has the model so split all the facts into seperate strings

            //parse all facts
            //allFacts = new Fact[StringFacts.Length];
            
            numLevels = 1;
            setUpLevels();


            for (int i = 0; i < StringFacts.Length; i++ )
            {
                curFact = new Fact(StringFacts[i]);
                sortFact(curFact);
            }

            buildLevels();

            //return allFacts;
        }

        public void writeWorldFile(System.IO.StreamWriter file)
        {
            file.WriteLine(""); //the clingo output has 4 unecessary line at the start so add blank lines to match the formula
            file.WriteLine("");
            file.WriteLine(""); 
            file.WriteLine("");


            Fact totalLevels = new Fact("totalLevels", new String[] { "1" });
            file.Write(totalLevels.getStringRepresentation());

            /*
            //placeholder till npcs are actually stored in memory
            foreach (Fact f in npcFacts)
            {
                file.Write(f.getStringRepresentation());
            }

            //placeholder till quests are actually stored in memory
            foreach (Fact f in questFacts)
            {
                file.Write(f.getStringRepresentation());
            }*/

            for(int i = 0; i < numLevels; i++)
            {
                allLevels[i].write(file, i);
            }
        }

        //numberOfLevels - number of levels to generate. If -1 generate as many as clingo feels is necessary
        public void writeClingoInputFile(System.IO.StreamWriter file, int numberOfLevels)
        {
            if (numberOfLevels != -1)
            {
                Fact totalLevels = new Fact("totalLevels", new String[] { numberOfLevels.ToString() });
                file.Write(totalLevels.getStringRepresentation(true));
            }

            for (int i = 0; i < numLevels; i++)
            {
                allLevels[i].write(file, i, true);
            }
        }

        //does the inital processing of facts. The order is not all that reliable so store everything that has any prequristies to be processed
        private void sortFact(Fact fact)
        {
            switch (fact.getPredicate())
            {
                case "room":
                    roomFacts.Add(fact);
                    break;
                case "edge":
                    edgeFacts.Add(fact);
                    break;
            }
        }

        //determine the dimensions of each level and create the level objects
        //Must have already parsed the clingo file and sorted all the facts from it
        private void setUpLevels()
        {
            for(int i = 0; i < numLevels; i++)
            {
                Level newLevel = new Level(LEVEL_X_DIMENSION, LEVEL_Y_DIMENSION);
                allLevels.Add(newLevel);
            }
        }
    
        //go through the level facts to build all the levels
        private void buildLevels()
        {
            //we need to specify the start of the level last or that can be overwritten so these variables hold that position until the end

            //Place the rooms on the temporaly vizulized level
            foreach (Fact f in roomFacts)
            {
                allLevels[0].setTileType(f.getNumericValue(1), f.getNumericValue(2), TileType.floor);
            }

            buildRoomInfo();

            //now place corridors on the temporary vizual.
            foreach (Fact f in edgeFacts)
            {
                int room1 = f.getNumericValue(0);
                int room2 = f.getNumericValue(1);

                int X1 = allLevels[0].allRooms[room1].X;
                int Y1 = allLevels[0].allRooms[room1].Y;
                int X2 = allLevels[0].allRooms[room2].X;
                int Y2 = allLevels[0].allRooms[room2].Y;

                System.Console.WriteLine("room1 " + room1 + " X1 " + X1 + " Y1 " + Y1);
                System.Console.WriteLine("room2 " + room2 + " X2 " + X2 + " Y2 " + Y2);

                //swap values so X1 is always less then X2 and same with Y values
                if (X1 > X2)
                {
                    int temp = X2;
                    X2 = X1;
                    X1 = temp;
                }

                if (Y1 > X2)
                {
                    int temp = Y2;
                    Y2 = Y1;
                    Y1 = temp;
                }

                //if the corridor runs horizontally
                //as a hack these corridors will be red
                if (Y1 == Y2)
                {
                    for (int i = X1 + 1; i < X2; i++)
                    {
                        allLevels[0].setTileType(i, Y1, TileType.arena);
                    }
                }

                //if the corridor runs vertically
                //as a hack these corridors will be yellow
                else
                {
                    for (int i = Y1 + 1; i < Y2; i++)
                    {
                        allLevels[0].setTileType(X1, i, TileType.treasureRoom);
                    }
                }

            }

            foreach(Level l in allLevels)
            {
                l.finalizeLevel();
            }
        }

        //function that goes through all the rectangle and room ID facts to determine what room belongs where
        public void buildRoomInfo()
        {
            int i;

            //NEED TO FIX THIS TO DETERMINE PROPER NUMBER OF ROOMS NEEDED
            Room[] tempRooms = new Room[100]; //create an array to store the rectangles temporaily until we can determine their room numbers

            for (i = 0; i < 100; i++ )
            {
                tempRooms[i] = null;
            }


            foreach (Fact f in roomFacts) //go through all rooms and store information
            {
                i = f.getNumericValue(0);

                tempRooms[i] = new Room();
                tempRooms[i].roomNumber = i;
                tempRooms[i].X = f.getNumericValue(1);
                tempRooms[i].Y = f.getNumericValue(2);

                i++;
            }

            //add all rooms created to the level
            i = 1;  //start at 1 as there is no room 0

            while(tempRooms[i] != null && i < 100)
            {
                allLevels[0].addRoom(tempRooms[i]);
                i++;
            }

        }

        //setters and getters
        public Level getLevel(int levelNum)
        {
            return allLevels[levelNum];
        }

        //add an additional level
        public void addLevel(int x, int y)
        {
            Level newLevel = new Level(x, y);
            allLevels.Add(newLevel);
            numLevels++;
        }
    }


 
    
}
