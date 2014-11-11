/* The Template class stores a template for a room layout. 
 * 
 * 
 *
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCG_GUI.Facts;

namespace PCG_GUI.WorldModel
{
    public class Template
    {
        public int xDimension { get; private set; }  //length of level (x)
        public int yDimension { get; private set; } //height of level (y)
        public Tile[,] templateMap { get; private set; } //all the tiles making up the template. 0,0 is upper left
        public Template(int xDim, int yDim)
        {
            xDimension = xDim;
            yDimension = yDim;
            templateMap = new Tile[xDim, yDim];

            for (int i = 0; i < xDimension; i++)
            {
                for (int j = 0; j < yDimension; j++)
                {
                    templateMap[i, j] = new Tile();
                }
            }
        }

        public TileType getTileType(int x, int y)
        {
            return templateMap[x, y].tType;
        }

        public void setTileType(int x, int y, TileType type)
        {
            templateMap[x, y].tType = type;
        }

        //copy the contents of the template to a specific place in the level
        //Currently the roomNumber isn't used for anything other then changing the floor of the starting room to stand out
        //but it could be more important in the future
        public void copyToLevel(int xStart, int yStart, Level copyTo, int roomNumber)
        {
            for(int i = xStart; i < xStart + xDimension; i++)
            {
                for (int j = yStart; j < yStart + yDimension; j++)
                {
                    if (roomNumber != 0)
                    {
                        copyTo.setTileType(i, j, templateMap[i - xStart, j - yStart].tType);
                    }

                    else
                    {
                        if(templateMap[i - xStart, j - yStart].tType == TileType.floor) //replace all floor with starting room tiles
                        {
                            copyTo.setTileType(i, j, TileType.startingRoom);
                        }

                        else //otherwise just copy whatever is in the template
                        {
                            copyTo.setTileType(i, j, templateMap[i - xStart, j - yStart].tType);
                        }
                    }
                }
            }
        }
    }
}
