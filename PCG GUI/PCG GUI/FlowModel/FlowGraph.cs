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

        private int lastRoomNum;

        //constructor
        public FlowGraph()
        {
            //until a bug in the pcg is fixed this code will break if minRoomLength = maxRoomLength
            minRoomLength = 4;
            maxRoomLength = 6;
            areaSize = 20;

            nextRoomNumber = 1;
            allRooms = new List<FlowRoom>();
            allLinks = new List<FlowLink>();

            lastRoomNum = -1;

            /*
            int i = addRoom();
            allRooms[i].soft = false;

            for (int j = 1; j <= 20; j++)
            {
                i = addRoom();
                allRooms[i].soft = false;

                i = addHardLink();
                addRoomToLink(i, j - 1);
                addRoomToLink(i, j);

                System.IO.StreamWriter file = new System.IO.StreamWriter("WorldDef" + (j + 1) + ".txt");
                
                this.writeFlow(file);

                file.Close();

            }*/

            
            /*
             *          7
             *          |
             *        4 5 6
             *        | | |
                      2-1-3             
            */

            int i = addRoom();
            allRooms[i].soft = false;
            i = addRoom();
            allRooms[i].soft = false;
            i = addRoom();
            allRooms[i].soft = false;
            i = addRoom();
            allRooms[i].soft = false;
            i = addRoom();
            allRooms[i].soft = false;
            i = addRoom();
            allRooms[i].soft = false;
            i = addRoom();
            allRooms[i].soft = false;
            
            i = addHardLink();
            addRoomToLink(i, 0);
            addRoomToLink(i, 1);

            i = addHardLink();
            addRoomToLink(i, 0);
            addRoomToLink(i, 2);

            i = addHardLink();
            addRoomToLink(i, 0);
            addRoomToLink(i, 4);

            i = addHardLink();
            addRoomToLink(i, 1);
            addRoomToLink(i, 3);

            i = addHardLink();
            addRoomToLink(i, 2);
            addRoomToLink(i, 5);

            i = addHardLink();
            addRoomToLink(i, 4);
            addRoomToLink(i, 6);


            lastRoomNum = 7;


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

                if (r.soft)
                {
                    soft = true;
                }
            }

            List<int> reachableWithoutRoomsToSetup = new List<int>();

            foreach (FlowLink l in allLinks)
            {
                if (l.type != LinkType.direct) //a non direct link requires a corridor
                {
                    minCorNeeded++;
                }

                //when I better define the starting room update this
                //List<int> closerRoom = getCloserRoom(0, l.roomsConnected[0], l.roomsConnected[1], allRooms, allLinks);

                //Console.WriteLine(l.roomsConnected[0] + " " + l.roomsConnected[1]);

                //for(int i = 0; i < closerRoom.Count; i++)
                //{
                //    Console.WriteLine(closerRoom[i]);
                //}

                int numRoomsUsed = l.writeLink(file, allRooms, allLinks);

                if(numRoomsUsed != -1 && !reachableWithoutRoomsToSetup.Contains(numRoomsUsed))
                {
                    reachableWithoutRoomsToSetup.Add(numRoomsUsed);

                    if(numRoomsUsed != 1 && !reachableWithoutRoomsToSetup.Contains(numRoomsUsed - 1))
                    {
                        reachableWithoutRoomsToSetup.Add(numRoomsUsed - 1);
                    }
                }

                if (l.type == LinkType.soft)
                {
                    soft = true;
                }
            }

            if(! reachableWithoutRoomsToSetup.Contains(1))
            {
                reachableWithoutRoomsToSetup.Add(1);
            }

            if (!reachableWithoutRoomsToSetup.Contains(2))
            {
                reachableWithoutRoomsToSetup.Add(2);
            }
            foreach (int i in reachableWithoutRoomsToSetup)
            {
                string forbiddenVariables = "";
                for(int j = 1; j <= i; j++)
                {
                    forbiddenVariables += ",FB" + j;
                }

                string SetupReachable = "reachableWithoutRooms(ID2" + forbiddenVariables + ") :- reachableWithoutRooms(ID1" + forbiddenVariables + "), edge(ID1,ID2)";

                for(int j = 1; j <= i; j++)
                {
                    SetupReachable += ", ID1 != FB" + j;
                }

                SetupReachable += ".";
                //Setup += "finalRoom(IDF), ID1 != IDF.

                file.WriteLine(SetupReachable);
            }

            //replace this when the flow graph actually knows which room is the start
            file.WriteLine("levelStartRoom(1).");
            file.WriteLine("finalRoom(" + allRooms.Count + ").");

            if (soft)
            {
                //if there are soft rooms/links allow a few extra rooms and corridors to be created
                maxRoomsNeeded = minRoomsNeeded + 3;
                maxCorNeeded = minCorNeeded * 2 + 6;
            }

            else
            {
                maxRoomsNeeded = minRoomsNeeded;
                maxCorNeeded = minCorNeeded * 2;
            }

            //write the constants
            file.WriteLine("#const minRooms=" + minRoomsNeeded.ToString() + ".");
            file.WriteLine("#const maxRooms=" + maxRoomsNeeded.ToString() + ".");


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
            if (allLinks[linkIndex].type != LinkType.direct)
            {
                allRooms[roomIndex].addLink(linkIndex);
            }
            else
            {
                allRooms[roomIndex].addDirectConnection();
            }
        }

    }

}
