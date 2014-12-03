/*The atom class is used to help with communication between clingo and the GUI. It is capable of taking a string containing an atom output by Clingo and parsing said string
 *to read in the atom and it's contents, or of being set to a specific type of atom and being filled with certain values, and then outputting a string that contains a representation of its contents
 *in the form clingo would read an atom in as input
 
  I am probally unfortantly screwing up the termanology here a bit. I did some searching but I couldn't really figure out what the offical term in Clingo was for the values stored inside of an atom
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG_GUI
{
    public class Atom
    {
        private String atomName; //what type of atom is it
        private List<String> values; //values for each of the variables in the atom
                                     //these are usually but not always numbers. As such we have to use strings to store them, but there are several functions provided to make numerical values easier to work with

        public Atom() //base constructor
        {
            atomName = "";
            values = new List<String>();
        }

        //constuctor for use in parsing a string from a clingo output file and extracting the atom contained within said string
        //the format of an atom is NAME(v1,v2....,vn)
        public Atom(string factString) 
        {
            int parIndex; //index of the (

            parIndex = factString.IndexOf("("); //find where the ( is which marks the end the name of the atom
            atomName = factString.Substring(0, parIndex); //store the name of the atom

            values = factString.Substring(parIndex + 1, factString.Count() - parIndex - 2).Split(',').ToList(); //split everything between the ( ), 
                                                                                                                //into the individual values using the fact that they are seperated by ','s
        }

        //Constructor that given a type of Atom and a set of values for its variables store the atom
        public Atom(string atomType, String[] values)
        {
            this.atomName = atomType;
            this.values = values.ToList();
        }

        //return the number of values stored within the atom
        public int getNumVariables()
        {
            return values.Count;
        }

        public string getAtomName()
        {
            return atomName;
        }

        public void setAtomName(string predicate)
        {
            this.atomName = predicate;
        }

        //get the ith value in the atom
        public string getValue(int i)
        {
            return values[i];
        }

        //set the ith value in the atom. This will automatically resize the atom if there is no ith value already
        public void setValue(int i, string Value)
        {
            while (i >= values.Count) //if we don't have i elements create them till we have that many
            {
                string blank = "";

                values.Insert(values.Count, blank);
            }

            values[i] = Value;
        }

        //Get the ith value in the atom as an integer. Currently there is no error checking if said value is not really an int
        public int getNumericValue(int i)
        {
            return Convert.ToInt32(values[i]);
        }

        //Take a value as an int and turn in into a string before storing (since all values are stored as strings even if they are really integers)
        public void setNumericValue(int i, int value)
        {
            setValue(i, value.ToString());
        }

        //returns a string representation of the atom in the same format clingo would output
        //If ClingoInput is true then output in the form clingo needs as input (adds a . to the end of the atom which marks the end of a line in an input file)
        //If ClingoInput is false then a space will be added instead is important because this allows us to mimic the format of the files Clingo outputs, which has the same format for atoms but puts them all
        //on one line with a single space seperating each from the next
        //If not is true add a :- in front of the atom (This only makes sense when we are creating a file to use as input to Clingo. In such a case this means that we are telling Clingo to not let this
        //atom exist. This has absolutely no meaning in the type of files clingo outputs, so doing this while trying to emulate one will only break parsing further down the road)
        public string getStringRepresentation(bool ClingoInput = false, bool not = false)
        {
            string rep = ""; //the string represenation

            if(not) //if we are creating an atom that cannot exist add the :-
            {
                rep = rep + ":- ";
            }

            rep = rep + atomName + "("; //print the name of the atom

            if (values.Count > 0) //if there is at least one value inside the atom add the values to the string
                                  //(Technically this check doesn't really fully handle a atom with no values properly. There would be no () in that case
                                  //but I never bothered to fix it since I haven't used any such types of atoms, and its easier to add a dummy 1 value inside then to handle all the special cases
                                  //parsing such atomTypes would require)
            {
                rep += values[0]; //print the first value outside of the loop to make adding commas easier

                for (int i = 1; i < values.Count; i++) //for each additional value print a , to seperate then the value
                {
                    rep = rep + "," + values[i];
                }
            }

            rep += ")"; //finish the atom

            if (ClingoInput)
            {
                rep += ".\n"; //all atoms in input files for clingo need to end with a .
            }

            else
            {
                rep += " "; //add a space before next atom if we're instead trying to match na output file
            }

            return rep;
        }
    }
}
