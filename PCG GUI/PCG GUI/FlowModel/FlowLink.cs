using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG_GUI.FlowModel
{
    public enum LinkType 
    { 
        direct, //connects two rooms directly (they have to be adjacent)
        soft,   //hallway that connects two or more rooms. May connect to additional rooms that are not part of the flow graph
        hard,  //hallway that only connects to the rooms listed as part of the link
    };


    class FlowLink
    {
       public List<int> roomsConnected; //array indexs of all the rooms the link connects
       public LinkType type;

       public FlowLink(LinkType typeOfLink)
       {
           type = typeOfLink;
           roomsConnected = new List<int>();
       }

        public void addRoomToLink(int room)
        {
            roomsConnected.Add(room);
        }

        public void writeLink(System.IO.StreamWriter file, List<FlowRoom> allRooms)
        {
            if (type.Equals(LinkType.direct))
            {
                writeDirectLink(file, allRooms);
            }

            else if (type.Equals(LinkType.soft))
            {
                writeSoftLink(file, allRooms);
            }

            else
            {
                writeHardLink(file, allRooms);
            }
        }

        public void writeDirectLink(System.IO.StreamWriter file, List<FlowRoom> allRooms)
        {
            if(roomsConnected.Count == 2) //a direct link can only connect 2 rooms
            {
                //add constraint there must be a connection between the two rooms
                file.WriteLine(":- not connectedRooms(" + allRooms[roomsConnected[0]].roomNumber + "," + allRooms[roomsConnected[1]].roomNumber + ").");
            }
        }

        public void writeSoftLink(System.IO.StreamWriter file, List<FlowRoom> allRooms)
        {
            if (roomsConnected.Count == 2) //a soft link should support more then 2 rooms but keep it at 2 for now
            {
                //for the moment forbid direct connections
                file.WriteLine(":- connectedRooms(" + allRooms[roomsConnected[0]].roomNumber + "," + allRooms[roomsConnected[1]].roomNumber + ").");
                //things are ok if the two rooms are connected by a single other room
                file.WriteLine("connectionOk(" + allRooms[roomsConnected[0]].roomNumber + "," + allRooms[roomsConnected[1]].roomNumber + ", ID) :- connectedRooms(" + allRooms[roomsConnected[0]].roomNumber + ", ID), connectedRooms(ID," + allRooms[roomsConnected[1]].roomNumber + ").");
                //add a constraint that such a connection must occur
                file.WriteLine(":- not connectionOk(" + allRooms[roomsConnected[0]].roomNumber + "," + allRooms[roomsConnected[1]].roomNumber + ", _).");
                //for now that additional "room" must be a hallway not a room
                file.WriteLine(":- connectionOk(" + allRooms[roomsConnected[0]].roomNumber + "," + allRooms[roomsConnected[1]].roomNumber + ", ID), roomID(XUL,YUL,ID), rectangle(XUL,YUL,_,_,T), T == room.");

            }
        }

        public void writeHardLink(System.IO.StreamWriter file, List<FlowRoom> allRooms)
        {
            writeSoftLink(file, allRooms); //a hard link has the same basic setup as a soft link just with additional constraints so reuse code for initial setup
            //the room of a hard link may only have 3 connections (to both ends of the link and to itself)
            file.WriteLine(":- connectionOk(" + allRooms[roomsConnected[0]].roomNumber + "," + allRooms[roomsConnected[1]].roomNumber + ", ID), 4 {connectedRooms(ID,ID2) : rectRange(ID2) }.");
        }
    }
}
