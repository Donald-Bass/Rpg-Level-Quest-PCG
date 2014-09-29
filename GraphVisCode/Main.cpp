#include <ogdf/basic/Graph.h>
#include <ogdf/basic/GraphAttributes.h>
#include <ogdf/planarity/PlanarizationLayout.h>
#include <ogdf/planarity/VariableEmbeddingInserter.h>
#include <ogdf/planarity/FastPlanarSubgraph.h>
#include <ogdf/orthogonal/OrthoLayout.h>
#include <ogdf/planarity/EmbedderMinDepthMaxFaceLayers.h>
#include <ogdf/basic/extended_graph_alg.h>


#include <ogdf/energybased/StressMajorizationSimple.h>
#include <ogdf/energybased/CoinTutteLayout.h>

#include <ogdf/energybased/DavidsonHarelLayout.h>
#include <ogdf/energybased/SpringEmbedderKK.h>
#include <ogdf/energybased/SpringEmbedderFR.h>
#include <ogdf/energybased/FMMMLayout.h>
#include <ogdf/energybased/GEMLayout.h>

#include<vector>
#include "atom.h"

using namespace ogdf;
using namespace std;

vector<atom> roomAtoms;
vector<atom> edgeAtoms;

int numRooms = -1;

void parseAtom(atom toParse)
{
    if(toParse.atomName == "room")
    {
        roomAtoms.push_back(toParse);

        //int vertIndex = atoi(toParse.allPreds[0].c_str()) - 1; //convert from 1 indexed to 0 indexed
        //allVertexs[vertIndex].cluster = atoi(toParse.allPreds[1].c_str());
    }

    else if(toParse.atomName == "edge")
    {
        edgeAtoms.push_back(toParse);

/*
        edge newEdge;
        newEdge.vertex1Index = (atoi(toParse.allPreds[0].c_str())) - 1; //convert from 1 indexed to 0 indexed
        newEdge.vertex2Index = (atoi(toParse.allPreds[1].c_str())) - 1; //convert from 1 indexed to 0 indexed

        if(!isEdge(newEdge.vertex1Index, newEdge.vertex2Index)) //if the edge doesn't already exist
        {
            allEdges.push_back(newEdge);
        }*/
    }

    else if(toParse.atomName == "numRooms")
    {
        numRooms = atoi(toParse.allPreds[0].c_str());
    }
}

void parseInputFile()
{
    ifstream input;
    input.open("results.pcg");

    string atomsString;
    int spacePosition;

    //skip the first 4 lines
    getline(input, atomsString);
    getline(input, atomsString);
    getline(input, atomsString);
    getline(input, atomsString);

    getline(input, atomsString);

    spacePosition = atomsString.find(" ");

    while(spacePosition != -1)
    {
        atom curAtom;
        curAtom.parseString(atomsString.substr(0,spacePosition));
        parseAtom(curAtom);
        atomsString = atomsString.substr(spacePosition+1);
        spacePosition = atomsString.find(" ");
    }

    //get the last atom
    atom curAtom;
    curAtom.parseString(atomsString);
    parseAtom(curAtom);

    input.close();
}

void createGraph()
{
    Graph G;
    GraphAttributes GA(G, GraphAttributes::nodeGraphics | GraphAttributes::edgeGraphics | GraphAttributes::nodeLabel);

    node* allNodes = new node[numRooms];

    atom curAtom;

    //create the rooms
    for(int i = 0; i < roomAtoms.size(); i++)
    {
        curAtom = roomAtoms[i];
        int roomNumber = atoi(curAtom.allPreds[0].c_str());
        allNodes[roomNumber - 1] = G.newNode(); //add the node to the graph, and store that node for use with adding edges

        node CurNode = allNodes[roomNumber - 1];

        GA.x(CurNode) = -5*(i+1);
		GA.y(CurNode) = -20*i;
		GA.width(CurNode) = 30;
		GA.height(CurNode) = 30;

        const ogdf::String roomLabel(curAtom.allPreds[0].c_str());

		GA.labelNode(CurNode) = roomLabel; //curAtom.allPreds[0];
    }

    //create the edges
    for(int i = 0; i < edgeAtoms.size(); i++)
    {
        curAtom = edgeAtoms[i];
        //get the two endpoints
        int u = atoi(curAtom.allPreds[0].c_str());
        int v = atoi(curAtom.allPreds[1].c_str());

        if(u < v) //each edge is listed twice, once as (u,v) and once as (v,u). To ensure they are only added once only add the edge when u < v
        {
            edge curEdge = G.newEdge(allNodes[u - 1], allNodes[v - 1]); //create the node. The -1 is to convert from 1 based to 0 based indices.

        }

    }


    if(isPlanar(G))
    {
        cout << "Planar" << endl;
    }

    else
    {
        cout << "Non Planar" << endl;
    }

    PlanarizationLayout pl;

    FastPlanarSubgraph *ps = new FastPlanarSubgraph;
    ps->runs(100);
    VariableEmbeddingInserter *ves = new VariableEmbeddingInserter;
    ves->removeReinsert(EdgeInsertionModule::rrAll);
    pl.setSubgraph(ps);
    pl.setInserter(ves);


    EmbedderMinDepthMaxFaceLayers *emb = new EmbedderMinDepthMaxFaceLayers;
    pl.setEmbedder(emb);

    OrthoLayout *ol = new OrthoLayout;
    ol->separation(40.0);
    ol->cOverhang(0.4);
    ol->setOptions(2+4);
    pl.setPlanarLayouter(ol);

    pl.call(GA);


    GA.writeSVG("dungeonLayout.svg");
    GA.writeGML("dungeonLayout.gml");

    //GA.writeDungeon("er-diagram-layout.pcg");

    /*for(int i = 0; i < numRooms; i++)
    {
        //cout << GA.x(allNodes[i]) << endl;
    }


    GEMLayout fmmm;
    fmmm.minDistCC(150);
    //fmmm.setPlanarityWeight(20.0);
    //fmmm.setRepulsionWeight(10.0);
    //fmmm.setPreferredEdgeLength(100);

	//fmmm.useHighLevelOptions(true);
	//fmmm.unitEdgeLength(150.0);
	//fmmm.newInitialPlacement(true);
	//fmmm.qualityVersusSpeed(FMMMLayout::qvsGorgeousAndEfficient);

	fmmm.call(GA);
	GA.writeSVG("LayoutTest.svg"); */

    delete(allNodes);


}



int main()
{
    parseInputFile();
    createGraph();

	return 0;
}

