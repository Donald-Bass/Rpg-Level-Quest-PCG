//code for reading and writing atoms in the format that clingo outputs

#include "atom.h"

#include<string>

atom::atom()
{

}

atom::atom(string toParse)
{
    parseString(toParse);
}

atom::~atom()
{

}

void atom::parseString(string toParse)
{
    allPreds.clear();

    int substrEnd; //the end of the particular substring we want at any point in time

    substrEnd = toParse.find("("); //find the ( seperating the atom name and predicates

    atomName = toParse.substr(0,substrEnd);
    toParse = toParse.substr(substrEnd+1); //remove everything up to and including the (

    substrEnd = toParse.find(","); //find the first ,

    while(substrEnd != -1)
    {
        allPreds.push_back(toParse.substr(0,substrEnd));
        toParse = toParse.substr(substrEnd+1); //remove everything up to and including the ()
        substrEnd = toParse.find(","); //find the first ,
    }

    //the last predicate should be all the remaining string except for the last character (the ')' )
    allPreds.push_back(toParse.substr(0,toParse.length() - 1));

}
