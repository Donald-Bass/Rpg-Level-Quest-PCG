/* The level class is used to store the actual contents of the level generated. Mainly its a big 2 dimensional array of tiles, as well as a list of rooms in the level
 * This is probally the class where I most violated the principle of encapsulation. A few too many classes right now modify the levelMap directly instead of using any provided functions
 */

using PCG_GUI.WorldModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PCG_GUI.Facts
{
    //public enum levelType{interior, exterior};

    public class Level
    {
        public int xDimension { get; private set; }  //length of level (x)
        public int yDimension { get; private set; } //height of level (y)
        public Tile[,] levelMap { get; private set; } //all the tiles making up the level[x,y]. 0,0 is upper left corner of the level 

        public Room[] allRooms {get; set; } //array containing all rooms in the level, and since it is currently sized larger then it should be with null values for at the indexes of all additional rooms that don't really exist

        public int maxRoomNumber { get; set; } //how many rooms did we create in the index (not the same as home many rooms are actually in the level currently. See my comments in the constructor) 

        public Level(int xDim, int yDim)
        {
            xDimension = xDim;
            yDimension = yDim;
            levelMap = new Tile[xDim, yDim];

            //create all the tiles in the level
            for (int i = 0; i < xDimension; i++)
            {
                for (int j = 0; j < yDimension; j++)
                {
                    levelMap[i, j] = new Tile();
                }
            }

            //Right now we are not bothering to check how many rooms there are and create a properly sized array, so just create an array bigger then we'll likely ever need
            //Rooms don't take that much memory space up anyways
            maxRoomNumber = 100; 
            allRooms = new Room[maxRoomNumber]; 

            //typeOfLevel = levelType.interior; //default to interior for now

        }


        public TileType getTileType(int x, int y)
        {
            return levelMap[x, y].tType;
        }

        public void setTileType(int x, int y, TileType type)
        {
            levelMap[x, y].tType = type;
        }

        //set additional information for a tile (this is used to store stuff like the number of the key a tile contains, or that will open a locked door in a tile
        public void setTileAdditionalInformation(int x, int y, int info)
        {
            levelMap[x, y].additionalInformation = info;
        }

        //Old code for working with walls as the boundaries between tiles. See the tile class for some discussion of what I'm talking about here
        /*
        public void addWallX(int x, int y, WallType type)
        {
            //the wall being added goes from X,Y to X+1,Y

            //if not at the very top of the map
            if (y != 0)
            {
                levelMap[x, y - 1].southWall = type;
            }

            if (y != yDimension)
            {
                levelMap[x, y].northWall = type;
            }
        }

        public void addWallY(int x, int y, WallType type)
        {
            //the wall being added goeX from X,Y to X+1,Y

            //if not at the very top of the map
            if (x != 0)
            {
                levelMap[x - 1, y].eastWall = type;
            }

            if (x != xDimension)
            {
                levelMap[x, y].westWall = type;
            }
        }

        public void removeWallX(int x, int y)
        {
            //the wall being added goes from X,Y to X+1,Y

            //if not at the very top of the map
            if (y != 0)
            {
                levelMap[x, y - 1].southWall = WallType.none;
            }

            if (y != yDimension)
            {
                levelMap[x, y].northWall = WallType.none;
            }
        }

        public void removeWallY(int x, int y)
        {
            //the wall being added goeX from X,Y to X+1,Y

            //if not at the very top of the map
            if (x != 0)
            {
                levelMap[x - 1, y].eastWall = WallType.none;
            }

            if (x != xDimension)
            {
                levelMap[x, y].westWall = WallType.none;
            }
        }*/


        //covert all undefined tiles into blocked tiles
        public void finalizeLevel()
        {
            for (int i = 0; i < xDimension; i++)
            {
                for (int j = 0; j < yDimension; j++)
                {
                    if (levelMap[i, j].tType == TileType.undefined)
                    {
                        levelMap[i, j].tType = TileType.blocked;
                    }
                }
            }
        }

        //adds a room to the array of rooms, with the roomNumber being both the number of the room, and what will ultimately be used as it's index
        public void addRoom(int lowX, int lowY, int highX, int highY, int roomNumber, string RoomType)
        {
            Room toAdd = new Room();
            toAdd.XUL = lowX;
            toAdd.YUL = lowY;
            toAdd.XBR = highX;
            toAdd.YBR = highY;
            toAdd.roomType = RoomType;
            toAdd.roomNumber = roomNumber;

            //Console.WriteLine("Adding Room " + roomNumber);

            allRooms[roomNumber] = toAdd;

        }

        //write the contents of the level to file. This file will mimic the format clingo outputs in currently. This is an artifact of earlier plans where Clingo would generate a complete level
        //and the GUI could modify that, and as such it made sense for the GUI's changes to be saved in an identical format and by the time it turned out that fully generating a level in Clingo took too
        //long all the other code expected said format and it made no sense to change it
        public void write(System.IO.StreamWriter file)
        {
            Atom curFact; //atom containing the current piece of information to output

            //output dimensions
            curFact = new Atom("levelLengthX", new String[] { xDimension.ToString()});
            file.Write(curFact.getStringRepresentation());

            curFact = new Atom("levelLengthY", new String[] { yDimension.ToString()});
            file.Write(curFact.getStringRepresentation());

            //output all the tiles in the level
            for(int i = 0; i < xDimension; i++)
            {
                for (int j = 0; j < yDimension; j++)
                {
                    //output an atom that varies based on the type of tile we are looking at.
                    //Note that there is no clause for blocked tiles. These don't need to be saved as when we load a level we automatically set all tiles that don't get set to a specific type
                    //to Blocked
                    if (levelMap[i, j].tType == TileType.floor) 
                    {
                        curFact = new Atom("floor", new String[] { i.ToString(), j.ToString() });
                        file.Write(curFact.getStringRepresentation());
                    }

                    else if (levelMap[i, j].tType == TileType.door)
                    {
                        curFact = new Atom("door", new String[] { i.ToString(), j.ToString() });
                        file.Write(curFact.getStringRepresentation());
                    }

                    else if(levelMap[i, j].tType == TileType.startingRoom) //for now we'll treat starting room tiles as different. Once rooms are handled more comprehensively
                                                                           //this will probally be obsolete
                    {
                        curFact = new Atom("startingRoom", new String[] { i.ToString(), j.ToString() });
                        file.Write(curFact.getStringRepresentation());
                    }

                    else if (levelMap[i, j].tType == TileType.key)
                    {
                        curFact = new Atom("key", new String[] { i.ToString(), j.ToString(), levelMap[i,j].additionalInformation.ToString() });
                        file.Write(curFact.getStringRepresentation());
                    }

                    else if (levelMap[i, j].tType == TileType.locked)
                    {
                        curFact = new Atom("lock", new String[] { i.ToString(), j.ToString(), levelMap[i, j].additionalInformation.ToString() });
                        file.Write(curFact.getStringRepresentation());
                    }
                }
            }

            for(int i = 0; i < maxRoomNumber; i++) //output an atom for each room
            {
                if(allRooms[i] != null) //if the room exists
                {
                    file.Write(allRooms[i].roomAtom().getStringRepresentation()); 
                }
            }
        }

    }

}
