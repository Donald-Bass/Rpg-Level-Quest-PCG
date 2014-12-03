/* The Template class stores a template for a room layout. Essentially the template defines the shape of the room 
 * and is used to place a copy of said shape into the actual level.
 * 
 * The logic here is threefold. First it is an easy way to avoid just having square and rectangular rooms. Secondly it is intended to be used with rooms that are meant to hold certain types of events
 * (boss battles, treasure rooms, and all the other stuff we might want to implement), and ensure that the room created is appropiate to that type of event. The third though is to show that this 
 * method of pcg is actually viable with how the big studios would actually work, showing that it can fit arbitary templates in a level, should be roughly equivilent to being able to make use of arbitary
 * premade art assests
 *
 * Templates are read from a file the format for which is
 *    
 *         The first line is a single number which is the number of templates in the file
 *         The next line starts the first template. 
 *         The first line of the template is the x  dimensions of the template
 *         and the second is the y
 *         Each additional line holds one row of the template detailing each tile. Currently
 *              . is floor
 *              # is blocked terrain
 *
 * The file is actually read in, in the readTemplates function of the levelBuilder class (which also has a second copy of the above file format documentation)
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
        public Template(int xDim, int yDim) //create a template with a length of xDim and a height of yDim, and all tiles set to undefined
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

        public TileType getTileType(int x, int y) //get the type of the file at a specific set of coordinates
        {
            return templateMap[x, y].tType;
        }

        public void setTileType(int x, int y, TileType type) //set the type of the file at a specific set of coordinates
        {
            templateMap[x, y].tType = type;
        }

        //copy the contents of the template to a specific place in the level
        //xStart, and yStart are the coordinates of the upper left corner of the area of the level the template should overwrite
        //copyTo is the level that the template will actually be written to
        //roomNumber is the number of the room this template is being used to create
        //Currently the roomNumber isn't used for anything other then changing the floor of the starting room to stand out
        //but it could be more important in the future
        public void copyToLevel(int xStart, int yStart, Level copyTo, int roomNumber)
        {
            for(int i = xStart; i < xStart + xDimension; i++) //for each tile in the template
            {
                for (int j = yStart; j < yStart + yDimension; j++)
                {
                    //check if the room is the starting room (i.e the first room, room 0)
                    if (roomNumber != 0)
                    {
                        copyTo.setTileType(i, j, templateMap[i - xStart, j - yStart].tType); //convert from coordinates in the template to coordinates in the level and overwrite the tileType in the level
                                                                                             //with that found in the template
                    }

                    else //for now for the starting room place a different type of tile to represent the floor
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
