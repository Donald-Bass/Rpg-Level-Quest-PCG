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
    /*public class layoutGridTile
    {
        bool room;
        bool eastCor;
        bool westCor;
        bool northCor;
        bool southCor;
    }*/

    public class LevelBuilder
    {
        private List<Fact> roomFacts; //facts giving information about rooms
        private List<Fact> edgeFacts; //facts giving information about connections between rooms
        private List<Fact> keyFacts; //facts giving information about the location of keys
        private List<Fact> tileFacts; //facts giving information about the layout of individual tiles


        //dimensions of the grid the level is laid out on
        const int LEVEL_X_DIMENSION = 10; 
        const int LEVEL_Y_DIMENSION = 10;

        public int numRooms;

        private Level pcgLevel; //the level generated

        private List<List<Template>> allTemplates; //list of lists of all templates. Currently (and I admit this is ugly, index 0 is boss rooms, 1 is treasure rooms, 2 is gauntlets, 
                                                   //3 is entrances, and 4 is generic rooms
        const int BOSS_INDEX = 0;
        const int TREASURE_INDEX = 1;
        const int GAUNTLET_INDEX = 2;
        const int ENTRANCE_INDEX = 3;
        const int GENERIC_INDEX = 4;

        int loadedLevelX; //if a level is loaded from a file this stores the size of the x Axis
        int loadedLevelY; //if a level is loaded from a file this stores the size of the x Axis

        public LevelBuilder()
        {
            roomFacts = new List<Fact>();
            edgeFacts = new List<Fact>();
            keyFacts = new List<Fact>();
            tileFacts = new List<Fact>();

            //set up the templates
            allTemplates = new List<List<Template>>();

            List<Template> curTemplatesList = readTemplates("boss.txt");
            allTemplates.Add(curTemplatesList);
            curTemplatesList = readTemplates("treasure.txt");
            allTemplates.Add(curTemplatesList);
            curTemplatesList = readTemplates("gauntlet.txt");
            allTemplates.Add(curTemplatesList);
            curTemplatesList = readTemplates("entrance.txt");
            allTemplates.Add(curTemplatesList);
            curTemplatesList = readTemplates("generic.txt");
            allTemplates.Add(curTemplatesList);
        }

        /*This reads in a series of templates from an input file. The format is simple
        * The first line is a single number which is the number of templates in the file
        * The next line starts the first template. 
        * The first line of the template is the x  dimensions of the template
        * and the second is the y
        * Each additional line holds one row of the template detailing each tile. Currently
        *       . is floor
        *      # is blocked terrain
        * This function does no error checking currently      
        */
        public List<Template> readTemplates(string templateFile)
        {
            List<Template> tList = new List<Template>();

            System.IO.StreamReader file = new System.IO.StreamReader(templateFile);

            int numTemplates = int.Parse(file.ReadLine()); //the first line is the number of templates

            for (int i = 0; i < numTemplates; i++)
            {
                int xDimension = int.Parse(file.ReadLine()); //the first line is the number of templates
                int yDimension = int.Parse(file.ReadLine()); //the first line is the number of templates

                Template curTemplate = new Template(xDimension, yDimension);

                //read in each line of the template
                for(int j = 0; j < yDimension; j++)
                {
                    string templateRow = file.ReadLine();

                    for(int k = 0; k < xDimension; k++) //for each tile in the line
                    {
                        switch(templateRow[k])
                        {
                            case '#':
                                curTemplate.setTileType(k, j, TileType.blocked);
                                break;
                            case '.':
                                curTemplate.setTileType(k, j, TileType.floor);
                                break;
                        }
                    }
                }

                //add the finished template to the list
                tList.Add(curTemplate);

            }

            file.Close();

            return tList;
        }

        public void parseClingoFile(System.IO.StreamReader file)
        {
            Fact curFact;
            
            String firstLine = file.ReadLine(); //the first line notes whether this is a input file from Clingo or an already complete level saved by the GUI
            file.ReadLine(); //the next 3 lines contain nothing useful
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

            if (firstLine.Equals("COMPLETE")) //if this was marked as output from the GUI load a completed level
            {
                loadLevel();
            }

            else //otherwise finish a partially made level from Clingo
            {
                buildLevels();
            }
            //return allFacts;
        }

        public void writeWorldFile(System.IO.StreamWriter file)
        {
            file.WriteLine("COMPLETE"); //the clingo output has 4 unecessary line at the start so use the first line to denote that this is not the clingo output file
                                        //and then add 3 more to match the format
            file.WriteLine("");
            file.WriteLine(""); 
            file.WriteLine("");

            pcgLevel.write(file);
        }

        //does the inital processing of facts. The order is not all that reliable so store everything that has any prequristies to be processed
        private void sortFact(Fact fact)
        {
            switch (fact.getPredicate())
            {
                //Below this point are atoms that are used either when loading a partialy finished level from Clingo or a completed level finished by the GUI
                //The meaning may change though based on which one it is
                case "room":
                    roomFacts.Add(fact);
                    break;
                //These are atoms currently only used when reading in partially complete levels from Clingo
                case "numRooms": //if the atom specifies the number of rooms we can use that information right now.
                    numRooms = fact.getNumericValue(0);
                    break;
                case "uniqueEdge":
                    edgeFacts.Add(fact);
                    break;
                case "uniqueLock":
                    edgeFacts.Add(fact);
                    break;
                case "keyRoom":
                    keyFacts.Add(fact);
                    break;
                //Below this point are atoms used exclusively for loaded already completed levels
                case "floor":
                case "door" :
                case "startingRoom":
                case "lock":
                case "key":
                    tileFacts.Add(fact);
                    break;
                case "levelLengthX": //we can use the dimensions of the level right now. No need to store anything
                    loadedLevelX = fact.getNumericValue(0);
                    break;
                case "levelLengthY":
                    loadedLevelY = fact.getNumericValue(0);
                    break;
            }
        }

        //take a level fully defined in an output file and load it
        private void loadLevel()
        {
            pcgLevel = new Level(loadedLevelX, loadedLevelY);

            foreach (Fact t in tileFacts) //read in all the tiles and place them in the level
            {
                int x = t.getNumericValue(0); //x coordinate of tile
                int y = t.getNumericValue(1); //y coordinate of tile
                TileType type = TileType.undefined; //type of tile to add


                switch(t.getPredicate()) //act different depending on what type of atom it is
                {
                    case "floor":
                        type = TileType.floor;
                        break;
                    case "door":
                        type = TileType.door;
                        break;
                    case "startingRoom":
                        type = TileType.startingRoom;
                        break;
                    case "key":
                        type = TileType.key;
                        pcgLevel.setTileAdditionalInformation(x,y,t.getNumericValue(2)); //extract the key number
                        pcgLevel.levelMap[x, y].RoomNumber = t.getNumericValue(2); //set the room number of the tile to the key number as a quick hack to get the number displayed
                        break;
                    case "lock":
                        type = TileType.locked;
                        pcgLevel.setTileAdditionalInformation(x, y, t.getNumericValue(2)); //extract the key number
                        pcgLevel.levelMap[x, y].RoomNumber = t.getNumericValue(2); //set the room number of the tile to the key number as a quick hack to get the number displayed
                        break;
                }

                pcgLevel.setTileType(x, y, type);
            }
           
            foreach (Fact r in roomFacts) //read in all the rooms and store them
            {
                pcgLevel.addRoom(r.getNumericValue(0), r.getNumericValue(1), r.getNumericValue(2), r.getNumericValue(3), r.getNumericValue(4), r.getValue(5));
            }

            pcgLevel.finalizeLevel();
        }

        //go through the level facts to build all the levels
        private void buildLevels()
        {
            //Step 1. Determine the ultimate dimensions of the level. To do so create a grid with the positions of all the rooms then decide how many tiles to allocate to each row/column based
            //on the room types

            int[] columnWidths = new int[LEVEL_X_DIMENSION];
            int[] rowHeights = new int[LEVEL_Y_DIMENSION];

            int[] roomXCenter = new int[numRooms];
            int[] roomYCenter = new int[numRooms];

            int[] templateIndex = new int[numRooms];

            int x, y;

            Random rng = new Random();

            //For each room read in the row and column the room is in (X is the column and Y is the row)
            //Determine how big the room should be, and if the column/row isn't large enough to hold the room place increase the size
            foreach (Fact f in roomFacts) //read in each room
            {
                int roomNumber = f.getNumericValue(0) - 1;
                x = f.getNumericValue(1);
                y = f.getNumericValue(2);

                int roomDimensionX = 0;
                int roomDimensionY = 0;

                string roomType = f.getValue(3);

                if (roomType.Equals("boss"))
                {
                    templateIndex[roomNumber] = rng.Next(allTemplates[BOSS_INDEX].Count);
                    roomDimensionX = allTemplates[BOSS_INDEX][templateIndex[roomNumber]].xDimension;
                    roomDimensionY = allTemplates[BOSS_INDEX][templateIndex[roomNumber]].yDimension;
                }

                else if (roomType.Equals("gauntlet"))
                {
                    templateIndex[roomNumber] = rng.Next(allTemplates[GAUNTLET_INDEX].Count);
                    roomDimensionX = allTemplates[GAUNTLET_INDEX][templateIndex[roomNumber]].xDimension;
                    roomDimensionY = allTemplates[GAUNTLET_INDEX][templateIndex[roomNumber]].yDimension;
                }

                else if (roomType.Equals("treasure"))
                {
                    templateIndex[roomNumber] = rng.Next(allTemplates[TREASURE_INDEX].Count);
                    roomDimensionX = allTemplates[TREASURE_INDEX][templateIndex[roomNumber]].xDimension;
                    roomDimensionY = allTemplates[TREASURE_INDEX][templateIndex[roomNumber]].yDimension;
                }

                else if (roomType.Equals("entrance"))
                {
                    templateIndex[roomNumber] = rng.Next(allTemplates[ENTRANCE_INDEX].Count);
                    roomDimensionX = allTemplates[ENTRANCE_INDEX][templateIndex[roomNumber]].xDimension;
                    roomDimensionY = allTemplates[ENTRANCE_INDEX][templateIndex[roomNumber]].yDimension;
                }

                else //if (roomType.Equals("generic"))
                {
                    templateIndex[roomNumber] = rng.Next(allTemplates[GENERIC_INDEX].Count);
                    roomDimensionX = allTemplates[GENERIC_INDEX][templateIndex[roomNumber]].xDimension;
                    roomDimensionY = allTemplates[GENERIC_INDEX][templateIndex[roomNumber]].yDimension;
                }

                //add a 1 tile wide boundary around the dimensions of the room
                roomDimensionX += 2;
                roomDimensionY += 2;

                if (rowHeights[y] < roomDimensionY)
                {
                    rowHeights[y] = roomDimensionY;
                }

                if (columnWidths[x] < roomDimensionX)
                {
                    columnWidths[x] = roomDimensionX;
                }
            }

            //find the total width and height of the level
            int width = 0;
            foreach(int i in columnWidths)
            {
                width += i;
            }

            int height = 0;
            foreach(int i in rowHeights)
            {
                height += i;
            }

            pcgLevel = new Level(width, height);

            System.Console.WriteLine(width + " " + height);

            //Now place each room
            foreach (Fact f in roomFacts)
            {
                int roomNumber = f.getNumericValue(0) - 1;
                x = f.getNumericValue(1);
                y = f.getNumericValue(2);

                int roomDimensionX = 0;
                int roomDimensionY = 0;

                string roomType = f.getValue(3);

                System.Console.WriteLine(x + " " + y + " " + roomType);

                int templateListIndex; //index of the sepcific list of templates to use

                //redetermine the roomsize. Determine the right list of templates then reaquire the dimensions
                if (roomType.Equals("boss"))
                {
                    templateListIndex = BOSS_INDEX;
                }

                else if (roomType.Equals("gauntlet"))
                {
                    templateListIndex = GAUNTLET_INDEX;
                }

                else if (roomType.Equals("treasure"))
                {
                    templateListIndex = TREASURE_INDEX;
                }

                else if (roomType.Equals("entrance"))
                {
                    templateListIndex = ENTRANCE_INDEX;
                }

                else //if (roomType.Equals("generic"))
                {
                    templateListIndex = GENERIC_INDEX;
                }

                roomDimensionX = allTemplates[templateListIndex][templateIndex[roomNumber]].xDimension;
                roomDimensionY = allTemplates[templateListIndex][templateIndex[roomNumber]].yDimension;

                System.Console.WriteLine(roomDimensionX + " " + roomDimensionY);

                //determine where the rooms row and column starts
                int rowStart = 0; //the first y value belonging to the row
                int columnStart = 0; //the first x value belonging to the column

                for(int i = 0; i < y; i++)
                {
                    rowStart += rowHeights[i];
                }

                for (int i = 0; i < x; i++)
                {
                    columnStart += columnWidths[i];
                }

                //all the room sizes are currently only using odd numbers so this room should have a center tile. Determine where that is

                int centerX = columnStart + (int)(columnWidths[x] / 2); //since the widths are odd we can integer divide by 2 then add 1 to get distance to the center. (except the values we need are 0 indexed so the +1 is canceled out)
                int centerY = rowStart + (int)(rowHeights[y] / 2); //since the heights are odd we can integer divide by 2 then add 1 to get distance to the center (except the values we need are 0 indexed so the +1 is canceled out)

                System.Console.WriteLine(centerX + " " + centerY);

                //store the centers
                roomXCenter[roomNumber] = centerX;
                roomYCenter[roomNumber] = centerY;

                //now determine the upper left and bottom right corners of the room (the corner closest to 0,0 and fartherst from respectively)
               
                int lowX = centerX - (int)roomDimensionX / 2;
                
                int lowY = centerY - (int)roomDimensionY / 2;
                int highX = centerX + (int)roomDimensionX / 2;
                
                int highY = centerY + (int)roomDimensionY / 2;

                //place the room
                allTemplates[templateListIndex][templateIndex[roomNumber]].copyToLevel(lowX, lowY, pcgLevel, roomNumber);

                pcgLevel.addRoom(lowX, lowY, highX, highY, roomNumber, roomType);
            }

            //now add the edges (corridors)
            foreach (Fact f in edgeFacts)
            {

                //get the rooms at either end
                int room1 = f.getNumericValue(0) - 1;
                int room2 = f.getNumericValue(1) - 1;

                int keyNumber = -1; //if the corridor is locked which number key opens it

                if (f.getPredicate().Equals("uniqueLock")) //if there is a lock
                {
                    keyNumber = f.getNumericValue(2);
                }

                //keep track of whether the two necessary doors have been placed
                bool door1Placed = false;
                bool door2Placed = false;
                int prevX = 0; // track the previous tile checked for use in placing doors correctly
                int prevY = 0; 

                //the corridor can be either vertical or horizontal, and in each case its possible for either room1 or room2 to be the top/left room
                //So determine the start and end points of the corridors. For convience sake for now I am going to start/end each corridor in the center of the rooms
                //This will cause some tiles to be set to floor twice but shouldn't hurt anything
                
                int lowX, highX, lowY, highY;

                if(roomXCenter[room1] < roomXCenter[room2])
                {
                    lowX = roomXCenter[room1];
                    highX = roomXCenter[room2];
                }

                else
                {
                    lowX = roomXCenter[room2];
                    highX = roomXCenter[room1];
                }

                if (roomYCenter[room1] < roomYCenter[room2])
                {
                    lowY = roomYCenter[room1];
                    highY = roomYCenter[room2];
                }

                else
                {
                    lowY = roomYCenter[room2];
                    highY = roomYCenter[room1];
                }

                for (int i = lowX; i <= highX; i++)
                {
                    for (int j = lowY; j <= highY; j++)
                    {
                            if (!door1Placed && pcgLevel.getTileType(i, j) == TileType.undefined) //the first door is placed as soon as we hit an undefined tile (ie leave the current room)
                            {
                                if (keyNumber != -1) //if the corridor is locked
                                {
                                    pcgLevel.setTileType(i, j, TileType.locked);
                                    pcgLevel.setTileAdditionalInformation(i, j, keyNumber); //mark the number of the keys

                                    pcgLevel.levelMap[i, j].RoomNumber = keyNumber; //as a quick hack to make the key number visible set the room
                                    //number to the key number which as part of a previous hack is displayed on the map
                                }

                                else //otherwise the corridor is not locked
                                {
                                    pcgLevel.setTileType(i, j, TileType.door);
                                }

                                door1Placed = true;
                            }

                            else if(door1Placed && !door2Placed) //once the first door is placed we can place the corridor
                            {
                                if (!door2Placed && pcgLevel.getTileType(i, j) != TileType.undefined) //The second door goes one tile before the start of the second room so once we reach that backtrack one tile
                                {
                                    door2Placed = true;

                                    if (keyNumber != -1) //if the corridor is locked
                                    {
                                        pcgLevel.setTileType(prevX, prevY, TileType.locked);
                                        pcgLevel.setTileAdditionalInformation(prevX, prevY, keyNumber); //mark the number of the keys

                                        pcgLevel.levelMap[prevX, prevY].RoomNumber = keyNumber; //as a quick hack to make the key number visible set the room
                                                                                                //number to the key number which as part of a previous hack is displayed on the map
                                    }

                                    else //otherwise the corridor is not locked
                                    {
                                        pcgLevel.setTileType(prevX, prevY, TileType.door);
                                    }
                                }

                                else //otherwise we have not reache the end of the corridor so place floor
                                {
                                    pcgLevel.setTileType(i, j, TileType.floor);
                                }
                            }
                        
                        //keep track of where we were just at
                        prevX = i;
                        prevY = j;
                    }
                }
            }

            //finally mark locks and keys
            //right now this is mostly to easily see where the locks and keys are. The actual method of displaying this is not final
            foreach (Fact f in keyFacts)
            {
                if(f.getPredicate().Equals("keyRoom")) //if the atom is a keyRoom place a key tile in the center 
                {
                    int room = f.getNumericValue(0) - 1; //get the room number and convert to being zero index
                    int key = f.getNumericValue(1); //get the key number

                    pcgLevel.setTileType(roomXCenter[room], roomYCenter[room], TileType.key);
                    pcgLevel.setTileAdditionalInformation(roomXCenter[room], roomYCenter[room], key); //mark the number of the key

                    //as a temporary measure set the room number of the tile to the key number
                    pcgLevel.levelMap[roomXCenter[room], roomYCenter[room]].RoomNumber = key;
                        
                }

            }

            pcgLevel.finalizeLevel();
        }

        
        //setters and getters
        public Level getLevel()
        {
            return pcgLevel;
        }

    }

    
}
