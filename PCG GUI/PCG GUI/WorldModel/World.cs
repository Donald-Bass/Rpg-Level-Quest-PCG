//holds the state of the entire world.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PCG_GUI.Facts
{
    class World
    {
        private List<Fact> npcFacts; //facts necessary to build the level
        private List<Fact> questFacts; //facts necessary to build the level
        private List<Fact> levelFacts; //facts necessary to build the level
        private List<Fact> levelDimensionFacts; //facts storying dimensions of levels
        public int numLevels { get; private set; } //how many different levels are there
        int[,] levelDimensions; //dimensions of each level. First index is level number second is x/y (0 is x dimension 1 is y dimension)
        Level[] allLevels; //all levels

        public World()
        {
            numLevels = -1;
            npcFacts = new List<Fact>();
            levelFacts = new List<Fact>();
            levelDimensionFacts = new List<Fact>();
            questFacts = new List<Fact>(); 
        }

        public void parseClingoFile(System.IO.StreamReader file)
        {
            Fact curFact;
            
            file.ReadLine(); //the first line is the number of answers which we can skip
            String[] StringFacts = file.ReadLine().Trim().Split(' '); //the second line has the model so split all the facts into seperate strings

            //parse all facts
            //allFacts = new Fact[StringFacts.Length];



            for (int i = 0; i < StringFacts.Length; i++ )
            {
                curFact = new Fact(StringFacts[i]);
                sortFact(curFact);
            }

            setUpLevels();

            buildLevels();

            //return allFacts;
        }

        public void writeClingoFile(System.IO.StreamWriter file)
        {
            file.WriteLine(""); //the clingo output has a unecessary line at the start so add an blank line to match the formula

            Fact totalLevels = new Fact("totalLevels", new String[] { numLevels.ToString() });
            file.Write(totalLevels.getStringRepresentation() + " ");

            //placeholder till npcs are actually stored in memory
            foreach (Fact f in npcFacts)
            {
                file.Write(f.getStringRepresentation() + " ");
            }

            //placeholder till quests are actually stored in memory
            foreach (Fact f in questFacts)
            {
                file.Write(f.getStringRepresentation() + " ");
            }

            for(int i = 0; i < numLevels; i++)
            {
                allLevels[i].write(file, i);
            }
        }

        //does the inital processing of facts. The order is not all that reliable so store everything that has any prequristies to be processed
        private void sortFact(Fact fact)
        {
            switch (fact.getPredicate())
            {
                case "totalLevels":
                    numLevels = fact.getNumericValue(0); //get the number of levels from the fact
                    break;

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
                case "tree":
                    levelFacts.Add(fact);
                    break;
                case "interior":
                    levelFacts.Add(fact);
                    break;
                case "exterior":
                    levelFacts.Add(fact);
                    break;
                case "wallX":
                    levelFacts.Add(fact);
                    break;
                case "wallY":
                    levelFacts.Add(fact);
                    break;

                //npc facts
                case "npc":
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
                    break;
                default:
                    System.Console.Error.WriteLine("Unknown predicate type " + fact.getPredicate());
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

            //generate level objects
            allLevels = new Level[numLevels];

            for(int i = 0; i < numLevels; i++)
            {
                allLevels[i] = new Level(levelDimensions[i, 0], levelDimensions[i, 1]);
            }
        }

        //process a level dimension fact
        private void processDimensionFact(Fact f)
        {
            if(f.getPredicate() == "levelLengthX")
            {
                //the levelLenghtX fact has form(length,level)
                levelDimensions[f.getNumericValue(1), 0] = f.getNumericValue(0);
            }

            else if (f.getPredicate() == "levelLengthY")
            {
                //the levelLenghtY fact has form(length,level)
                levelDimensions[f.getNumericValue(1), 1] = f.getNumericValue(0);
            }

            else
            {
                System.Console.Error.WriteLine("Error predicate type " + f.getPredicate() + " should not be in levelDimensionFacts list");
            }
        }
    
        //go through the level facts to build all the levels
        private void buildLevels()
        {
            //process all the level dimensions TO DO POSSIBLY CHECK FOR MISSING DIMESNIONS
            foreach (Fact f in levelFacts)
            {
                switch (f.getPredicate())
                {
                    case "floor": //floor format is (X,Y,L)
                        allLevels[f.getNumericValue(2)].setTileType(f.getNumericValue(0), f.getNumericValue(1), TileType.floor);
                        break;
                    case "tree":
                        //levelFacts.Add(fact);
                        break;
                    case "interior": //interior format is (L)
                        allLevels[f.getNumericValue(0)].typeOfLevel = levelType.interior;
                        break;
                    case "exterior": //exterior format is (L)
                        allLevels[f.getNumericValue(0)].typeOfLevel = levelType.exterior;
                        break;
                    case "wallX": //wallX(X,Y,L) is a wall from X,Y to X+1,Y in L
                        allLevels[f.getNumericValue(2)].addWallX(f.getNumericValue(0), f.getNumericValue(1));
                        break;
                    case "wallY": //wallY(X,Y,L) us a wakk from X,Y to X,Y+1
                        allLevels[f.getNumericValue(2)].addWallY(f.getNumericValue(0), f.getNumericValue(1));
                        break;
                    default:
                        System.Console.Error.WriteLine("Error predicate type " + f.getPredicate() + " should not be in levelFact list");
                        break;
                }
            }

            foreach(Level l in allLevels)
            {
                l.finalizeLevel();
            }
        }

        //setters and getters
        public Level getLevel(int levelNum)
        {
            return allLevels[levelNum];
        }
    }


 
    
}
