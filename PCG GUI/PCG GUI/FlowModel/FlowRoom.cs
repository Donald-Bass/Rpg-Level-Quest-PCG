using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG_GUI.FlowModel
{
    class FlowRoom
    {
        public int roomNumber { get; set; }
        public Boolean soft { get; set; } //is the room soft (i.e are links outside the specifieds ones allowed)

        private int numConnections;

        public FlowRoom(int num)
        {
            roomNumber = num;
            numConnections = 1; //for technical reasons a room always counts as being connected to itself
        }

        public void addLink()
        {
            numConnections++;
        }

        public void writeRoom(System.IO.StreamWriter file)
        {
            //add constraint that this room exists
            file.WriteLine(":- not roomIDExists(" + roomNumber.ToString() + ").");

            //for now force these to be room rooms.
            file.WriteLine(":- roomID(XUL,YUL," + roomNumber.ToString() + "), rectangle(XUL,YUL,_,_,T), T != room.");

            //if this isn't a soft room add a connection number constraint
            if(!soft)
            {
                file.WriteLine(":- " + (numConnections + 1) + " {connectedRooms(" + roomNumber.ToString() + ",ID2) : rectRange(ID2) }.");
            }
        }
    }
}
