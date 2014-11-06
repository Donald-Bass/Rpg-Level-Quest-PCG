/*This class and the FlowLink and FlowRoom class are pretty much entirely obsolete. There were created for an earlier iteration of the project. These classes were used
 * to construct a graph that describes the majority of the level, and then generate constraints that would produce levels in Clingo that use said graph as the basis.
 * 
 * These files were replaced with the PlanModel set of classes, which implemented a more reasonable model for describing a level.
 * 
 * Some of this code may still be intresting as it produces some possibly useful sets of constraints. To summarise briefly when this model was created, I had the idea of rooms and corridors being
 * soft or hard. A hard room would only have the edges specified in the FlowGraph, while a soft room would give Clingo premission to add new edges leading possibly to new rooms.
 * Similarly a hard corridor(which I have been refering to as links) was a corridor(or a series of rooms and corridors) that went directly between the two rooms it linked with no sidepaths, 
 * while a soft corridor allowed for the PCG to add side passages and rooms to the corridor.
 * 
 * What is probally of most intrest in this is the PCG code used for links. There is code for allowing for a connection between two rooms where there is any number of rooms/edges in between, 
 * but those edges and rooms don't connect to any other part of the graph. It could come in handy if you want to allow a user to specify a chain of rooms that have to be connected in a certain order
 * but wants that set of rooms as a whole to be part of the same step as some other rooms, 
 */

/*
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

        //constructor
        public FlowGraph()
        {

            nextRoomNumber = 1; //the first room should be room 1
            allRooms = new List<FlowRoom>();
            allLinks = new List<FlowLink>();

            //This class was abandoned before it could ever get hooked into the ui, so all testing was done with graphs hardcoded into the class as seen before

            //This is a rough diagram of the graph being hardcoded in below
            //  
            //          7
            //          |
            //        4 5 6
            //        | | |
            //        2-1-3             
            

            //General procedure for creating a graph was you add the rooms you need, getting the index of the room back each time, then you edited those rooms as necessary
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
            
            //Continuing from the previous comment you would then add links as necessary, specifying whether to use soft or hard links, get an index back, and then add the two endpoints
            //of the Link(Edge) to the link you created
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

        }

        //This function wrote the necessary set of parameters needed to generate the specified graph to a file in a format clingo could use as input
        public void writeFlow(System.IO.StreamWriter file)
        {

            //keep track of data needed to decide what general rules to output
            int minRoomsNeeded = 0; //minimum number of rooms  needed
            int maxRoomsNeeded = 0; //maximum number of rooms allowed
            bool soft = false; //are there any soft rooms/links

            //For each room we print the set of constraints corrosponding to that room, and check if the room is soft
            foreach (FlowRoom r in allRooms)
            {
                minRoomsNeeded++;
                r.writeRoom(file);

                if (r.soft)
                {
                    soft = true;
                }
            }

            //The link code used some rules that needed a variable number of atoms depending on how complex the graph was. Since there is no easy way to implement that in Clingo
            //The solution I reached was to keep track of the different numbers of atoms used, and generate rules on the fly for the numbers used
            List<int> reachableWithoutRoomsToSetup = new List<int>(); 

            foreach (FlowLink l in allLinks)
            {
                int numRoomsUsed = l.writeLink(file, allRooms, allLinks); //write the code for the link and store how many atoms it used for some specific rules (as discussed above)

                if(numRoomsUsed != -1 && !reachableWithoutRoomsToSetup.Contains(numRoomsUsed)) //if the number of atoms used is different from any seen, store that number
                {
                    reachableWithoutRoomsToSetup.Add(numRoomsUsed);

                    if(numRoomsUsed != 1 && !reachableWithoutRoomsToSetup.Contains(numRoomsUsed - 1)) //Making this even more complicated the rules we needed to generate were for the number of atoms
                                                                                                      //used and the number of atoms used - 1, so we need to check if that value was seen yet as well
                    {
                        reachableWithoutRoomsToSetup.Add(numRoomsUsed - 1);
                    }
                }

                if (l.type == LinkType.soft)
                {
                    soft = true;
                }
            }

            //Finally these vauge rules I were discussing are always needed in the 1 and 2 atoms version
            if(! reachableWithoutRoomsToSetup.Contains(1))
            {
                reachableWithoutRoomsToSetup.Add(1);
            }

            if (!reachableWithoutRoomsToSetup.Contains(2))
            {
                reachableWithoutRoomsToSetup.Add(2);
            }

            //Here is where the rules are finally generated. The rule being produced is reachableWithoutRooms(ID,FB1,....FBN)
            //It means that the room ID can be reached from the first room of the level without ever passing through any of the rooms R1 through RN.
            //
            foreach (int i in reachableWithoutRoomsToSetup)
            {

                //give each forbidden room a variable in the form of FBx and build a string containing variables for all forbidden rules
                string forbiddenVariables = "";
                for(int j = 1; j <= i; j++)
                {
                    forbiddenVariables += ",FB" + j;
                }

                //produce the main body of the rule. This says that a room can be reached without passing through the forbidden rooms, if another room can be reached
                //and that room has an edge connecting it to the former room
                string SetupReachable = "reachableWithoutRooms(ID2" + forbiddenVariables + ") :- reachableWithoutRooms(ID1" + forbiddenVariables + "), edge(ID1,ID2)";

                //finally build the last part of the rule, saying that the above only holds true if the latter room is not one of the forbidden rooms
                for(int j = 1; j <= i; j++)
                {
                    SetupReachable += ", ID1 != FB" + j;
                }

                SetupReachable += "."; //all clingo rules end with a period.

                file.WriteLine(SetupReachable);
            }

            file.WriteLine("levelStartRoom(1)."); //The pcg needs a room to be marked as the first room so we know what direction the player is moving. We abitarily set it to 1 here
            file.WriteLine("finalRoom(" + allRooms.Count + ")."); //The pcg originally needed to know what the last room in the level was. There were reasons but they were stupid reasons and this 
                                                                  //information is no longer used or required

            //finally compute how many rooms and corridors are needed. If every room and link is hard there is no room to add new rooms so the max rooms allowed is the same as the min rooms. Otherwise\
            //allow for a few extra rooms
            if (soft)
            {
                //if there are soft rooms/links allow a few extra rooms and corridors to be created
                maxRoomsNeeded = minRoomsNeeded + 3;
            }

            else
            {
                maxRoomsNeeded = minRoomsNeeded;
            }

            //write the constants
            file.WriteLine("#const minRooms=" + minRoomsNeeded.ToString() + ".");
            file.WriteLine("#const maxRooms=" + maxRoomsNeeded.ToString() + ".");


        }

        private int addSoftLink()
        {
            return addLink(LinkType.soft);
        }
        private int addHardLink()
        {
            return addLink(LinkType.hard);
        }

        //creates an empty link (no endpoints set) as a certain type add it to the list of all links,
        //and return the index of the newly added link so it can be modified
        private int addLink(LinkType type)
        {
            //create the Link
            FlowLink newLink = new FlowLink(type);

            allLinks.Add(newLink); //add the Link to the list

            return allLinks.Count - 1; //return the index of the new Link so we can work with it
        }

        //creates an empty room (no type set) as a certain type add it to the list of all rooms,
        //and return the index of the newly added room so it can be modified
        private int addRoom()
        {
            //create the room
            FlowRoom newRoom = new FlowRoom(nextRoomNumber); 
            nextRoomNumber++;

            allRooms.Add(newRoom); //add the room to the list

            return allRooms.Count - 1; //return the index of the new room so we can work with it
        }

        //adds a room to a link (ie set one of the endpoints of a edge to that room). Takes two inputs, the index of the link to add the room too, and the index of the room to add
        private void addRoomToLink(int linkIndex, int roomIndex)
        {
            allRooms[roomIndex].addLink(linkIndex);
        }

    }

}
*/