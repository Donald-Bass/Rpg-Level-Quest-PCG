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
        public Tile[,] levelMap { get; private set; } //all the tiles making up the level[x,y]. 0,0 is upper left
        //public levelType typeOfLevel { get; set; }

        public Room[] allRooms {get; set; }

        public string levelName { get; set; } //TODO FIGURE OUT NAME GEN

        public int maxRoomNumber { get; set; }

        public Level(int xDim, int yDim)
        {
            xDimension = xDim;
            yDimension = yDim;
            levelMap = new Tile[xDim, yDim];

            for (int i = 0; i < xDimension; i++)
            {
                for (int j = 0; j < yDimension; j++)
                {
                    levelMap[i, j] = new Tile();
                }
            }

            maxRoomNumber = 100; //enventually to stop wasting memory add code to properly detect how many rooms there are

            allRooms = new Room[maxRoomNumber]; 

            levelName = "";

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

        public void setTileAdditionalInformation(int x, int y, int info)
        {
            levelMap[x, y].additionalInformation = info;
        }


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

        //adds a room to the array of rooms and update tiles appropiately
        public void addRoom(int lowX, int lowY, int highX, int highY, int roomNumber, string RoomType)
        {
            Room toAdd = new Room();
            toAdd.XUL = lowX;
            toAdd.YUL = lowY;
            toAdd.XBR = highX;
            toAdd.YBR = highY;
            toAdd.roomType = RoomType;
            toAdd.roomNumber = roomNumber;

            Console.WriteLine("Adding Room " + roomNumber);

            allRooms[roomNumber] = toAdd;

        }

        //write the contents of the level to file (levelNumber is the number to give to this level in the file)
        public void write(System.IO.StreamWriter file)
        {
            Fact curFact;

            //output dimensions
            curFact = new Fact("levelLengthX", new String[] { xDimension.ToString()});
            file.Write(curFact.getStringRepresentation());

            curFact = new Fact("levelLengthY", new String[] { yDimension.ToString()});
            file.Write(curFact.getStringRepresentation());


            for(int i = 0; i < xDimension; i++)
            {
                for (int j = 0; j < yDimension; j++)
                {
                    if (levelMap[i, j].tType == TileType.floor) 
                    {
                        curFact = new Fact("floor", new String[] { i.ToString(), j.ToString() });
                        file.Write(curFact.getStringRepresentation());
                    }

                    else if (levelMap[i, j].tType == TileType.door)
                    {
                        curFact = new Fact("door", new String[] { i.ToString(), j.ToString() });
                        file.Write(curFact.getStringRepresentation());
                    }

                    else if(levelMap[i, j].tType == TileType.startingRoom) //for now we'll treat starting room tiles as different. Once rooms are handled more comprehensively
                                                                           //this will probally be obsolete
                    {
                        curFact = new Fact("startingRoom", new String[] { i.ToString(), j.ToString() });
                        file.Write(curFact.getStringRepresentation());
                    }

                    else if (levelMap[i, j].tType == TileType.key)
                    {
                        curFact = new Fact("key", new String[] { i.ToString(), j.ToString(), levelMap[i,j].additionalInformation.ToString() });
                        file.Write(curFact.getStringRepresentation());
                    }

                    else if (levelMap[i, j].tType == TileType.locked)
                    {
                        curFact = new Fact("lock", new String[] { i.ToString(), j.ToString(), levelMap[i, j].additionalInformation.ToString() });
                        file.Write(curFact.getStringRepresentation());
                    }
                }
            }

            for(int i = 0; i < maxRoomNumber; i++) //for each room
            {
                if(allRooms[i] != null) //if the room exists
                {
                    file.Write(allRooms[i].roomAtom().getStringRepresentation());
                }
            }
        }

    }

}
