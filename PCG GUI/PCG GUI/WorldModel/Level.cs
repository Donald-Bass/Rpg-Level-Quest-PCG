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

            allRooms = new Room[100]; 

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
        public void addRoom(Room toAdd)
        {
            allRooms[toAdd.roomNumber] = toAdd;

            levelMap[toAdd.X, toAdd.Y].RoomNumber = toAdd.roomNumber;
        }

        //write the contents of the level to file (levelNumber is the number to give to this level in the file)
        public void write(System.IO.StreamWriter file, int levelNumber, bool ClingoInput = false)
        {
            Fact curFact;

            curFact = new Fact("level", new String[] {levelNumber.ToString() });
            file.Write(curFact.getStringRepresentation(ClingoInput));

            //output dimensions
            curFact = new Fact("levelLengthX", new String[] { xDimension.ToString(), levelNumber.ToString() });
            file.Write(curFact.getStringRepresentation(ClingoInput));

            curFact = new Fact("levelLengthY", new String[] { yDimension.ToString(), levelNumber.ToString() });
            file.Write(curFact.getStringRepresentation(ClingoInput));

            /*
            //output level type
            if (typeOfLevel == levelType.interior)
            {
                curFact = new Fact("interior", new String[] { levelNumber.ToString() });
            }

            else
            {
                curFact = new Fact("exterior", new String[] { levelNumber.ToString() });
            }
            file.Write(curFact.getStringRepresentation(ClingoInput));*/


            for(int i = 0; i < xDimension; i++)
            {
                for (int j = 0; j < yDimension; j++)
                {
                    if(levelMap[i,j].tType == TileType.floor)
                    {
                        curFact = new Fact("floor", new String[] { i.ToString(), j.ToString(), levelNumber.ToString() });
                        file.Write(curFact.getStringRepresentation(ClingoInput));
                    }

                    else if (levelMap[i, j].tType == TileType.blocked && ClingoInput) //if inputing into clingo specifically mark tiles that can't be floors
                    {
                        curFact = new Fact("floor", new String[] { i.ToString(), j.ToString(), levelNumber.ToString() });
                        file.Write(curFact.getStringRepresentation(ClingoInput, true));
                    }

                    //check for north and west walls (south and east walls will normally be caught by tile to south/east
                    if(levelMap[i,j].westWall == WallType.wall)
                    {
                        curFact = new Fact("wallY", new String[] { i.ToString(), j.ToString(), levelNumber.ToString() });
                        file.Write(curFact.getStringRepresentation(ClingoInput));
                    }

                    else if (levelMap[i, j].westWall == WallType.door)
                    {
                        curFact = new Fact("doorY", new String[] { i.ToString(), j.ToString(), levelNumber.ToString() });
                        file.Write(curFact.getStringRepresentation(ClingoInput));
                    }

                    else if (ClingoInput && i != 0 && levelMap[i, j].tType == TileType.floor && levelMap[i - 1, j].tType == TileType.floor) //if there is a floor to the west and no wall there can't be a wall there
                    {
                        curFact = new Fact("barricadeY", new String[] { i.ToString(), j.ToString(), levelNumber.ToString() });
                        file.Write(curFact.getStringRepresentation(ClingoInput, true));
                    }

                    if (levelMap[i, j].northWall == WallType.wall)
                    {
                        curFact = new Fact("wallX", new String[] { i.ToString(), j.ToString(), levelNumber.ToString() });
                        file.Write(curFact.getStringRepresentation(ClingoInput));
                    }

                    if (levelMap[i, j].northWall == WallType.door)
                    {
                        curFact = new Fact("doorX", new String[] { i.ToString(), j.ToString(), levelNumber.ToString() });
                        file.Write(curFact.getStringRepresentation(ClingoInput));
                    }


                    else if (ClingoInput && j != 0 && levelMap[i, j].tType == TileType.floor  && levelMap[i, j - 1].tType == TileType.floor) //if there is a floor to the north and no wall there can't be a wall there
                    {
                        curFact = new Fact("barricadeX", new String[] { i.ToString(), j.ToString(), levelNumber.ToString() });
                        file.Write(curFact.getStringRepresentation(ClingoInput, true));
                    }

                    //if at the east or south edges check for east/south wall
                    if(i == xDimension - 1 && levelMap[i,j].eastWall == WallType.wall)
                    {
                        curFact = new Fact("wallY", new String[] { (i + 1).ToString(), j.ToString(), levelNumber.ToString() });
                        file.Write(curFact.getStringRepresentation(ClingoInput));
                    }

                    //if at the east or south edges check for east/south wall
                    else if (i == xDimension - 1 && levelMap[i, j].eastWall == WallType.door)
                    {
                        curFact = new Fact("doorY", new String[] { (i + 1).ToString(), j.ToString(), levelNumber.ToString() });
                        file.Write(curFact.getStringRepresentation(ClingoInput));
                    }


                    if (j == yDimension - 1 && levelMap[i, j].southWall == WallType.wall)
                    {
                        curFact = new Fact("wallX", new String[] { i.ToString(), (j + 1).ToString(), levelNumber.ToString() });
                        file.Write(curFact.getStringRepresentation(ClingoInput));
                    }

                    else if (j == yDimension - 1 && levelMap[i, j].southWall == WallType.door)
                    {
                        curFact = new Fact("doorX", new String[] { i.ToString(), (j + 1).ToString(), levelNumber.ToString() });
                        file.Write(curFact.getStringRepresentation(ClingoInput));
                    }
                }
            }
        }

        //set the type of a room
        /*public void setRoomType(string type, int roomNum)
        {
            allRooms[roomNum].roomType = type;

            TileType tileToSet = TileType.floor;

            if(type.Equals("treasure"))
            {
                tileToSet = TileType.treasureRoom;
            }

            else if (type.Equals("arena"))
            {
                tileToSet = TileType.arena;
            }

            for (int i = allRooms[roomNum].XUL; i <= allRooms[roomNum].XBR; i++)
            {
                for (int j = allRooms[roomNum].YUL; j <= allRooms[roomNum].YBR; j++)
                {
                    levelMap[i, j].tType = tileToSet;
                }
            }
            
        }*/
    }

}
