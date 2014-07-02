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
        private List<Fact> levelFacts; //facts necessary to build the level
        private List<Fact> levelDimensionFacts; //facts storying dimensions of levels
        private List<Fact> rectFacts; //facts storing rectangles
        private List<Fact> roomIDFacts; //facts storing room ID's
        private List<Fact> roomTypeFacts; //facts storing room types

        public int numLevels { get; private set; } //how many different levels are there
        int[,] levelDimensions; //dimensions of each level. First index is level number second is x/y (0 is x dimension 1 is y dimension)
        private List<Level> allLevels; //all levels

        public World()
        {
            numLevels = 0;
            rectFacts = new List<Fact>();
            roomIDFacts = new List<Fact>();
            levelFacts = new List<Fact>();
            levelDimensionFacts = new List<Fact>();
            roomTypeFacts = new List<Fact>(); 
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

            for (int i = 0; i < StringFacts.Length; i++ )
            {
                curFact = new Fact(StringFacts[i]);
                sortFact(curFact);
            }

            numLevels = 1;

            setUpLevels();

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
                /*case "totalLevels":
                    numLevels = fact.getNumericValue(0); //get the number of levels from the fact
                    break;*/

                //level dimension facts
                case "levelLengthX":
                    levelDimensionFacts.Add(fact);
                    break;
                case "levelLengthY":
                    levelDimensionFacts.Add(fact);
                    break;
                //level facts
                case "floor":
                    levelFacts.Add(fact);
                    break;
                case "wall":
                    levelFacts.Add(fact);
                    break;
                case "door":
                    levelFacts.Add(fact);
                    break;
                case "levelStart":
                    levelFacts.Add(fact);
                    break;
                case "rectangle":
                    rectFacts.Add(fact);
                    break;
                case "roomID":
                    roomIDFacts.Add(fact);
                    break;
                case "typeOfRoom":
                    roomTypeFacts.Add(fact);
                    break;              

                //npc facts
                /*case "npc":
                    npcFacts.Add(fact);
                    break;
                case "friendly":
                    npcFacts.Add(fact);
                    break;
                case "hostile":
                    npcFacts.Add(fact);
                    break;
                case "npcLocation":
                    npcFacts.Add(fact);
                    break;
                //quest facts
                case "stage":
                    questFacts.Add(fact);
                    break;
                case "typeOfObjective":
                    questFacts.Add(fact);
                    break;
                case "action":
                    questFacts.Add(fact);
                    break;
                case "playerAt":
                    questFacts.Add(fact);
                    break;
                case "stageAction":
                    questFacts.Add(fact);
                    break;
                case "quest":
                    questFacts.Add(fact);
                    break;
                case "questGiver":
                    questFacts.Add(fact);
                    break;
                case "questObjectiveType":
                    questFacts.Add(fact);
                    break;
                case "questTarget":
                    questFacts.Add(fact);
                    break;
                case "kill":
                    questFacts.Add(fact);
                    break;
                case "move":
                    questFacts.Add(fact);
                    break;
                case "totalStages":
                    questFacts.Add(fact);
                    break;
                case "getQuest":
                    questFacts.Add(fact);
                    break;
                //ignored facts
                case "step":
                    break;
                case "alive":
                    break;
                case "dead":
                    break;
                case "npcLevel":
                    break;*/
                default:
                    //System.Console.Error.WriteLine("Unknown predicate type " + fact.getPredicate());
                    break;
            }
        }

        //determine the dimensions of each level and create the level objects
        //Must have already parsed the clingo file and sorted all the facts from it
        private void setUpLevels()
        {
            levelDimensions = new int[numLevels, 2]; //create array to store level dimensions in

            //process all the level dimensions TO DO POSSIBLY CHECK FOR MISSING DIMESNIONS
            foreach(Fact f in levelDimensionFacts)
            {
                processDimensionFact(f);
            }

            for(int i = 0; i < numLevels; i++)
            {
                Level newLevel = new Level(levelDimensions[i, 0], levelDimensions[i, 1]);
                allLevels.Add(newLevel);
            }
        }

        //process a level dimension fact
        private void processDimensionFact(Fact f)
        {
            if(f.getPredicate() == "levelLengthX")
            {
                //the levelLenghtX fact has form(length,level)
                levelDimensions[0, 0] = f.getNumericValue(0);
            }

            else if (f.getPredicate() == "levelLengthY")
            {
                //the levelLenghtY fact has form(length,level)
                levelDimensions[0, 1] = f.getNumericValue(0);
            }

            else
            {
                System.Console.Error.WriteLine("Error predicate type " + f.getPredicate() + " should not be in levelDimensionFacts list");
            }
        }
    
        //go through the level facts to build all the levels
        private void buildLevels()
        {
            //we need to specify the start of the level last or that can be overwritten so these variables hold that position until the end
            int levelStartX = 0;
            int levelStartY = 0;

            //process all the level dimensions TO DO POSSIBLY CHECK FOR MISSING DIMESNIONS
            foreach (Fact f in levelFacts)
            {
                switch (f.getPredicate())
                {
                    case "floor": //floor format is (X,Y,L)
                        allLevels[0].setTileType(f.getNumericValue(0), f.getNumericValue(1), TileType.floor);
                        break;
                    case "door": //floor format is (X,Y,L)
                        allLevels[0].setTileType(f.getNumericValue(0), f.getNumericValue(1), TileType.door);
                        break;
                    case "wall": //floor format is (X,Y,L)
                        allLevels[0].setTileType(f.getNumericValue(0), f.getNumericValue(1), TileType.wall);
                        break;
                    default:
                        System.Console.Error.WriteLine("Error predicate type " + f.getPredicate() + " should not be in levelFact list");
                        break;
                }
            }

            determineRooms();

            foreach(Level l in allLevels)
            {
                l.finalizeLevel();
            }
        }

        //function that goes through all the rectangle and room ID facts to determine what room belongs where
        public void determineRooms()
        {
            //NEED TO FIX THIS TO DETERMINE PROPER NUMBER OF ROOMS NEEDED
            Room[] tempRooms = new Room[100]; //create an array to store the rectangles temporaily until we can determine their room numbers

            int i = 0; //where do we store the current rectangle
            
            foreach (Fact f in rectFacts) //go through all rectangles
            {
                tempRooms[i] = new Room();

                tempRooms[i].XUL = f.getNumericValue(0);
                tempRooms[i].YUL = f.getNumericValue(1);
                tempRooms[i].XBR = f.getNumericValue(2);
                tempRooms[i].YBR = f.getNumericValue(3);
                tempRooms[i].roomType = f.getValue(4);

                i++;
            }

            foreach (Fact f in roomIDFacts) //go through all room id's
            {
                i = 0;

                //go through all rooms until you find the room which upper left corner matches the corner of the roomID predicate
                while(i < 26 && (tempRooms[i].XUL != f.getNumericValue(0) || tempRooms[i].YUL != f.getNumericValue(1)))
                {
                    i++;
                }

                if(i != 26)
                {
                    tempRooms[i].roomNumber = f.getNumericValue(2);
                    allLevels[0].addRoom(tempRooms[i]);
                }
            }

            foreach (Fact f in roomTypeFacts) //go through all room types
            {
                allLevels[0].setRoomType(f.getValue(1), f.getNumericValue(0));
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
