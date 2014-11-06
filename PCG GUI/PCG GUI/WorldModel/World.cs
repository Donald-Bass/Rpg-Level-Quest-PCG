/* The World class is another class whose purpose is confusing do to a change in direction in the research. It was originally going to be possible for there to be multiple levels generated
 * by the PCG and the world class would have held them all and handled any communications between the GUI and a specific level. There is only a single level being generated now
 * so the World class is mostly uncessary complication.
 */

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

        private Level pcgLevel; //all levels

        public World()
        {
            roomFacts = new List<Fact>();
            edgeFacts = new List<Fact>();
            pcgLevel = new Level(LEVEL_X_DIMENSION, LEVEL_Y_DIMENSION);
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

            pcgLevel.write(file, 1);
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

        //go through the level facts to build all the levels
        private void buildLevels()
        {
            //we need to specify the start of the level last or that can be overwritten so these variables hold that position until the end

            //Place the rooms on the temporaly vizulized level
            foreach (Fact f in roomFacts)
            {
                pcgLevel.setTileType(f.getNumericValue(1), f.getNumericValue(2), TileType.floor);
            }

            buildRoomInfo();

            //now place corridors on the temporary vizual.
            foreach (Fact f in edgeFacts)
            {
                int room1 = f.getNumericValue(0);
                int room2 = f.getNumericValue(1);

                int X1 = pcgLevel.allRooms[room1].X;
                int Y1 = pcgLevel.allRooms[room1].Y;
                int X2 = pcgLevel.allRooms[room2].X;
                int Y2 = pcgLevel.allRooms[room2].Y;

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
                        pcgLevel.setTileType(i, Y1, TileType.arena);
                    }
                }

                //if the corridor runs vertically
                //as a hack these corridors will be yellow
                else
                {
                    for (int i = Y1 + 1; i < Y2; i++)
                    {
                        pcgLevel.setTileType(X1, i, TileType.treasureRoom);
                    }
                }

            }

            pcgLevel.finalizeLevel();
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
                pcgLevel.addRoom(tempRooms[i]);
                i++;
            }

        }

        //setters and getters
        public Level getLevel()
        {
            return pcgLevel;
        }

    }

    
}
