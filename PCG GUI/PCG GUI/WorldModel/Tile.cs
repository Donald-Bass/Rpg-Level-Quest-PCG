using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG_GUI.Facts
{
    public enum TileType{floor, blocked, wall, undefined, door, key, locked, startingRoom}; //a tile can either be passable floor, impassible, or not yet defined.
    public enum WallType { none, wall, door};

    public class Tile
    {
        public TileType tType { get; set; } //type of file
        //do the fall different walls exist
        public WallType northWall { get; set; }
        public WallType southWall { get; set; }
        public WallType eastWall { get; set; }
        public WallType westWall { get; set; }

        public int additionalInformation { get; set; } //stores additional information varying based on tile type
                                                       //Currently only used for keys and locked doors to store the number of the key that is there/opens the door

        public int RoomNumber { get; set; } //what room does this belong to
        
        public Tile()
        {
            tType = TileType.undefined;
            northWall = WallType.none;
            southWall = WallType.none;
            westWall = WallType.none;
            eastWall = WallType.none;
            RoomNumber = -1;

        }

    }
}
