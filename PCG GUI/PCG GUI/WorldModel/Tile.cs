using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG_GUI.Facts
{
    public enum TileType{floor, blocked, undefined, levelStart}; //a tile can either be passable floor, impassible, or not yet defined.
    public enum WallType { none, wall, door};

    public class Tile
    {
        public TileType tType { get; set; } //type of file
        //do the fall different walls exist
        public WallType northWall { get; set; }
        public WallType southWall { get; set; }
        public WallType eastWall { get; set; }
        public WallType westWall { get; set; }
        public Tile()
        {
            tType = TileType.undefined;
            northWall = WallType.none;
            southWall = WallType.none;
            westWall = WallType.none;
            eastWall = WallType.none;

        }

    }
}
