/* A PlanRoom object is used to represent a single room in the partially ordered plan used to describe the level
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG_GUI.PlanModel
{
    public enum roomTypes {TreasureRoom, BossFight, Gauntlet};

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

        public void writeRoom(System.IO.StreamWriter file)
        {

        }
    }
}
