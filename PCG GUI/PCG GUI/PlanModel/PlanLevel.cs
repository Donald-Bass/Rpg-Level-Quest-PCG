/*  The PlanLevel class is used to store a users desired plans for the level in the form of a partially ordered graph, and convert that into code for Clingo.
 * 
 * To be more specific the PlanLevel class represents the overall plan for the level. The plan is a list of steps, with each step holding a set of rooms (which are intended to represent the core content of
 * the level) that must be visited by the player before any room that is part of a future step can be reached.
 * 
 * There are several pieces of functionality found here that is not usable from the GUID yet. The main two are pacing/links, and optional rooms. Pacing and Links are really the same thing for the most part 
 * a way of dicating how much time passes between major rooms. A link specifies one room in one step and a second in the next step, and says that the first room is the last room visited in it's step, 
 * the second room is the first room visited in it's step, and a set number of rooms not specifically part of the plan must occur inbetween. Pacing is similar but doesn't have predetermined rooms 1 and 2
 * Instead pacing just says that somehow there is X non important rooms between the last room of one step and the first room of the next (This ends up using the same code as links for now with the GUI 
 * randomly picking rooms to be room 1 and 2). 
 * 
 * Optional rooms are rooms that are well optional. This means that they must be placed in such a way that you must visit all the (nonoptional) rooms from previous steps, before it can be reached
 * but once it can be reached, visiting it is not a requirement to reach any of the rooms in the next step. This is useful for adding side rooms and the like.
 * 
 * Also the first room is hardcoded to be the entrance and be the only room in its step to simplfy the pcg. The UI currently doesn't really give any feedback on this issue though
 * 
 * Also search for TODO and you may find a few bugs that I didn't have time to fix but knew about. Hopefully though I will have fixed them and removed this
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
    //The planStep class is used to represent a single step in the overall plan. The INotifyPropertyChanged allows the class to notify the GUI when some of the values stored in it changes
    //Allowing the UI to match the actual contents of the plan being generated
    public class planStep :  INotifyPropertyChanged 
    {
        public planStep(int stepNum) 
        {
            this.stepNum = stepNum;
            stepRooms = new List<PlanRoom>();
            pacing = 1;
        }

        public List<PlanRoom> stepRooms { get; set; } //list of all rooms part of the current step

        //we need a variable to keep track what number step the current object holds. This is complicated by the fact that we need to alert the GUI if the step number
        //disapears (such as if a previous step was deleted) so we need a custom set function. Since this set function has to change the value of the variable to avoid an infinite loop of the set
        //function calling it self we have a trueStepNum variable that actually holds the value, and a stepNum "variable" which has the get and set functions that the rest of the code uses
        //There is probally a better way of doing this if you know C# better then I do but this works
        public int trueStepNum; //what is the number of the step. 

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


        public int pacing; //how many rooms should occur between this level and the next

        //if we update the contents of the step this room alerts the gui to that fact
        public void stepUpdated()
        {
            RaisePropertyChanged("stepContents");
        }

      
        //Function that returns a string representation of the contents of the step, listing the types of rooms we have in the step
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

                    else if(r.type == roomTypes.Entrance)
                    {
                        contents += "Entrance ";
                    }
                }

                return contents;
            }
        }

        //used to alert the GUI to any changes in data
        internal void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

    };

    //This class represents the overall plan. It essential has a list of planStep objects and the functions needed to manage the overall list
    public class PlanLevel : INotifyPropertyChanged
    {
        public ObservableCollection<planStep> stepList { get; set; } //An ObservableCollection is a special type of collection that allows the gui to detect items being added and removed
                                                                     //and in general draw data from

        public int stepIndex {get; set;} //what is the index of the step currently selected in the gui
 

        int nextRoomNumber; //room number to give to the next room. Used to avoid having duplicate room numbers
        //int maxExtraRooms; //max number of extra rooms to add that are not part of the plan. This is not yet implemented in any way shape or form

        //constructor
        public PlanLevel()
        {
            nextRoomNumber = 2; //The first room is hardcoded to be the entrance so the first room we create will be room 2

            stepList = new ObservableCollection<planStep>();

            stepIndex = -1; //the UI starts out with no step selected

            //create a step containing just the entrance and then a second blank step for the user to start putting rooms in
            PlanRoom Entrance = new PlanRoom(1, roomTypes.Entrance);

            addStep();
            addRoomToStep(Entrance, 0);
            addStep();
        }

        //adds an empty step to the step list
        public void addStep()
        {
            planStep nextStep = new planStep(stepList.Count + 1);
            stepList.Insert(stepList.Count, nextStep);
        }

        //Takes the currently selected step and removes all rooms from said step, as well as update all the room numbers to ensure there is no gap in them (One of the assumptions
        //The PCG code makes to keep things simple is that if it is told that there are X rooms those rooms have the numbers 1..X with no gaps).
        public void clearStep()
        {
            if (stepIndex > 0) //the first step is hardcoded and shouldn't be changable anything else is fair game
            {
                //Since we can't have a gap in room numbers we need update all the room numbers to remove the gap removing this rooms creates
                int roomsRemoved = stepList[stepIndex].stepRooms.Count; //get the number of rooms removed

                nextRoomNumber -= roomsRemoved; //update the next room number to use to account for the rooms removed

                if(stepIndex != stepList.Count - 1) //if the step is not the last in the list
                {
                    for(int i = stepIndex + 1; i < stepList.Count; i++) //reduce the room numbers of all rooms that come after the step
                    {
                        foreach (PlanRoom r in stepList[i].stepRooms)
                        {
                            r.roomNumber -= roomsRemoved;
                        }
                    }
                }

                stepList[stepIndex].stepRooms.Clear(); //remove all rooms

                stepList[stepIndex].stepUpdated(); //notify the gui that the step was updated
            }
        }

        //this function removes the step currently selected in the GUI  from the stepList 
        public void deleteStep()
        {
            if (stepIndex > 0) //the first step is hardcoded and shouldn't be changable anything else is fair game
            {
                clearStep(); //We need to clear all the rooms from the step first or the room numbers will be incorrect

                if (stepIndex < stepList.Count - 1) //if the item selected is not the last in the list we need to decrease the step numbers for the rest of the list
                {
                    for (int i = stepIndex + 1; i < stepList.Count; i++)
                    {
                        stepList[i].stepNum--;
                    }
                }

                stepList.RemoveAt(stepIndex); //finally remove the step
            }
        }

        //Add a Planroom room to the plan in the step found at the given index step (with step being a 0 indexed value (so the first step has an index of 0)
        public void addRoomToStep(PlanRoom room, int step)
        {
            while (stepList.Count <= step) //This should probally never trigger but if the index is for a step that hasn't been created, keep creating new steps until said step is created)
            {
                planStep nextStep = new planStep(stepList.Count + 1);
                stepList.Insert(stepList.Count, nextStep);
            }

            stepList[step].stepRooms.Insert(stepList[step].stepRooms.Count, room);
            stepList[step].stepUpdated(); //notify the gui that the step has been updated
        }

        //These functions add rooms of different types
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

        //Given a type of room, create a room of that type and add it to the step currently selected
        public void addRoom(roomTypes type)
        {
            if(stepIndex > 0) //the first step is hardcoded and shouldn't be changable. Everything else is fair game
            {

            PlanRoom room = new PlanRoom(nextRoomNumber, type);
            nextRoomNumber++;

            addRoomToStep(room, stepIndex);
            }
        }

        //Add a link between 2 rooms. This functionality is incomplete
        public void addLink(int room1, int room2, int linkLength)
        {
            //PlanLink l = new PlanLink(room1, plan[getStepOfRoom(room1)], room2, plan[getStepOfRoom(room2)], linkLength);

            //links.Insert(links.Count, l);
        }

        //Write a set of Clingo rules to a file that contain the series of rules necessary to ensure that whatever Clingo generates as output matches the plan
        public void writePlan(System.IO.StreamWriter file)
        {
            //write the necessary constants. Hardcoded for the moment 
            int minRoomsNeeded = 0; //how many rooms are needed
            int maxRoomsNeeded = 0; //the maximum number of rules to allow
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
                    //TODO. This will fail quietly if I end up with an empty step which I haven't guarded against currently. 
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

                        //OK defining this is tricker then I expected so for now lets say that an optional room just has only one edge connecting it
                        //to the rest of the graph
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
                        file.Write(numberOfEdges.getStringRepresentation(true, true)); //the second true means the atom we generated must not exist

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
                                    file.Write(mustVisitXtoReachY.getStringRepresentation(true, true)); //the second true means the atom we generated must not exist
                                }
                            }
                        }

                    }
                    
                }
            }

            int previousRoom2 = -1; //keep track of what was the second room of the previous link generated to implement pacing

            //Go through the steps and handle the pacing between them. This will need updating if other links are added but the code should remain quite similar
            for (int i = 0; i < stepList.Count - 1; i++) //for each step but the last
            {
                System.Random rand = new System.Random();

                //randomly pick a room from the current step and the next step to generate a link between
                int room1 = stepList[i].stepRooms[rand.Next(0,stepList[i].stepRooms.Count)].roomNumber;
 
                while(stepList[i].stepRooms.Count > 1 && room1 == previousRoom2) //if there is more then 1 room in the current step don't pick a room that was used as room2 on the previous step
                {
                    room1 = stepList[i].stepRooms[rand.Next(0, stepList[i].stepRooms.Count)].roomNumber;
                }
                
                int room2 = stepList[i+1].stepRooms[rand.Next(0, stepList[i+1].stepRooms.Count)].roomNumber;
                previousRoom2 = room2;

                PlanLink l = new PlanLink(room1, stepList[i].stepRooms, room2, stepList[i+1].stepRooms, stepList[i].pacing); //generate a link between those two rooms

                linkRules = true;
                if (l.getForbiddenRoomsCount() > 0) //check if we need to forbid any rooms. (More detail about what this means in the PlanLink class and below
                {
                    linkRulesForbid = true;
                }

                minRoomsNeeded += l.linkLength; //add the number of rooms we need in the link to the total number of rooms needed

                l.writeLink(file);

            }

            maxRoomsNeeded = minRoomsNeeded + 2; //Placeholder. Allow for there to be 2 more rooms then necessary

            file.WriteLine("levelStartRoom(1)."); //Room 1 is hardcoded to always be the start of the level

            //set all rooms after the last room specifically specified by the plan as generic rooms. 
            Fact typeOfRoom = new Fact();
            typeOfRoom.setPredicate("typeOfRoom");
            typeOfRoom.setValue(1, "generic");

            for (int i = nextRoomNumber; i <= maxRoomsNeeded; i++ ) //this may set more rooms to generic then are ultimately generated in the level.
                                                                    //clingo should harmlessly ignore the extra room designations
            {
                typeOfRoom.setNumericValue(0, i);
                file.WriteLine(typeOfRoom.getStringRepresentation(true));
            }

                //Finally output only the clingo rules needed for the particular plan. This will include extensive comments in the output file describing what they do, which is
                //all the stuff found after each % if you want to read them in this file

                //Clingo will throw a fit if we put in the rules for optional rooms and no rooms using it are specified. So only write this when we need it
                if (optional)
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
                file.WriteLine("% Forbidden rooms are rooms the link is not allowed to pass through.");
                file.WriteLine("% It is also disallowed for there to be a path between the two endpoints of the link that passes through a forbidden room and is shorter then the actual link's path is.");
                file.WriteLine("% This is used when connecting the last room of one step with the first room of another to ensure that you don't end up with other rooms from those step in between them violating their status as last and first.");
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

            for(int i = 0; i < stepList.Count; i++) //for each step
            {
                for (int j = 0; j < stepList[i].stepRooms.Count; j++) //for eah room in that step
                {
                    if (stepList[i].stepRooms[j].roomNumber == roomNumber)
                    {
                        roomStep = i;
                    }
                }
            }

            return roomStep;
        }

        //This function resets the plan to it's original mostly empty state. (One step with just the enterance, then a second empty step)
        public void clear()
        {
            nextRoomNumber = 2;
            stepList.Clear();

            stepIndex = -1;

            PlanRoom Entrance = new PlanRoom(1, roomTypes.Entrance);

            addStep();
            addRoomToStep(Entrance, 0);
            addStep();

            RaisePropertyChanged("stepList");
        }

        internal void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}