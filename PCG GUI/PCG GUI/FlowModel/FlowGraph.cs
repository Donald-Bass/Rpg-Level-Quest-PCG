/*Datastructure that holds the overall graph of the intended flow of the level*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG_GUI.FlowModel
{
    class FlowGraph
    {
        //list holding all rooms and links in the graph. It is important to note that each room/link has two sort of identifiers
        //a indexNumber, for the rooms index in the list, and a room number. The code in c# uses the former to allow easy lookup, but any clingo code
        //created will need to use the latter
        public List<FlowRoom> allRooms;
        public List<FlowLink> allLinks;

        public int nextRoomNumber; //what room number should the next room created have

        public int minRoomLength;
        public int maxRoomLength; //maxRoom
        public int areaSize; //size of the area (assumes area is a square for now)

        //constructor
        public FlowGraph()
        {
            minRoomLength = 3;
            maxRoomLength = 4;
            areaSize = 12;

            nextRoomNumber = 1;
            allRooms = new List<FlowRoom>();
            allLinks = new List<FlowLink>();

            int i = addRoom();
            allRooms[i].soft = false;
            i = addRoom();
            allRooms[i].soft = false;

            i = addRoom();
            allRooms[i].soft = false;
            i = addRoom();
            allRooms[i].soft = false;



            i = addHardLink();
            addRoomToLink(i,0);
            addRoomToLink(i,1);

            i = addHardLink();
            addRoomToLink(i,0);
            addRoomToLink(i,2);

            i = addHardLink();
            addRoomToLink(i,1);
            addRoomToLink(i,3);

            i = addHardLink();
            addRoomToLink(i, 2);
            addRoomToLink(i, 3);

        }

        //write a world def file
        public void writeFlow(System.IO.StreamWriter file)
        {
            //write the necessary constants. Hardcoded for the moment won't be for long
            
            int minRoomsNeeded = 0; //how many rooms are needed
            int minCorNeeded = 0; //number of corridors needed
            int maxRoomsNeeded = 0;
            int maxCorNeeded = 0;
            bool soft = false; //are there any soft rooms

            foreach (FlowRoom r in allRooms)
            {
                minRoomsNeeded++;
                r.writeRoom(file);

                if(r.soft)
                {
                    soft = true;
                }
            }

            foreach (FlowLink l in allLinks)
            {
                if (l.type != LinkType.direct) //a non direct link requires a corridor
                {
                    minCorNeeded++;
                }

                l.writeLink(file, allRooms);

                if(l.type == LinkType.soft)
                {
                    soft = true;
                }
            }

            //replace this when the flow graph actually knows which room is the start
            file.WriteLine("levelStartRoom(1).");

            if(soft)
            {
                //if there are soft rooms/links allow a few extra rooms and corridors to be created
                maxRoomsNeeded = minRoomsNeeded + 3;
                maxCorNeeded = minCorNeeded + 3;
            }

            else
            {
                maxRoomsNeeded = minRoomsNeeded;
                maxCorNeeded = minCorNeeded;
            }

            //write the constants
            file.WriteLine("#const maxAreaSize=" + areaSize.ToString() + ".");
            file.WriteLine("#const minAreaSize=" + areaSize.ToString() + ".");
            file.WriteLine("#const maxRects=" + (maxRoomsNeeded+ maxCorNeeded).ToString() + ".");
            file.WriteLine("#const maxLength=" + maxRoomLength.ToString() + ".");
            file.WriteLine("#const minLength=" + minRoomLength.ToString() + ".");
            file.WriteLine("#const minRooms=" + minRoomsNeeded.ToString() + ".");
            file.WriteLine("#const maxRooms=" + maxRoomsNeeded.ToString() + ".");
            file.WriteLine("#const minCor=" + minCorNeeded.ToString() + ".");
            file.WriteLine("#const maxCor=" + maxCorNeeded.ToString() + ".");


            //anyways we can get a considerable speed boost by preplacing a single room.
            //we will generate a completely random room for the momement (This will change a bit with testing), under the theory
            //that if the room is randomly generarated every time Clingo is run there should be no appreciable difference in the range of possible results that could be produced

            Random rand = new Random();

            //determine the width and length of the room
            int length = rand.Next(minRoomLength, maxRoomLength + 1);
            int height = rand.Next(minRoomLength, maxRoomLength + 1);

            //determine where the upper left corner is, ignoring possibilites that would push the bottom right corner of the room out of bounds
            int XUL = rand.Next(0, (areaSize - length + 1));
            int YUL = rand.Next(0, (areaSize - height + 1));

            file.WriteLine("room(" + XUL.ToString() + "," + YUL.ToString() + "," + length.ToString() + "," + height.ToString() + ").");
            file.WriteLine("roomID(" + XUL.ToString() + "," + YUL.ToString() + "1).");
        }

        private int addDirectLink()
        {
            return addLink(LinkType.direct);
        }

        private int addSoftLink()
        {
            return addLink(LinkType.soft);
        }
        private int addHardLink()
        {
            return addLink(LinkType.hard);
        }


        private int addLink(LinkType type)
        {
            //create the Link
            FlowLink newLink = new FlowLink(type);

            allLinks.Add(newLink); //add the Link to the list

            return allLinks.Count - 1; //return the index of the new Link so we can work with it
        }

        private int addRoom()
        {
            //create the room
            FlowRoom newRoom = new FlowRoom(nextRoomNumber);
            nextRoomNumber++;

            allRooms.Add(newRoom); //add the room to the list

            return allRooms.Count - 1; //return the index of the new room so we can work with it
        }

        private void addRoomToLink(int linkIndex, int roomIndex)
        {
            allLinks[linkIndex].addRoomToLink(roomIndex);
            allRooms[roomIndex].addLink();
        }
    }
}
