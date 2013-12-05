//holds the state of the entire world.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG_GUI.Facts
{
    class World
    {
        private Fact[] allFacts;
        int numLevels; //how many different levels are there
        int levelDimensions[,]; //dimensions of each level. First index is level number second is x/y (0 is x dimension 1 is y dimension)
        Level[] allLevels; //all levels

        public World()
        {

        }

        public void parseClingoFile(System.IO.StreamReader file)
        {
            Fact curFact;
            
            file.ReadLine(); //the first line is the number of answers which we can skip
            String[] StringFacts = file.ReadLine().Trim().Split(' '); //the second line has the model so split all the facts into seperate strings

            //parse all facts
            //allFacts = new Fact[StringFacts.Length];



            for (int i = 0; i < StringFacts.Length; i++ )
            {
                curFact = new Fact(StringFacts[i]);
                processFact(curFact);
            }

            Tile test = new Tile();
            //test.

            //return allFacts;
        }

        public void processFact(Fact fact)
        {

        }
    
    }

 
    
}
