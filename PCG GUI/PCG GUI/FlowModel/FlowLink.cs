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

        public int writeLink(System.IO.StreamWriter file, List<FlowRoom> allRooms, List<FlowLink> allLinks)
        {
            if (type.Equals(LinkType.direct))
            {
                writeDirectLink(file, allRooms);
                return -1;
            }

            else
            {
                List<int> shortestPath = getPathCloserRoom(0, roomsConnected[0], roomsConnected[1], allRooms, allLinks);

                int closerRoom = shortestPath.Last(); //which room is closer (since the path leads to this room the last node in the path is the closer room)
                int fartherRoom; //which room is farther from the start

                //use the closer room to determine what the farther room is
                if (closerRoom == roomsConnected[0])
                {
                    fartherRoom = roomsConnected[1];
                }

                else
                {
                    fartherRoom = roomsConnected[0];
                }

                //System.Console.WriteLine(closerRoom + " " + fartherRoom);

                List<int> forbiddenRooms = findSufficentRoomsToBlockRoom(0, shortestPath, fartherRoom, allRooms, allLinks);

                //System.Console.WriteLine("Done. Forbid");


                if (type.Equals(LinkType.soft))
                {
                    return writeSoftLink(file, shortestPath, closerRoom, fartherRoom, forbiddenRooms, allRooms, allLinks);
                }

                else
                {
                    return writeHardLink(file, shortestPath, closerRoom, fartherRoom, forbiddenRooms, allRooms, allLinks);
                }

            }
        }

        public void writeDirectLink(System.IO.StreamWriter file, List<FlowRoom> allRooms)
        {
            if (roomsConnected.Count == 2) //a direct link can only connect 2 rooms
            {
                //add constraint there must be a connection between the two rooms
                file.WriteLine("edge(" + allRooms[roomsConnected[0]].roomNumber + "," + allRooms[roomsConnected[1]].roomNumber + ")");

                //file.WriteLine(":- not connectedRooms(" + allRooms[roomsConnected[0]].roomNumber + "," + allRooms[roomsConnected[1]].roomNumber + ").");
                //file.WriteLine("link(" + allRooms[roomsConnected[0]].roomNumber + "," + allRooms[roomsConnected[1]].roomNumber + ").");
            }
        }

        public int writeSoftLink(System.IO.StreamWriter file, List<int> shortestPath, int closerRoom, int fartherRoom, List<int> forbiddenRooms, List<FlowRoom> allRooms, List<FlowLink> allLinks)
        {
            if (roomsConnected.Count == 2) //a soft link should support more then 2 rooms but keep it at 2 for now
            {
                //file.WriteLine("edge(" + allRooms[closerRoom].roomNumber + "," + allRooms[fartherRoom].roomNumber + ").");

                //Ok basic rundown of the issue. We don't want to define specifically what needs to be between the two rooms just give a general definition of what needs to be connecting them
                //To do so we need to know a set of nodes of rooms such that if you don't pass through any of said rooms, the only way to get to the farthest room is after going through the closer room.
                //once those rooms are know we can write constraints that specifically effect only the intended link

                //To start off with we find which room is closer to the start and the shortest path to that room

                for (int i = 0; i < forbiddenRooms.Count; i++)
                {
                   System.Console.WriteLine(forbiddenRooms[i]);
                }

                //create a string with all the forbidden room numbers
                string forbiddenRoomsString = "";

                //add the forbidden rooms
                foreach (int f in forbiddenRooms)
                {
                    forbiddenRoomsString += "," + allRooms[f].roomNumber;
                }


                //we need to setupu the reachableWithoutRooms predicate for the specific combinations of rooms we need

                //we always care about what rooms are reachable without going through the farther room
                string reachableWithoutRoomsSetup1 = "reachableWithoutRooms(START," + allRooms[fartherRoom].roomNumber + ") :- levelStartRoom(START).";

                //we also care about the set of rooms reachable without going through the closer room, and the set of room sufficent to block all other paths to the farther room
                string reachableWithoutRoomsSetup2 = "reachableWithoutRooms(START," + allRooms[closerRoom].roomNumber + forbiddenRoomsString + ") :- levelStartRoom(START).";
                

                //assert that there must be a way to reach the closer room without going through the farther room
                string closeBeforeFar = "xBeforeY(" + allRooms[closerRoom].roomNumber + "," + allRooms[fartherRoom].roomNumber + ").";

                //the farther room must not be reachable without going through the closer room or one of the other determined routes.
                string farAfter = ":- reachableWithoutRooms(" + allRooms[fartherRoom].roomNumber + "," + allRooms[closerRoom].roomNumber + forbiddenRoomsString + ").";
                                
                file.WriteLine(reachableWithoutRoomsSetup1);
                file.WriteLine(reachableWithoutRoomsSetup2);
                file.WriteLine(closeBeforeFar);
                file.WriteLine(farAfter);

                if (forbiddenRooms.Count > 0) //if there are forbidden rooms we have more work to do
                {
                    //we want to know which rooms are reachable without going through the forbidden rooms
                    string reachableWithoutRoomsSetupForbid1 = "reachableWithoutRooms(START" + forbiddenRoomsString + ") :- levelStartRoom(START).";

                    //and which rooms are reachable without going through the closer or farther rooms
                    string reachableWithoutRoomsSetupForbid2 = "reachableWithoutRooms(START," + allRooms[closerRoom].roomNumber + "," + allRooms[fartherRoom].roomNumber + ") :- levelStartRoom(START).";
                    
                    //or without going through the farther or forbidden rooms
                    string reachableWithoutRoomsSetupForbid3 = "reachableWithoutRooms(START," + allRooms[fartherRoom].roomNumber + forbiddenRoomsString + ") :- levelStartRoom(START).";

                    //the farther room must not be reachable without going through the closer room or one of the other determined routes but should be reachable going through the closer room and none of those other routes
                    string farBeforeForbid = ":- not reachableWithoutRooms(" + allRooms[fartherRoom].roomNumber + forbiddenRoomsString + ").";

                    file.WriteLine(reachableWithoutRoomsSetupForbid1);
                    file.WriteLine(reachableWithoutRoomsSetupForbid2);
                    file.WriteLine(reachableWithoutRoomsSetupForbid3);


                    file.WriteLine(farBeforeForbid);

                    //to prevent different branches from crossing over there can be no room, that you have to either pass through the closer room or the forbidden rooms to get to, but not through the farther room
                    //and you can reach while not crossing the closer room or not crossing the forbidden rooms
                    string noCrossOver = ":- room(ID), reachableWithoutRooms(ID," + allRooms[fartherRoom].roomNumber + "), not reachableWithoutRooms(ID," + allRooms[closerRoom].roomNumber + forbiddenRoomsString + "), reachableWithoutRooms(ID, " + allRooms[closerRoom].roomNumber + "," + allRooms[fartherRoom].roomNumber + "), reachableWithoutRooms(ID," + allRooms[fartherRoom].roomNumber + forbiddenRoomsString + "), ID !="  + allRooms[fartherRoom].roomNumber + ".";
                    file.WriteLine(noCrossOver); 
   
                     //+ allRooms[fartherRoom].roomNumber + forbiddenRoomsString + ").";

                }

                return forbiddenRooms.Count + 1;
            }

            return -1;
        }

        public int writeHardLink(System.IO.StreamWriter file, List<int> shortestPath, int closerRoom, int fartherRoom, List<int> forbiddenRooms, List<FlowRoom> allRooms, List<FlowLink> allLinks)
        {
            int forbidCount = writeSoftLink(file, shortestPath, closerRoom, fartherRoom, forbiddenRooms, allRooms, allLinks); //a hard link has the same basic setup as a soft link just with additional constraints so reuse code for initial setup

            //directly create an edge between the rooms
            file.WriteLine("edge(" + allRooms[closerRoom].roomNumber + "," + allRooms[fartherRoom].roomNumber + ").");

            //file.WriteLine(typeOfRoomsConstraint);

            return forbidCount;
        }

        //given two rooms returns the shortest path to the room that is closer (in terms of minimum number of links needed to travel to get to said room) 
        //to the starting room, or -null if neither room is connected to the start. If both rooms are equal distance the room the path leads to that is returned is arbitary
        private List<int> getPathCloserRoom(int startRoom, int room1, int room2, List<FlowRoom> allRooms, List<FlowLink> allLinks)
        {
            int closerRoom = -1; //which room is closer to the start. If neither room is connected the -1 will not be changed and will be returned to indicate the error
            bool[] visited = new bool[allRooms.Count];
            int[] previousRoom = new int[allRooms.Count];
            Queue<int> roomsToVisit = new Queue<int>();


            int curRoom; //the current room we are looking at
            int connectedRoom; //the room on the other side of a specific link from the curRoom;

            for (int i = 0; i < visited.Length; i++)
            {
                visited[i] = false;
                previousRoom[i] = -1;
            }

            //run a breadth first search from the start (Breadth first will find the shortest path to a node)
            //this could be optimized more but the graphs should be small enough that this is not necessary.
            roomsToVisit.Enqueue(startRoom);
            visited[startRoom] = true;

            while (roomsToVisit.Count > 0 && closerRoom == -1)
            {

                //get the next room
                curRoom = roomsToVisit.Dequeue();
                //System.Console.WriteLine(curRoom);

                //if the current room
                if (curRoom == room1)
                {
                    closerRoom = room1;
                }

                else if (curRoom == room2)
                {
                    closerRoom = room2;
                }

                else
                {
                    foreach (int l in allRooms[curRoom].allLinks) //for each link
                    {
                        //determine the room on the other side of the link

                        //FIX THIS IF I EVER ALLOW A LINK CONNECTING 3 OR MORE ROOMS
                        if (allLinks[l].roomsConnected[0] == curRoom) //check if the current room is the first half of the link
                        {
                            connectedRoom = allLinks[l].roomsConnected[1];
                        }

                        else //otherwise it must be the second
                        {
                            connectedRoom = allLinks[l].roomsConnected[0];
                        }

                        //check if the connected room has been visited and if no queue it
                        if (visited[connectedRoom] == false)
                        {
                            visited[connectedRoom] = true;
                            previousRoom[connectedRoom] = curRoom;
                            roomsToVisit.Enqueue(connectedRoom);
                        }
                    }
                }
            }

            if (closerRoom == -1)
            {
                return null;
            }

            else
            {

                List<int> shortestPath = new List<int>();

                //follow the chain of previous rooms back to the start
                curRoom = closerRoom;
                while (curRoom != -1)
                {
                    shortestPath.Add(curRoom);
                    curRoom = previousRoom[curRoom];
                }

                shortestPath.Reverse();

                return shortestPath;
            }

        }


        //find a set of rooms sufficent that the only paths to the fartherRoom, that don't pass through said rooms go through the closerRoom
        private List<int> findSufficentRoomsToBlockRoom(int startRoom, List<int> shortestPath, int fartherRoom, List<FlowRoom> allRooms, List<FlowLink> allLinks)
        {
            //alright the basic algorithm here is going to be a modified depth first search. The algorithm will keep track of what rooms it can't go through and try and find a path to the farther room
            //for the purpose of this algorithm the closerRoom will count as a room it can't go through. If a path is found, the path will be followed backwards and compared against the shortest path to
            //the closer room until a node is found that isn't part of said shorter path which will then be set as a room that can't be passed through and the process repeats. Once no more paths can be found
            //we know we have a subset that works. 

            bool[] visited = new bool[allRooms.Count];

            for (int i = 0; i < visited.Length; i++)
            {
                visited[i] = false;
            }

            List<int> forbidden = new List<int> ();
            //forbidden.Add(shortestPath.Last()); //make the first item on the forbidden room list the closer room. This can be removed later

            //find the first path to the farther room
            List<int> pathToFarther = findSufficentRoomsToBlockRoomRecurse(0, shortestPath.Last(), fartherRoom, visited, forbidden, allRooms, allLinks);

            //while paths are still being found
            while(pathToFarther != null)
            {
                int toAddForbidden = -1; //room to add to the forbidden list
                int i = 1;

                System.Console.WriteLine("Shortest Path");
                for (int j = 0; j < shortestPath.Count; j++)
                {
                    System.Console.WriteLine(shortestPath[j]);
                }
                System.Console.WriteLine("Path Found");
                for (int j = 0; j < pathToFarther.Count; j++)
                {
                   System.Console.WriteLine(pathToFarther[j]);
                }

                //while some of the path hasn't been checked, and we haven't found a room to add to the forbidden list
                while(i < pathToFarther.Count && toAddForbidden == -1)
                {

                    if(!shortestPath.Contains(pathToFarther[i])) //if the room isn't part of the shortest path
                    {
                        toAddForbidden = pathToFarther[i];
                    }

                    i++;
                }

                if(toAddForbidden == -1) //if we didn't find a room that is not part of the shortest path then we have a specific edge case, where we have a cylce of 3 in the graph. DEAL WITH THIS LATER
                {
                    System.Console.WriteLine("ADS");
                    break;
                }

                else
                {
                    forbidden.Add(toAddForbidden);
                }

                pathToFarther = findSufficentRoomsToBlockRoomRecurse(0, shortestPath.Last(), fartherRoom, visited, forbidden, allRooms, allLinks);
            }

            //forbidden.RemoveAt(0); //remove the closerRoom put at the front of the list

            return forbidden;
        }

        //recursive function to use with above function. Returns (part) of a path to the fartherRoom or null if no path was found on the specific branch. 
        private List<int> findSufficentRoomsToBlockRoomRecurse(int curRoom, int closerRoom, int fartherRoom, bool[] visited, List<int> forbiddenRooms, List<FlowRoom> allRooms, List<FlowLink> allLinks)
        {
            List<int> path = null; //the path to the farther room

            //if we have reached the farther room
            if(curRoom == fartherRoom)
            {
                path = new List<int>();
                path.Add(curRoom);

                return path;
            }

            else //go through other rooms liked
            {
                //make a copy of the room visited list so we dont't accidently affect other branched
                bool[] newVisitedList = new bool[allRooms.Count];
                visited.CopyTo(newVisitedList,0);

                //mark the current room as visited
                newVisitedList[curRoom] = true;

                int i = 0;

                while(path == null && i < allRooms[curRoom].allLinks.Count) //for each link
                {
                    int l = allRooms[curRoom].allLinks[i]; //the number of the current link

                    int connectedRoom; //the room the link connects to

                    //determine the room on the other side of the link

                    //FIX THIS IF I EVER ALLOW A LINK CONNECTING 3 OR MORE ROOMS
                    if (allLinks[l].roomsConnected[0] == curRoom) //check if the current room is the first half of the link
                    {
                        connectedRoom = allLinks[l].roomsConnected[1];
                    }

                    else //otherwise it must be the second
                    {
                        connectedRoom = allLinks[l].roomsConnected[0];
                    }

                    if(!newVisitedList[connectedRoom]) //if the room hasn't been visited
                    {
                        
                        if(!forbiddenRooms.Contains(connectedRoom)) //and the room is not forbidden
                        {
                            if (curRoom != closerRoom || connectedRoom != fartherRoom) //special edge case. The one link we want to ignore is the existing link between the closer and farther rooms
                            {
                                path = findSufficentRoomsToBlockRoomRecurse(connectedRoom, closerRoom, fartherRoom, newVisitedList, forbiddenRooms, allRooms, allLinks);
                            }
                        }
                    }

                    i++;
                }

            }

            if(path != null) //if a path was found append the current room to the path
            {
                path.Add(curRoom);
            }

            return path;
        }
    }
}
