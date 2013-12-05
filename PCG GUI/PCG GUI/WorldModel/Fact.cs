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
        private String[] values; //values for each of the variables in the predicate

        public Fact() //base constructor
        {
            predicate = "";
            values = new String[0];
        }

        //constructor that parses a fact from a clingo output file. The string factString should start with the predicate and end with the )
        //TODO: ? Add error checking
        public Fact(string factString) 
        {
            int parIndex; //index of the (

            parIndex = factString.IndexOf("("); //find where the ( is ending the name of the predicate
            predicate = factString.Substring(0, parIndex);

            values = factString.Substring(parIndex + 1, factString.Count() - parIndex - 2).Split(',');
        }

        public Fact(string predicate, String[] values)
        {
            this.predicate = predicate;
            this.values = values;
        }

        //return number of values in the fact
        public int getNumVariables()
        {
            return values.Length;
        }

        public string getPredicate()
        {
            return predicate;
        }

        public string[] getValues()
        {
            return values;
        }

        public string getValue(int i)
        {
            return values[i];
        }

        //returns a string representation of the fact in the same format clingo would output
        public string getStringRepresentation()
        {
            string rep = predicate + "(";

            if(values.Length > 0) //if there is at least one value
            {
                rep += values[0]; //print the first value outside of the loop to make adding commas easier

                for(int i = 1; i < values.Length; i++)
                {
                    rep = rep + "," + values[i];
                }
            }

            rep += ")";

            return rep;
        }
    }
}
