/*
 *  Defines a link between two rooms  used to specify a path between them of a certain length ?not interrupted by any other rooms in the plan?
 *  Note to self. This currently creates a path of X rooms between the two, but other rooms of the same level of room 2 may be reached without going through the path.
 *  Probally as it should be though. If the user really wanted the player to visit room2 before any other room in that step, then these other rooms should have been 1 step further.
 *  
 * Ok I just tested this with two links between the first and second steps. Currently there is nothing preventing the two from sharing the same rooms in the link. 
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
        public int room1 { get; set; }
        public int room2 { get; set; }

        private List<int> forbiddenRoomsStep1; //list of rooms that the link cannot pass through from room 1's step
        private List<int> forbiddenRoomsStep2; //list of rooms that the link cannot pass through from room 2's step

        public int linkLength { get; set; }

        //constructor
        public PlanLink(int room1, List<PlanRoom> room1Step, int room2, List<PlanRoom> room2Step, int linkLength)
        {
            forbiddenRoomsStep1 = new List<int>();
            forbiddenRoomsStep2 = new List<int>();

            this.room1 = room1;
            this.room2 = room2;
            this.linkLength = linkLength;

            //add each room to 
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

            

            if(getForbiddenRoomsCount() > 0) //if there are forbidden rooms then we need to add a constraint that there is no path between them and the room on the other step of a length shorter to
                                             //or equal to the link
            {

                Fact notRoomsBetween = new Fact();
                //the notRoomsBetween atom takes the format notRoomsBetween(U,V,N), and defines that the shortest path between U and V passes must pass through more then N rooms
                //To be specific the N is refering to the of number of rooms between U and V not counting U or V. If the graph is U-V then N would have to be 0 for this to be satisifed. U-X-Y would
                //be what a N of 1 represents. 

                notRoomsBetween.setPredicate("notRoomsBetween");
                //notRoomsBetween.setNumericValue(0, room1);
                //notRoomsBetween.setNumericValue(1, room2);
                notRoomsBetween.setNumericValue(2, linkLength);

                //for rooms in the first rooms step look at distances to room 2
                notRoomsBetween.setNumericValue(0, room2);

                foreach (int i in forbiddenRoomsStep1) 
                {
                    notRoomsBetween.setNumericValue(1,i);
                    file.Write(notRoomsBetween.getStringRepresentation(true));
                }

                //for rooms in the second rooms step look at distances to room 1

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
