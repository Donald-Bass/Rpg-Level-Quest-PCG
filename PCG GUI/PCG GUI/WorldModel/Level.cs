using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PCG_GUI.Facts
{
    enum levelType{interior, exterior};

    class Level
    {
        public int xDimension { get; private set; }  //length of level (x)
        public int yDimension { get; private set; } //height of level (y)
        public Tile[,] levelMap { get; private set; } //all the tiles making up the level[x,y]. 0,0 is upper left
        public levelType typeOfLevel { get; set; }

        public string levelName; //TODO FIGURE OUT NAME GEN

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

            typeOfLevel = levelType.interior; //default to interior for now

        }

        public void setTileType(int x, int y, TileType type)
        {
            levelMap[x, y].tType = type;
        }

        public void addWallX(int x, int y)
        {
            //the wall being added goes from X,Y to X+1,Y

            //if not at the very top of the map
            if (y != 0)
            {
                levelMap[x, y - 1].southWall = true;
            }

            if (y != yDimension)
            {
                levelMap[x, y].northWall = true;
            }
        }

        public void addWallY(int x, int y)
        {
            //the wall being added goeX from X,Y to X+1,Y

            //if not at the very top of the map
            if (x != 0)
            {
                levelMap[x - 1, y].eastWall = true;
            }

            if (x != xDimension)
            {
                levelMap[x, y].westWall = true;
            }
        }

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

        //write the contents of the level to file (levelNumber is the number to give to this level in the file)
        public void write(System.IO.StreamWriter file, int levelNumber)
        {
            Fact curFact;

            //output dimensions
            curFact = new Fact("levelLengthX", new String[] { xDimension.ToString(), levelNumber.ToString() });
            file.Write(curFact.getStringRepresentation() + " ");

            curFact = new Fact("levelLengthY", new String[] { yDimension.ToString(), levelNumber.ToString() });
            file.Write(curFact.getStringRepresentation() + " ");

            for(int i = 0; i < xDimension; i++)
            {
                for (int j = 0; j < yDimension; j++)
                {
                    if(levelMap[i,j].tType == TileType.floor)
                    {
                        curFact = new Fact("floor", new String[] { i.ToString(), j.ToString(), levelNumber.ToString() });
                        file.Write(curFact.getStringRepresentation() + " ");
                    }

                    else if (levelMap[i, j].tType == TileType.blocked)
                    {
                        //don't do anything yet. Representing a blocked floor is only necessary when passing to clingo and there are several other format changes necessary for that as well
                    }

                    //check for north and west walls (south and east walls will normally be caught by tile to south/east
                    if(levelMap[i,j].westWall)
                    {
                        curFact = new Fact("wallY", new String[] { i.ToString(), j.ToString(), levelNumber.ToString() });
                        file.Write(curFact.getStringRepresentation() + " ");
                    }

                    if (levelMap[i, j].northWall)
                    {
                        curFact = new Fact("wallX", new String[] { i.ToString(), j.ToString(), levelNumber.ToString() });
                        file.Write(curFact.getStringRepresentation() + " ");
                    }

                    //if at the east or south edges check for east/south wall
                    if(i == xDimension - 1 && levelMap[i,j].eastWall)
                    {
                        curFact = new Fact("wallY", new String[] { (i + 1).ToString(), j.ToString(), levelNumber.ToString() });
                        file.Write(curFact.getStringRepresentation() + " ");
                    }

                    if (j == yDimension - 1 && levelMap[i, j].southWall)
                    {
                        curFact = new Fact("wallX", new String[] { i.ToString(), (j + 1).ToString(), levelNumber.ToString() });
                        file.Write(curFact.getStringRepresentation() + " ");
                    }
                }
            }
        }

    }
}
