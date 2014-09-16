#include "DungeonLayout.h"
#include<iostream>

DungeonLayout::DungeonLayout()
{
    //ctor

    grid = NULL;
}

DungeonLayout::~DungeonLayout()
{
    if(grid != NULL)
    {

        for(int i = 0; i < yCoords.size(); i++)
        {
            delete grid[i];
        }

        delete grid;
    }
}

void DungeonLayout::outputCoord()
{

    cout << "Start columns" << endl;
    for(list<coordRange>::iterator i = xCoords.begin(); i != xCoords.end(); i++)
    {
        cout << i -> lowerBound << " " << i -> upperBound << endl;
    }

    cout << "Start rows" << endl;

    for(list<coordRange>::iterator i = yCoords.begin(); i != yCoords.end(); i++)
    {
        cout << i -> lowerBound << " " << i -> upperBound << endl;
    }

    cout << "End rows" << endl;
}

//check if x is in the list, and if not add it to the list
void DungeonLayout::addColumn(int x)
{
    //figure out the front and back of the range of coordinates the room occupies
    coordRange coords;

    coords.lowerBound = x - (OGDF_ROOM_SIZE / 2);
    coords.upperBound = x + (OGDF_ROOM_SIZE / 2);

    bool found = false;
    list<coordRange>::iterator i = xCoords.begin();

    while(i != xCoords.end() && !found)
    {
        if(i -> lowerBound == coords.lowerBound)
        {
            found = true;
        }

        else if(i -> lowerBound > coords.lowerBound)
        {
            xCoords.insert(i, coords);
            found = true;
        }

        i++;
    }

    //if found is still false when we reach here we need to add x to the end
    if(!found)
    {
        xCoords.insert(i, coords);
    }
}

void DungeonLayout::addRow(int y)
{
    //figure out the front and back of the range of coordinates the room occupies
    coordRange coords;

    coords.lowerBound = y - (OGDF_ROOM_SIZE / 2);
    coords.upperBound = y + (OGDF_ROOM_SIZE / 2);

    bool found = false;
    list<coordRange>::iterator i = yCoords.begin();

    while(i != yCoords.end() && !found)
    {
        if(i -> lowerBound == coords.lowerBound)
        {
            found = true;
        }

        else if(i -> lowerBound > coords.lowerBound)
        {
            yCoords.insert(i, coords);
            found = true;
        }

        i++;
    }

    //if found is still false when we reach here we need to add x to the end
    if(!found)
    {
        yCoords.insert(i, coords);
    }
}


void DungeonLayout::setUpGrid()
{
    grid = new room*[xCoords.size()];

    for(int i = 0; i < xCoords.size(); i++)
    {
        grid[i] = new room[yCoords.size()];

        for(int j = 0; j < yCoords.size(); j++)
        {
            grid[i][j].id = -1;
            grid[i][j].edgeDown = false;
            grid[i][j].edgeUp = false;
            grid[i][j].edgeLeft = false;
            grid[i][j].edgeRight = false;
        }
    }
}

void DungeonLayout::addRoom(int x, int y, int id)
{
    //convert coordinates in layout to coordinates in the grid
    int gridX, gridY;

    gridX = findGridX(x);
    gridY = findGridY(y);

    grid[gridX][gridY].id = id;

}

void DungeonLayout::addCorridor(int startX, int startY, int endX, int endY)
{
    int lowX, lowY, highX, highY;
    bool eastWest;

    //first make sure we know which X Y values are low and which are high

    if(startX <= endX)
    {
        lowX = startX;
        highX = endX;
    }

    else
    {
        lowX = endX;
        highX = startX;
    }

    if(startY <= endY)
    {
        lowY = startY;
        highY = endY;
    }

    else
    {
        lowY = endY;
        highY = startY;
    }

    //determine the direction of the corridor
    if(lowY == highY)
    {
        eastWest = true;
    }

    else
    {
        eastWest = false;
    }

    if(eastWest)
    {
        int row = findGridY(lowY);
        int firstColumn = findGridX(lowX);
        int secondColumn = findGridX(highX);

        grid[firstColumn][row].edgeRight = true;
        grid[secondColumn][row].edgeLeft = true;
    }

    else
    {
        int column = findGridX(lowX);
        int firstRow = findGridY(lowY);
        int secondRow = findGridY(highY);

        grid[column][firstRow].edgeDown = true;
        grid[column][secondRow].edgeUp  = true;
    }
}

int DungeonLayout::findGridX(int x)
{
    int index = 0;

    for(list<coordRange>::iterator i = xCoords.begin(); i != xCoords.end(); i++)
    {
        if(i -> lowerBound <= x && i -> upperBound >= x)
        {
            return index;
        }

        index++;
    }

    cout << x << " not found in gridX" << endl;

    return 0;
}

int DungeonLayout::findGridY(int y)
{
    int index = 0;

    for(list<coordRange>::iterator i = yCoords.begin(); i != yCoords.end(); i++)
    {
        if(i -> lowerBound <= y && i -> upperBound >= y)
        {
            return index;
        }

        index++;
    }

    cout << y << " not found in gridY" << endl;

    return 0;
}

void DungeonLayout::asciiMap()
{

    for(int y = 0; y < yCoords.size(); y++) //for each row of rooms
    {
        //first draw the rooms and horiziontal corridors
        for(int x = 0; x < xCoords.size(); x++)
        {
            if(grid[x][y].id != -1)
            {
                cout << "#"; //symbol for a room
            }

            else
            {
                cout << " ";
            }

            if(x != xCoords.size() - 1) //if not on the last room of the row
            {
                if(grid[x][y].edgeRight)
                {
                    cout << "=";
                }

                else
                {
                    cout << " ";
                }

                if(grid[x + 1][y].edgeLeft)
                {
                    cout << "=";
                }

                else
                {
                    cout << " ";
                }
            }
        }

        cout << endl;

        if(y != yCoords.size() - 1) //if not on the last row
        {
            //draw the vertical corridors (This takes two steps, once checking edgeDown for current row, once checking edgeUp for next row
            for(int x = 0; x < xCoords.size(); x++)
            {
                if(grid[x][y].edgeDown)
                {
                    cout << "|";
                }

                else
                {
                    cout << " ";
                }

                cout << "  "; //for alignment purposes
            }

            cout << endl;
            for(int x = 0; x < xCoords.size(); x++)
            {
                if(grid[x][y + 1].edgeUp)
                {
                    cout << "|";
                }

                else
                {
                    cout << " ";
                }

                cout << "  "; //for alignment purposes
            }

            cout << endl;
        }

    }
}
