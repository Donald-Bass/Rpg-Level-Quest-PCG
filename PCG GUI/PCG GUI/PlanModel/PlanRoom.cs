/* A PlanRoom object is used to represent a single room in the partially ordered plan used to describe the level. This currently doesn't really do anything. It stores the type
 * of the room that will be important when vizulization comes along, and may play into other constraints at a future date, but right now the only important thing this class does is store the roomnumber
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG_GUI.PlanModel
{
    public enum roomTypes {TreasureRoom, BossFight, Gauntlet, Entrance};

    public class PlanRoom
    {

        public int roomNumber { get; set; }

        public bool optionalRoom { get; set; }

        public roomTypes type { get; set; }

        public PlanRoom()
        {
            roomNumber = -1;
            type = roomTypes.TreasureRoom;
        }

        public PlanRoom(int roomNumber, roomTypes type)
        {
            this.roomNumber = roomNumber;
            this.type = type;
        }

        //print an atom containing type of the room
        public void writeRoom(System.IO.StreamWriter file)
        {
            Fact typeOfRoom = new Fact();
            typeOfRoom.setPredicate("typeOfRoom");
            typeOfRoom.setNumericValue(0, roomNumber);

            switch(type)
            {
                case roomTypes.TreasureRoom:
                    typeOfRoom.setValue(1, "treasure");
                    break;
                case roomTypes.BossFight:
                    typeOfRoom.setValue(1, "boss");
                    break;
                case roomTypes.Gauntlet:
                    typeOfRoom.setValue(1, "gauntlet");
                    break;
                case roomTypes.Entrance:
                    typeOfRoom.setValue(1, "entrance");
                    break;
            }

            file.WriteLine(typeOfRoom.getStringRepresentation(true));
        }
    }
}
