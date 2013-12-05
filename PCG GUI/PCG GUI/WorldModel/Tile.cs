using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG_GUI.Facts
{
    enum TileType{floor, blocked, undefined}; //a tile can either be passable floor, impassible, or not yet defined.

    class Tile
    {
        public TileType tType { get; set; } //type of file
        //do the fall different walls exist
        public bool northWall { get; set; }
        public bool southWall { get; set; }
        public bool eastWall { get; set; }
        public bool westWall { get; set; }
        public Tile()
        {
            tType = TileType.undefined;
            northWall = false;
            southWall = false;
            westWall = false;
            eastWall = false;

        }

    }
}
