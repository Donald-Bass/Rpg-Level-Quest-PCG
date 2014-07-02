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
            //simple loop
            int i = addRoom();
            allRooms[i].soft = false;
            i = addRoom();
            allRooms[i].soft = true;
            /*
            i = addRoom();
            allRooms[i].soft = false;
            i = addRoom();
            allRooms[i].soft = false;
            
            i = addHardLink();
            addRoomToLink(i, 0);
            addRoomToLink(i, 1);
            
            i = addHardLink();
            addRoomToLink(i, 1);
            addRoomToLink(i, 2);

            i = addDirectLink();
            addRoomToLink(i, 2);
            addRoomToLink(i, 3);
            
            i = addHardLink();
            addRoomToLink(i, 3);
            addRoomToLink(i, 0);
            
            lastRoomNum = 3;
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

            /*
            i = addRoom();
            allRooms[i].soft = false;
            i = addRoom();
            allRooms[i].soft = false;
            */
            
            i = addHardLink();
            addRoomToLink(i, 0);
            addRoomToLink(i, 1);
            
            i = addHardLink();
            addRoomToLink(i, 1);
            addRoomToLink(i, 2);
            
            i = addHardLink();
            addRoomToLink(i, 2);
            addRoomToLink(i, 3);
            
            
            i = addHardLink();
            addRoomToLink(i, 3);
            addRoomToLink(i, 4);
            
            i = addHardLink();
            addRoomToLink(i, 4);
            addRoomToLink(i, 5);

            /*
             i = addHardLink();
             addRoomToLink(i, 5);
             addRoomToLink(i, 6);
            
            
             i = addHardLink();
             addRoomToLink(i, 6);
             addRoomToLink(i, 7);
             */

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
                }

                if (l.type == LinkType.soft)
                {
                    soft = true;
                }
            }

            foreach (int i in reachableWithoutRoomsToSetup)
            {
                string forbiddenVariables = "";
                for(int j = 1; j <= i; j++)
                {
                    forbiddenVariables += ",FB" + j;
                }

                string Setup = "reachableWithoutRooms(ID2" + forbiddenVariables + " ) :- reachableWithoutRooms(ID1 " + forbiddenVariables + "), connectedRooms(ID1,ID2)";
                
                for(int j = 1; j <= i; j++)
                {
                    Setup += ", ID1 != FB" + j;
                }

                Setup += ".";
                //Setup += "finalRoom(IDF), ID1 != IDF.

                file.WriteLine(Setup);
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
            file.WriteLine("#const maxAreaSize=" + areaSize.ToString() + ".");
            file.WriteLine("#const minAreaSize=" + areaSize.ToString() + ".");
            file.WriteLine("#const maxRects=" + (maxRoomsNeeded + maxCorNeeded).ToString() + ".");
            file.WriteLine("#const maxLength=" + maxRoomLength.ToString() + ".");
            file.WriteLine("#const minLength=" + minRoomLength.ToString() + ".");
            file.WriteLine("#const minRooms=" + minRoomsNeeded.ToString() + ".");
            file.WriteLine("#const maxRooms=" + maxRoomsNeeded.ToString() + ".");
            file.WriteLine("#const minCor=" + 0 + ".");
            if (soft)
            {
                file.WriteLine("#const maxCor=" + 6 + ".");
            }

            else
            {
                file.WriteLine("#const maxCor=" + 0 + ".");
            }

            //anyways we can get a considerable speed boost by preplacing a single room.
            //we will generate a completely random room for the momement (This will change a bit with testing), under the theory
            //that if the room is randomly generarated every time Clingo is run there should be no appreciable difference in the range of possible results that could be produced
            //ADDENDUM: this will cause problems on really cramped levels. 
            //TODO: See why rooms sized 3x3 seem to break stuff on small? maps

            Random rand = new Random();
            /*
            //determine the width and length of the room
            int length = rand.Next(minRoomLength, maxRoomLength + 1);
            int height = rand.Next(minRoomLength, maxRoomLength + 1);

            //determine where the upper left corner is, ignoring possibilites that would push the bottom right corner of the room out of bounds
            int XUL = rand.Next(0, (areaSize - length + 1));
            int YUL = rand.Next(0, (areaSize - height + 1));

            //TEMP
            length = 4;
            height = 4;
            XUL = 0;
            YUL = 0;

            file.WriteLine("room(" + XUL.ToString() + "," + YUL.ToString() + "," + length.ToString() + "," + height.ToString() + ").");
            file.WriteLine("roomID(" + XUL.ToString() + "," + YUL.ToString() + ",1).");*/

            //quick code to put the starting and ending rooms on opposite sides of the map. This should be replaced soonish with code that adds some randomness where on each edge the two rooms are
            //(provided it doesn't impact performance too much)

            //determine the width and length of the room
            int length = rand.Next(minRoomLength, maxRoomLength + 1);
            int height = rand.Next(minRoomLength, maxRoomLength + 1);

            length = 4;
            height = 4;

            //determine where the upper left corner is
            int XUL = 0; //for now the starting room is always on the left
            int YUL = (areaSize / 2) - (int)(height / 2) - 1; //put the room roughly half way down. (remember the top row is row 0, not 1)

            file.WriteLine("room(" + XUL.ToString() + "," + YUL.ToString() + "," + length.ToString() + "," + height.ToString() + ").");
            file.WriteLine("roomID(" + XUL.ToString() + "," + YUL.ToString() + ",1).");

            //repeat for the final room
            if(lastRoomNum == -1)
            {
                //if no last room is defined say the last room added is the last room
                lastRoomNum = allRooms[allRooms.Count - 1].roomNumber;
            }

            //determine the width and length of the room
            length = rand.Next(minRoomLength, maxRoomLength + 1);
            height = rand.Next(minRoomLength, maxRoomLength + 1);

            length = 4;
            height = 4;

            //determine where the upper left corner is
            XUL = areaSize - length; //for now the starting room is always on the left
            YUL = (areaSize / 2) - (int)(height / 2) - 1; //put the room roughly half way down. (remember the top row is row 0, not 1)

            file.WriteLine("room(" + XUL.ToString() + "," + YUL.ToString() + "," + length.ToString() + "," + height.ToString() + ").");
            file.WriteLine("roomID(" + XUL.ToString() + "," + YUL.ToString() + "," + lastRoomNum + ").");
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
