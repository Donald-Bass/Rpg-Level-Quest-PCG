/*The room class is used to store the details of individual rooms in the level. Right now this mostly means the boundaries of the room, the type of the room, and the room number
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG_GUI.WorldModel
{
    public class Room
    {
        //coordinates of the upper left and bottom right corners of the room with the room being defined as the rectangle created by these two corners. While rooms may not actually be perfectly rectangular 
        //in shape, this rectanglular area between these 2 corners will always contain the entire room, and just that room, it won't have parts of any other rooms or corridors, 
        //just possibly some blocked tiles that can't really be said to be part of any room
        public int XUL { get; set; } //x coordinate of the upper left corner
        public int YUL { get; set; } //y coordinate of the upper left corner
        public int XBR { get; set; } //x coordinate of the bottom right corner
        public int YBR { get; set; } //y coordinate of the bottom right corner

        public int roomNumber { get; set; } //number of the room. This is essentially used as a unique identifier with each room having its own number

        public string roomType { get; set; } //type of the room. Probally should be replaced as an enum instead of a string
                                             //current roomTypes are "boss", "gauntlet", "treasure", "entrance", and "generic" that don't have a great deal of meaning yet
                                             //this value is set by the addRoom function of the levelClass, which is turn gets the value to set it to from the calls to said function in the levelBuilder class

        public Room()
        {
            XUL = -1;
            YUL = -1;
            XBR = -1;
            YBR = -1;
        }

        //returns an atom encoding the room
        //format is room(XUL,YUL,XBR,YBR,N,T) where t is the roomType and n is the room number
        public Atom roomAtom()
        {
            Atom atom = new Atom();

            atom.setAtomName("room");
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
