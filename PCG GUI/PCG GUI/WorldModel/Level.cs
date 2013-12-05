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
        private int xDimension; //length of level (x)
        private int yDimension; //height of level (y)

        private Tile[,] levelMap; //all the tiles making up the level[x,y]
        public levelType typeOfLevel { get; set; }

        public string levelName; //TODO FIGURE OUT NAME GEN

        public Level(int xDim, int yDim)
        {
            xDimension = xDim;
            yDimension = yDim;
            levelMap = new Tile[xDim,yDim];

            for(int i = 0; i < xDimension; i++)
            {
                for(int j = 0; j < yDimension; j++)
                {
                    levelMap[i, j] = new Tile();
                }
            }

            typeOfLevel = interior; //default to interior for now

        }



    }
}
