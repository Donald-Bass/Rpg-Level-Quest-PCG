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

    }
}
