/*  The PlanLevel class is used to store a users desired plans for the level in the form of a partially ordered graph, and convert that into code for Clingo.s
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCG_GUI.WorldModel;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PCG_GUI.PlanModel
{
    public class planStep :  INotifyPropertyChanged
    {
        public planStep(int stepNum)
        {
            this.stepNum = stepNum;
            stepRooms = new List<PlanRoom>();
        }

        public List<PlanRoom> stepRooms { get; set; }

        public int trueStepNum;

        public void stepUpdated()
        {
            RaisePropertyChanged("stepContents");
        }

        public int stepNum
        {

            get
            {
                return trueStepNum;
            }


            set
            {
                trueStepNum = value;
                RaisePropertyChanged("stepNum");
            }
        }

        public string stepContents
        {
            get
            {
                String contents = " ";
                foreach (PlanRoom r in stepRooms)
                {
                    if(r.type == roomTypes.BossFight)
                    {
                        contents += "BossFight ";
                    }

                    else if(r.type == roomTypes.Gauntlet)
                    {
                        contents += "Gauntlet ";
                    }

                    else if(r.type == roomTypes.TreasureRoom)
                    {
                        contents += "TreasureRoom ";
                    }
                }

                return contents;
            }
        }

        internal void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

    };

    public class PlanLevel : INotifyPropertyChanged
    {
        private List<PlanLink> links; //List of lists. Each individual list represents 1 step in the plan, and all the rooms that are part of that step. The overall list represents the order of the steps

        public ObservableCollection<planStep> stepList { get; set; }

        public int stepIndex {get; set;}
 

        int nextRoomNumber; //room number to give to the next room. Used to avoid having duplicate room numbers
        int maxExtraRooms; //max number of extra rooms to add that are not part of the plan

        //constructor
        public PlanLevel()
        {
            nextRoomNumber = 1; //the first room should be 1
            links = new List<PlanLink>();

            stepList = new ObservableCollection<planStep>();

            stepIndex = -1;

            //sample plan
            //addRoomToLevel(0);
            //addRoomToLevel(0);
            //addRoomToLevel(0);
            //addRoomToLevel(1);
            //addRoomToLevel(1);
            //addRoomToLevel(1);
            //addRoomToLevel(2);
            //addRoomToLevel(2);
            //addRoomToLevel(2);

            //plan[0][1].optionalRoom = true;
            //plan[1][1].optionalRoom = true;
            //plan[2][1].optionalRoom = true;

            //addLink(3, 6, 2);
            //addLink(4, 9, 2);

        }

        //adds a step to the step list
        public void addStep()
        {
            planStep nextStep = new planStep(stepList.Count + 1);
            stepList.Insert(stepList.Count, nextStep);
        }

        public void clearStep()
        {
            if (stepIndex != -1)
            {
                //Since we can't have a gap in room numbers update all the room numbers to remove the gap removing this rooms creates
                int roomsRemoved = stepList[stepIndex].stepRooms.Count;

                nextRoomNumber -= roomsRemoved;

                if(stepIndex != stepList.Count - 1) //if the step is not the last in the list
                {
                    for(int i = stepIndex + 1; i < stepList.Count; i++)
                    {
                        foreach (PlanRoom r in stepList[i].stepRooms)
                        {
                            r.roomNumber -= roomsRemoved;
                        }
                    }
                }

                stepList[stepIndex].stepRooms.Clear();

                stepList[stepIndex].stepUpdated();
            }
        }

        public void deleteStep()
        {
            System.Console.WriteLine(stepIndex);

            if (stepIndex != -1) //if a step is selected
            {
                if (stepIndex < stepList.Count - 1) //if the item selected is not the last in the list we need to decrease the step numbers for the rest of the list
                {
                    for (int i = stepIndex + 1; i < stepList.Count; i++)
                    {
                        stepList[i].stepNum--;
                    }
                }

                stepList.RemoveAt(stepIndex);
            }
        }

        //Add a Planroom room to the plan at the given step (with step being a 0 indexed value)
        public void addRoomToStep(PlanRoom room, int step)
        {
            while (stepList.Count <= step) //if there are not enough steps add them
            {
                planStep nextStep = new planStep(stepList.Count + 1);
                stepList.Insert(stepList.Count, nextStep);
            }

            stepList[step].stepRooms.Insert(stepList[step].stepRooms.Count, room);
            stepList[step].stepUpdated();
        }

        public void addBossRoom()
        {
            addRoom(roomTypes.BossFight);
        }

        public void addTreasureRoom()
        {
            addRoom(roomTypes.TreasureRoom);
        }


        public void addGauntlet()
        {
            addRoom(roomTypes.Gauntlet);
        }


        public void addRoom(roomTypes type)
        {
            if(stepIndex != -1)
            {

            PlanRoom room = new PlanRoom(nextRoomNumber, type);
            nextRoomNumber++;

            addRoomToStep(room, stepIndex);
            }
        }

        /*public void addRoomToLevel(int step)
        {
            PlanRoom r = new PlanRoom(nextRoomNumber, roomTypes.BossFight);
            nextRoomNumber++;

            addRoomToLevel(r, step);
        }*/

        public void addLink(int room1, int room2, int linkLength)
        {
            //PlanLink l = new PlanLink(room1, plan[getStepOfRoom(room1)], room2, plan[getStepOfRoom(room2)], linkLength);

            //links.Insert(links.Count, l);
        }


        public void writePlan(System.IO.StreamWriter file)
        {
            //write the necessary constants. Hardcoded for the moment won't be for long

            int minRoomsNeeded = 0; //how many rooms are needed
            int maxRoomsNeeded = 0;
            bool optional = false; //have any optional rooms been found
            bool linkRules = false; //are any links present that require us to print the rules for links 
            bool linkRulesForbid = false; //are any links present that require us to print the additional rules for links with forbidden rooms

            for(int i = 0; i < stepList.Count; i++) //go through each step
            {
                foreach (PlanRoom r in stepList[i].stepRooms) //go through each room
                { 
                    minRoomsNeeded++; 
                    r.writeRoom(file);

                    //Add xBeforeY atoms as appropiate for each room in the previous step.
                    //NOTE. This will fail quietly if I end up with an empty step which I haven't guarded against currently. 
                    if (i != 0) //if this is not the first step
                    {
                        Fact xBeforeY = new Fact();
                        xBeforeY.setPredicate("xBeforeY"); //the xBeforeY predicate takes the form xBeforeY(X,Y), saying roomX must occur before roomY
                        xBeforeY.setNumericValue(1, r.roomNumber); //the Y part is the current room and will remain fixed 

                        foreach (PlanRoom p in stepList[i - 1].stepRooms) //for each room in the previous step
                        {
                            if (!p.optionalRoom) //if p is not an optional room
                            {
                                xBeforeY.setNumericValue(0, p.roomNumber); //the X part is whatever the current previous room is
                                file.Write(xBeforeY.getStringRepresentation(true));
                            }
                        }

                    }

                    if(r.optionalRoom) //if r is optional make sure none of the rooms in the current or next step require you to visit R
                    {
                        optional = true;

                        //OK defining this is tricker then I expected so lets say that an optional room just has only one edge
                        Fact numberOfEdges = new Fact();
                        numberOfEdges.setPredicate("numberOfEdges");
                        numberOfEdges.setNumericValue(0, r.roomNumber);
                        numberOfEdges.setNumericValue(1, 1);
                        file.Write(numberOfEdges.getStringRepresentation(true));

                        //also to be safe make sure the optional room has no keys
                        Fact keyRoom = new Fact();
                        numberOfEdges.setPredicate("keyRoom");
                        numberOfEdges.setNumericValue(0, r.roomNumber);
                        numberOfEdges.setValue(1, "_");
                        file.Write(numberOfEdges.getStringRepresentation(true, true));

                        /*
                        Fact mustVisitXtoReachY = new Fact();
                        mustVisitXtoReachY.setPredicate("mustVisitXtoReachY"); //format is mustVisitXtoReachY(X,Y), means that to get to Y you must pass through X first
                        mustVisitXtoReachY.setNumericValue(0, r.roomNumber);

                        foreach(PlanRoom c in plan[i]) //for each room in the current step
                        {
                            if(r.roomNumber != c.roomNumber) //if this is a different room
                            {
                                mustVisitXtoReachY.setNumericValue(1, c.roomNumber);
                                file.Write(mustVisitXtoReachY.getStringRepresentation(true, true));
                            }
                        }
                        */

                        if (i < stepList.Count - 1) //if there is a next level you must not have to visit those rooms to reach the optional room
                        {
                            Fact mustVisitXtoReachY = new Fact();
                            mustVisitXtoReachY.setPredicate("mustVisitXtoReachY"); //format is mustVisitXtoReachY(X,Y), means that to get to Y you must pass through X first
                            mustVisitXtoReachY.setNumericValue(1, r.roomNumber);

                            foreach (PlanRoom n in stepList[i + 1].stepRooms) //for each room in the current step
                            {
                                if (r.roomNumber != n.roomNumber) //if this is a different room
                                {
                                    mustVisitXtoReachY.setNumericValue(0, n.roomNumber);
                                    file.Write(mustVisitXtoReachY.getStringRepresentation(true, true));
                                }
                            }
                        }

                    }
                    
                }
            }

            foreach(PlanLink l in links)
            {

                linkRules = true;
                if (l.getForbiddenRoomsCount() > 0)
                {
                    linkRulesForbid = true;
                    minRoomsNeeded += l.linkLength;
                }


                 l.writeLink(file);

            }

            maxRoomsNeeded = minRoomsNeeded + maxExtraRooms;

            file.WriteLine("levelStartRoom(1).");

            //Clingo will throw a fit if we put in the rules for optional rooms and no rooms using it are specified. So only write this when we need it
            if(optional)
            {
                file.WriteLine(":- numberOfEdges(ID,C), numConnections(ID,N), C != N. %if a room has a set number of connections enforce that number");
            }

            //again if there are any links there are additional rules we need to add
            if (linkRules)
            {
                file.WriteLine("% Rules for links.");
                file.WriteLine("% to find distance we find all rooms reachable from U by passing through 0 other rooms, then 1, etc.");
                file.WriteLine("% if we find a path that is shorter then roomsBetween, we have a problem.");
                file.WriteLine("% else if not path exists of the desired lenght we have a problem");
                file.WriteLine("% findReachableInN(C,U,V,CN,N) means we are at C, we are trying to reach V having started at U, we have passed through CN rooms, and our target distance is N");
                file.WriteLine("findReachableInN(U,U,V,0,N)  :- roomsBetween(U,V,N). %start looking for a path at U");
                file.WriteLine("findReachableInN(C2,U,V,CN+1,N)  :-  findReachableInN(C1,U,V,CN,N), CN < N, edge(C1,C2).");
                file.WriteLine(":-  findReachableInN(C,U,V,CN,N), CN < N, edge(C,V). %If there is an edge to v before we have enough rooms there is a problem");
                file.WriteLine("distanceCorrect(U,V,N) :-  findReachableInN(C,U,V,CN,N), CN = N, edge(C,V). %If there is an edge to v when we have enough rooms we are golden");

                file.WriteLine(":- roomsBetween(U,V,N), not distanceCorrect(U,V,N).");

            }

            //again if there are any links with forbiddenre there are additional rules we need to add
            if (linkRulesForbid)
            {
                file.WriteLine("% Rules for links with forbidden rooms.");
                file.WriteLine("% findNotReachableInN(C,U,V,CN,N) means we are at C, we are trying to reach V having started at U, we have passed through CN rooms, and our target distance is N");
                file.WriteLine("findNotReachableInN(U,U,V,0,N)  :- notRoomsBetween(U,V,N). %start looking for a path at U");
                file.WriteLine("findNotReachableInN(C2,U,V,CN+1,N)  :-  findNotReachableInN(C1,U,V,CN,N), CN < N, edge(C1,C2).");
                file.WriteLine(":-  findNotReachableInN(C,U,V,CN,N), CN <= N, edge(C,V). %If there is an edge to v before we have enough rooms there is a problem");

            }


            //write the constants
            file.WriteLine("#const minRooms=" + minRoomsNeeded.ToString() + ".");
            file.WriteLine("#const maxRooms=" + maxRoomsNeeded.ToString() + ".");


        }

        //give a room number find the step that room is in or -1 if the room does not exist
        public int getStepOfRoom(int roomNumber)
        {
            int roomStep = -1;

            for(int i = 0; i < stepList.Count; i++)
            {
                for (int j = 0; j < stepList[i].stepRooms.Count; j++)
                {
                    if (stepList[i].stepRooms[j].roomNumber == roomNumber)
                    {
                        roomStep = i;
                    }
                }
            }

            return roomStep;
        }

        public void clear()
        {
            nextRoomNumber = 1;
            stepList.Clear();
            RaisePropertyChanged("stepList");
        }

        internal void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}