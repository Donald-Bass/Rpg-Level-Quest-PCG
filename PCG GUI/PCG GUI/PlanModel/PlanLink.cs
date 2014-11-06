/*
 *  Defines a link between two rooms  used to specify a path between them of a certain length that should probally not be interrupted by any other rooms in the plan 
 *  (That was the original idea that I tried to implement, could be a bad idea though).
 *  This currently creates a path of X rooms between the two, but other rooms of the same level of room 2 may be reached without going through the path.
 *  This possibly could use fixing since this code is currently being used for general pacing as well but possibly not. In practice this will mean that there are parts
 *  of the next step you can reach without passing through this link, but not that you can bypass the link alltogether. It's more a question of having part of the step as a branch leading up to a key
 *  needed to access another part of said step or the next step. Either way you will eventually pass through the link
 *  
 *  This doesn't handle there being two links between a pair of steps well at all right now. 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCG_GUI.WorldModel;

namespace PCG_GUI.PlanModel
{
    public class PlanLink
    {
        public int room1 { get; set; } //room number of the first room in the link. This comes from a step 1 step earlier then room2's
        public int room2 { get; set; } //room number of the second room in the link

        private List<int> forbiddenRoomsStep1; //list of all rooms in the step of room1 other then room1. We don't want the link to go through any of these
        private List<int> forbiddenRoomsStep2; //list of all rooms in the step of room2 other then room2. We don't want the link to go through any of these

        public int linkLength { get; set; } //number of rooms that should be between room1 and room2. This does not include room1 or room2

        //constructor. Takes the room numbers of room1 and room2, the contents of the steps said rooms are part of and the length of the list
        public PlanLink(int room1, List<PlanRoom> room1Step, int room2, List<PlanRoom> room2Step, int linkLength)
        {
            forbiddenRoomsStep1 = new List<int>();
            forbiddenRoomsStep2 = new List<int>();

            this.room1 = room1;
            this.room2 = room2;
            this.linkLength = linkLength;

            //add each room to the appropiate forbiddenRooms list except for room1 and room2
            foreach (PlanRoom r in room1Step)
            {
                if(r.roomNumber != room1)
                {
                    forbiddenRoomsStep1.Insert(forbiddenRoomsStep1.Count, r.roomNumber);
                }
            }

            foreach (PlanRoom r in room2Step)
            {
                if (r.roomNumber != room2)
                {
                    forbiddenRoomsStep2.Insert(forbiddenRoomsStep2.Count, r.roomNumber);
                }
            }

        }

        //This function outputs Clingo code that contains the constraints necessary to enforce the link
        public void writeLink(System.IO.StreamWriter file)
        {

                Fact roomsBetween = new Fact();  
                //the roomsBetween atom takes the format roomsBetween(U,V,N), and defines that the shortest path between U and V passes through N rooms, and no shorter path
                //is present. To be specific the N is the number of rooms between U and V. If the graph is U-V then N would have to be 0 for this to be satisifed. U-X-Y would
                //be what a N of 1 represents

                roomsBetween.setPredicate("roomsBetween");
                roomsBetween.setNumericValue(0, room1);
                roomsBetween.setNumericValue(1, room2);
                roomsBetween.setNumericValue(2, linkLength);

                file.Write(roomsBetween.getStringRepresentation(true));

            

            if(getForbiddenRoomsCount() > 0) //if there are forbidden rooms then we need to add a constraint that there is no path between them and the room on the step the forbidden room is not part of a 
                                             //length shorter to or equal to the link. (This is really checking that said rooms are not part of the link. If they are the distances will be shorter 
                                            //then the link distance. If they are not then do to room1 and room2 being the last and first rooms of their steps to reach the forbidden rooms of the opposite
                                            //steps any path should have to go through the entire link first and the distances will be longer the link distance)
                                            
            {

                Fact notRoomsBetween = new Fact();
                //the notRoomsBetween atom takes the format notRoomsBetween(U,V,N), and defines that the shortest path between U and V passes must pass through more then N rooms
                //To be specific the N is refering to the of number of rooms between U and V not counting U or V. If the graph is U-V then N would have to be 0 for this to be satisifed. U-X-Y would
                //be what a N of 1 represents. 

                notRoomsBetween.setPredicate("notRoomsBetween");
                notRoomsBetween.setNumericValue(2, linkLength);

                //for rooms in the first room's step look at distances to room 2
                notRoomsBetween.setNumericValue(0, room2);

                foreach (int i in forbiddenRoomsStep1) 
                {
                    notRoomsBetween.setNumericValue(1,i);
                    file.Write(notRoomsBetween.getStringRepresentation(true));
                }

                //for rooms in the second room's step look at distances to room 1
                notRoomsBetween.setNumericValue(0, room1);

                foreach (int i in forbiddenRoomsStep2)
                {
                    notRoomsBetween.setNumericValue(1, i);
                    file.Write(notRoomsBetween.getStringRepresentation(true));
                }
            }
        }

        //get how many rooms have to be forbidden from being in the path between room1 and room2
        public int getForbiddenRoomsCount()
        {
            return forbiddenRoomsStep1.Count + forbiddenRoomsStep2.Count;
        }
    }
}
