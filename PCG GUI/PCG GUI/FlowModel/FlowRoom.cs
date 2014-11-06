/*  This class we used to represent a room in the graph/eventual dungon level. It doesn't actually do all that mutch
 * 
 */

/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG_GUI.FlowModel
{
    class FlowRoom
    {
        public int roomNumber { get; set; } //what is the number of rhe room
        public Boolean soft { get; set; } //is the room soft (i.e are links outside the specifieds ones allowed)

        public List<int> allLinks { get; set; } //List of indecies of links the room is part of

        private int numConnections; //how many rooms the room is connected to. This was different from the number of links long ago, know doesn't mean much

        public FlowRoom(int num)
        {
            allLinks = new List<int>();
            roomNumber = num;
            numConnections = 1; //for technical reasons a room always counts as being connected to itself
        }

        public void addDirectConnection()
        {
            numConnections++;
        }

        public void addLink(int linkNum)
        {
            numConnections++;
            allLinks.Add(linkNum);
        }

        public void writeRoom(System.IO.StreamWriter file)
        {
            //add constraint that this room exists
            //file.WriteLine(":- not roomIDExists(" + roomNumber.ToString() + ").");

            //for now force these to be room rooms.
            //file.WriteLine(":- roomID(XUL,YUL," + roomNumber.ToString() + "), rectangle(XUL,YUL,_,_,T), T != room.");

            //write that this is a predefined room
            //file.WriteLine("predefined(" + roomNumber.ToString() + ").");

            if (!soft) //if the room is not soft (meaning it can't have additional edges added) constrict the number of edges allowed
            {
                file.WriteLine("numberOfEdges(" + roomNumber + "," + allLinks.Count + ").");
            }

            //if this isn't a soft room add a connection number constraint
            //if(!soft)
            //{
            //    file.WriteLine(":- {connectedRooms(" + roomNumber.ToString() + ",ID2) : rectRange(ID2) } " + (numConnections - 1) +  ".");
            //   file.WriteLine(":- " + (numConnections + 1) + " {connectedRooms(" + roomNumber.ToString() + ",ID2) : rectRange(ID2) }.");
            //}
        }
    }
}
*/