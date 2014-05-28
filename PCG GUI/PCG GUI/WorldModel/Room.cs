using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG_GUI.WorldModel
{
    public class Room
    {
        //coordinates of the upper left and lower right corners of the room
        public int XUL { get; set; }
        public int YUL { get; set; }
        public int XBR { get; set; }
        public int YBR { get; set; }

        public int roomNumber { get; set; }

        public string roomType { get; set; }

        public Room()
        {
            XUL = -1;
            YUL = -1;
            XBR = -1;
            YBR = -1;
        }
    }
}
