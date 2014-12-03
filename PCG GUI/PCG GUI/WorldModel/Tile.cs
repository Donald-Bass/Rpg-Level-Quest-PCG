/*The tile class is used to store information about each individual tile in the level generated. It really doesn't do much right now, some of the stuff it was used for in the past no longer is 
 * part of the scope of the project, and it may possibly be more useful in the future but right now it just holds a few variables for each tile*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG_GUI.Facts
{
    public enum TileType{floor, blocked, undefined, door, key, locked, startingRoom}; //Define all the valid tile types
                                                                                      //floor - an open tile you can walk through
                                                                                      //blocked - a tile cannot be passed through (mainly used for walls and the like
                                                                                      //undefined - a tile type that has no meaning in terms of the level itself. Its the default type of tile 
                                                                                      //and if a tile is undefined it basically means that it still needs to be set. As the last step of generating a level
                                                                                      //the program will actually go through and change all undefined tiles into blocked tiles
                                                                                      //door - a tile noting a door is postioned there
                                                                                      //key - a tile that notes a key is found at it's position. For this (additionalInformation is used to set the key number)
                                                                                      //locked - like a door tile, except the door requires the key whose number is stored in additionalInformation to open
                                                                                      //startingRoom - this is just a temporary ugly hack. A startingRoom tile is identical to a floor tile, except it is only found
                                                                                      //in the first room of the level. The only reason it is seperate for now is to make it easier to highlight where the starting point of the level is in
                                                                                      //the map, there is really no deep significance behind this tile type. If you want to remove this
                                                                                      //the code that actually places this type of tile is in the Template class's copyToLevel function



    /* This class has a few commented out references to walls, that may or may not need to be used again depending on how the final results will ultimately be transformed into a playable level.
     * The issue here is how different tile based games represent walls. Some games have walls as impassable tiles so with # being a wall, . a floor,  and --- being the border between tiles two adjacent corridors would 
     * look like this
     * 
     * #######################################
     * ---------------------------------------
     * .......................................
     * ---------------------------------------
     * #######################################
     * ---------------------------------------
     * .......................................
     * ---------------------------------------
     * #######################################
     * 
     * Other games though don't have walls as tiles but instead place them on the boundaries between tiles. So to diagram that let = be a boundary that is a wall as opposed to the - being a boundary with no wall
     * 
     * ================================================================
     * ................................................................
     * ================================================================
     * ................................................................
     * ================================================================
     * 
     * The point is that depending on which definition of a wall I use I need to handle things in different ways. The code is currently using the former definition, but at one point it used the latter
     * and one consquence of that is that each tile needs to keep track of whether each edge of the tile contains a wall or not.
     * 
     * If you want to go back that way you will need to uncomment out the wall variables, change the pcg to properly set them, and go into the BaseViewModel to change it to display the walls. For the last
     * bit what you are looking for is the part of the drawing code that draws the grid lines. You'll need to check for each grid line whether there is a wall between the tile tiles the line seperates
     * and adjust the display accordingly (just changing color should work to an extent)
     * 
     * */
    //public enum WallType { none, wall, door};  

    public class Tile
    {
        public TileType tType { get; set; } //type of file
        //do the fall different walls exist
       // public WallType northWall { get; set; }
        //public WallType southWall { get; set; }
        //public WallType eastWall { get; set; }
        //public WallType westWall { get; set; }

        public int additionalInformation { get; set; } //stores additional information varying based on tile type
                                                       //Currently only used for keys and locked doors to store the number of the key that is there/opens the door

        public int RoomNumber { get; set; } //what room does this tile belong to. Currently -1 is an acceptable value used for stuff like tiles in corridors that aren't part of any actual room
                                            //and tiles that are not part of the actual level (like stuff in corners there aren't any rooms in)
        
        public Tile()
        {
            tType = TileType.undefined;
            //northWall = WallType.none;
            //southWall = WallType.none;
            //westWall = WallType.none;
            //eastWall = WallType.none;
            RoomNumber = -1; //start the tile off in no room
        }

    }
}
