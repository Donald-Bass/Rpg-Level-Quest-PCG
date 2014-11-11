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


        public int X { get; set; }
        public int Y { get; set; }


        public int roomNumber { get; set; }

        public string roomType { get; set; }

        public Room()
        {
            XUL = -1;
            YUL = -1;
            XBR = -1;
            YBR = -1;
            X = -1;
            Y = -1;
        }

        //returns an atom encoding the room
        //format is room(XUL,YUL,XBR,YBR,N,T) where t is the roomType and n is the room number
        public Fact roomAtom()
        {
            Fact atom = new Fact();

            atom.setPredicate("room");
            atom.setNumericValue(0,XUL);
            atom.setNumericValue(1,YUL);
            atom.setNumericValue(2,XBR);
            atom.setNumericValue(3,YBR);
            atom.setNumericValue(4,roomNumber);
            atom.setValue(5,roomType);

            return atom;
        }
    }
}
