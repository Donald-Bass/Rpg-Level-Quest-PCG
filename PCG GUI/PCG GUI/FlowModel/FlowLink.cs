/*  The FlowLink class was used to define a link between two rooms. A link is essentialy a edge on the graph, it is a corridor, or a series of corridors and rooms connecting the two rooms 
 *  with no connections to the rest of the graph. It is possible if the link is soft for there to be a sidepath generated that leads to a few rooms not part of the connection between the original two rooms
 *  but these rooms should be just as disconnected from the rest of the graph as those making up the connection are. 
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
    public enum LinkType
    {
        direct, //connects two rooms directly (they have to be adjacent). (Note from later on. This type of link will never get used as changes in the PCG removed the possibility of two rooms
                //being adjacent in order to simplfy things so PCG would run faster) It looks like I made a slight attempt to redefine this to there always being a direct connection but I'm not sure
                //this was ever tested
        soft,   //hallway that connects two or more rooms. May connect to additional rooms that are not part of the flow graph
        hard,  //hallway that only connects to the rooms listed as part of the link
    };


    class FlowLink
    {
        public List<int> roomsConnected; //array indexs of all the rooms the link connects. I think at one point I was planning in theory to have links that do stuff like t corridors and connect
                                         //3+ rooms but I don't think this class was ever tested let alone used with more then two rooms in any indivdidual link
        public LinkType type;           //what type of link is this

        public FlowLink(LinkType typeOfLink)
        {
            type = typeOfLink;
            roomsConnected = new List<int>();
        }

        //add a room (endpoint to the link)
        public void addRoomToLink(int room)
        {
            roomsConnected.Add(room);
        }

        //write the rules Clingo needs to enforce the link specified in this class. This requires knowing all the rooms and links in the graph
        public int writeLink(System.IO.StreamWriter file, List<FlowRoom> allRooms, List<FlowLink> allLinks)
        {
            if (type.Equals(LinkType.direct)) //if the link is direct things are simple
            {
                writeDirectLink(file, allRooms);
                return -1;
            }

            else
            {
                //The first step  to writing the link is to know which end of the link was closer to the start. This is necessary to support cases like a bottleneck where the link
                //is the only connection between two components to ensure that our rules try to look for paths that come from the proper direction.
                List<int> shortestPath = getPathCloserRoom(0, roomsConnected[0], roomsConnected[1], allRooms, allLinks); //find the shortest path to one of the two endpoints and return the path

                int closerRoom = shortestPath.Last(); //determine which room is closer (since the path leads to the closer room the last node in the path is the closer room)
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

                //This here is the big one. To ensure our rules just look at the link we desire we need to find a set of rooms such that if you say the link is between u and v, 
                //if you don't pass through any of the rooms in the set the only path to v requires going through the link between u and v
                List<int> forbiddenRooms = findSufficentRoomsToBlockRoom(0, shortestPath, fartherRoom, allRooms, allLinks);

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

        //this in theory enforces there being a dirrect connection between the two edges of a link
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

        //This function writes additional rules for soft links using information computed in the writeLink function earier
        public int writeSoftLink(System.IO.StreamWriter file, List<int> shortestPath, int closerRoom, int fartherRoom, List<int> forbiddenRooms, List<FlowRoom> allRooms, List<FlowLink> allLinks)
        {
            if (roomsConnected.Count == 2) //a soft link should support more then 2 rooms but keep it at 2 for now
            {
                //create a string with all the forbidden room numbers
                string forbiddenRoomsString = "";

                //add the forbidden rooms
                foreach (int f in forbiddenRooms)
                {
                    forbiddenRoomsString += "," + allRooms[f].roomNumber;
                }

                //Now we define a set of rules using the reachableWithoutRooms atom.To recap the format is reachableWithoutRooms(ID,FB1,....FBN) It means that the room ID can 
                //be reached from the first room of the level without ever passing through any of the rooms R1 through RN. It is important to note here that the PCG doesn't compute this for
                //every combination of rooms. We have to specify that the first room is reachable for the sets of rooms we care about and then it will progate from that to determine which rooms
                //are reachable for each of those sets

                //We need to know what rooms are reachable without passing through the farther room
                string reachableWithoutRoomsSetup1 = "reachableWithoutRooms(START," + allRooms[fartherRoom].roomNumber + ") :- levelStartRoom(START).";

                //we also need to know the set of rooms reachable without going through the closer room, and the set of forbidden rooms (This should mean there is no path to the farther room)
                string reachableWithoutRoomsSetup2 = "reachableWithoutRooms(START," + allRooms[closerRoom].roomNumber + forbiddenRoomsString + ") :- levelStartRoom(START).";
                
                //assert that there must be a way to reach the closer room without going through the farther room.
                string closeBeforeFar = "xBeforeY(" + allRooms[closerRoom].roomNumber + "," + allRooms[fartherRoom].roomNumber + ").";

                //the farther room must not be reachable without going through the closer room or one of the other determined routes.
                string farAfter = ":- reachableWithoutRooms(" + allRooms[fartherRoom].roomNumber + "," + allRooms[closerRoom].roomNumber + forbiddenRoomsString + ").";
                             
                //write these rules to the output file
                file.WriteLine(reachableWithoutRoomsSetup1);
                file.WriteLine(reachableWithoutRoomsSetup2);
                file.WriteLine(closeBeforeFar);
                file.WriteLine(farAfter);

                if (forbiddenRooms.Count > 0) //if there are forbidden rooms we have more work to do
                {
                    //we need know which rooms are reachable without going through the forbidden rooms
                    string reachableWithoutRoomsSetupForbid1 = "reachableWithoutRooms(START" + forbiddenRoomsString + ") :- levelStartRoom(START).";

                    //the farther room must not be reachable without going through the closer room or one of the other determined routes (which we checked earlier) 
                    //but should be reachable going through the closer room and none of those other routes (the set of forbidden rooms)
                    string farBeforeForbid = ":- not reachableWithoutRooms(" + allRooms[fartherRoom].roomNumber + forbiddenRoomsString + ").";

                    //we need to know which rooms are reachable without going through the closer or farther rooms
                    string reachableWithoutRoomsSetupForbid2 = "reachableWithoutRooms(START," + allRooms[closerRoom].roomNumber + "," + allRooms[fartherRoom].roomNumber + ") :- levelStartRoom(START).";
                    
                    //and which rooms are reachable without going through the farther or forbidden rooms
                    string reachableWithoutRoomsSetupForbid3 = "reachableWithoutRooms(START," + allRooms[fartherRoom].roomNumber + forbiddenRoomsString + ") :- levelStartRoom(START).";


                    //to prevent different branches from crossing over there can be no room, that you have to either pass through the closer room or the forbidden rooms to get to, 
                    //but not through the farther room and you can reach while not crossing the closer room or not crossing the forbidden rooms
                    //To illustate suppose you have the following graph. 
                    //                          2--1--3
                    //                          |     |
                    //                          4     5.
                    //The situation we want to avoid is 
                    //                             1--2
                    //                             |  |
                    //                             3--6--5
                    //                                |
                    //                                4
                    //where the two links both pass through a newly added room 6.
                    string noCrossOver = ":- room(ID,_,_), reachableWithoutRooms(ID," + allRooms[fartherRoom].roomNumber + "), not reachableWithoutRooms(ID," + allRooms[closerRoom].roomNumber + forbiddenRoomsString + "), reachableWithoutRooms(ID, " + allRooms[closerRoom].roomNumber + "," + allRooms[fartherRoom].roomNumber + "), reachableWithoutRooms(ID," + allRooms[fartherRoom].roomNumber + forbiddenRoomsString + "), ID !="  + allRooms[fartherRoom].roomNumber + ".";

                    file.WriteLine(reachableWithoutRoomsSetupForbid1);
                    file.WriteLine(farBeforeForbid);
                    file.WriteLine(reachableWithoutRoomsSetupForbid2);
                    file.WriteLine(reachableWithoutRoomsSetupForbid3);
                    file.WriteLine(noCrossOver); 
   
                }

                return forbiddenRooms.Count + 1; //return the number of forbidden rooms + 1 so we know which versions of reachableWithoutRooms to generate rules for
            }

            return -1;
        }

        public int writeHardLink(System.IO.StreamWriter file, List<int> shortestPath, int closerRoom, int fartherRoom, List<int> forbiddenRooms, List<FlowRoom> allRooms, List<FlowLink> allLinks)
        {
            int forbidCount = writeSoftLink(file, shortestPath, closerRoom, fartherRoom, forbiddenRooms, allRooms, allLinks); //a hard link has the same basic setup as a soft link just with additional constraints so reuse code for initial setup

            //directly create an edge between the rooms
            //I have no idea what the hell I was trying to do here
            //If you go back far enough in github you will probally find the original rules I was using here that I replaced for some unknown reason
            file.WriteLine("basicEdge(" + allRooms[closerRoom].roomNumber + "," + allRooms[fartherRoom].roomNumber + ").");

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


        //find a set of rooms sufficent that the only paths to the fartherRoom, that don't pass through said rooms go through the closerRoom.
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
*/