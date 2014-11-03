using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG_GUI
{
    class Fact
    {
        private String predicate; //what predicate is the fact from
        private List<String> values; //values for each of the variables in the predicate

        public Fact() //base constructor
        {
            predicate = "";
            values = new List<String>();
        }

        //constructor that parses a fact from a clingo output file. The string factString should start with the predicate and end with the )
        //TODO: ? Add error checking
        public Fact(string factString) 
        {
            int parIndex; //index of the (

            parIndex = factString.IndexOf("("); //find where the ( is ending the name of the predicate
            predicate = factString.Substring(0, parIndex);

            values = factString.Substring(parIndex + 1, factString.Count() - parIndex - 2).Split(',').ToList();
        }

        //TODO go back to all these references and make them use a proper list
        public Fact(string predicate, String[] values)
        {
            this.predicate = predicate;
            this.values = values.ToList();
        }

        //return number of values in the fact
        public int getNumVariables()
        {
            return values.Count;
        }

        public string getPredicate()
        {
            return predicate;
        }

        public void setPredicate(string predicate)
        {
            this.predicate = predicate;
        }

        public string getValue(int i)
        {
            return values[i];
        }

        public void setValue(int i, string Value)
        {
            while (i >= values.Count) //if we don't have i elements create them till we have that many
            {
                string blank = "";

                values.Insert(values.Count, blank);
            }

            values[i] = Value;
        }

        //convert a value into an int before returning it
        public int getNumericValue(int i)
        {
            return Convert.ToInt32(values[i]);
        }

        //Take a value as an int and turn in into a string before stroing
        public void setNumericValue(int i, int value)
        {
            setValue(i, value.ToString());
        }

        //returns a string representation of the fact in the same format clingo would output
        //If ClingoInput is true then output in the form clingo needs as input (add a .)
        //If not is true add a not in front of the predicate (only use when ClingoInput is true for now)
        public string getStringRepresentation(bool ClingoInput = false, bool not = false)
        {
            string rep = "";

            if(not)
            {
                rep = rep + ":- ";
            }

            rep = rep + predicate + "(";

            if (values.Count > 0) //if there is at least one value
            {
                rep += values[0]; //print the first value outside of the loop to make adding commas easier

                for (int i = 1; i < values.Count; i++)
                {
                    rep = rep + "," + values[i];
                }
            }

            rep += ")";

            if (ClingoInput)
            {
                rep += ".\n"; //all facts in clingo need to end with a .
            }

            else
            {
                rep += " "; //add space before next fact
            }

            return rep;
        }
    }
}
