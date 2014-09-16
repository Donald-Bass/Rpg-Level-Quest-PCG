#ifndef ATOM_H
#define ATOM_H


#include<vector>
#include<string>

using namespace std;

class atom
{
    public:
        atom();
        atom(string toParse);

        virtual ~atom();

        string atomName;
        vector<string> allPreds;

        void parseString(string toParse);
        string toString();

    protected:
    private:
};

#endif // ATOM_H
