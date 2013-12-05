using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;



namespace PCG_GUI.Facts
{
    enum levelType{interior, exterior};

    class Level
    {
        private int xDimension; //length of level (x)
        private int yDimension; //height of level (y)

        private Tile[,] levelMap; //all the tiles making up the level[x,y]. 0,0 is upper left
        public levelType typeOfLevel { get; set; }

        public string levelName; //TODO FIGURE OUT NAME GEN

        public Level(int xDim, int yDim)
        {
            xDimension = xDim;
            yDimension = yDim;
            levelMap = new Tile[xDim,yDim];

            for(int i = 0; i < xDimension; i++)
            {
                for(int j = 0; j < yDimension; j++)
                {
                    levelMap[i, j] = new Tile();
                }
            }

            typeOfLevel = levelType.interior; //default to interior for now

        }

        public void setTileType(int x, int y, TileType type)
        {
            levelMap[x, y].tType = type;
        }

        public void addWallX(int x, int y)
        {
           //the wall being added goes from X,Y to X+1,Y

          //if not at the very top of the map
          if(y != 0)
          {
              levelMap[x, y - 1].southWall = true;
          }

          if (y != yDimension)
          {
              levelMap[x, y].northWall = true;
          }
        }

        public void addWallY(int x, int y)
        {
            //the wall being added goeX from X,Y to X+1,Y

            //if not at the very top of the map
            if (x != 0)
            {
                levelMap[x - 1, y].eastWall = true;
            }

            if (x != xDimension)
            {
                levelMap[x, y].westWall = true;
            }
        }
        
        //covert all undefined tiles into blocked tiles
        public void finalizeLevel()
        {
            for (int i = 0; i < xDimension; i++)
            {
                for (int j = 0; j < yDimension; j++)
                {
                    if(levelMap[i,j].tType == TileType.undefined)
                    {
                        levelMap[i, j].tType = TileType.blocked;
                    }
                }
            }
        }

        public void drawLevel(Canvas c)
        {
            c.Children.Clear();

            for (int i = 0; i < xDimension; i++)
            {
                for (int j = 0; j < yDimension; j++)
                {
                    drawTile(i, j, c);
                }
            }

            drawWallGrid(c);

        }

        private void drawWallGrid(Canvas c)
        {
            //draw horizontal lines
            for (int i = 0; i < xDimension; i++)
            {
                for (int j = 0; j <= yDimension; j++)
                {
                    Line gridLine = new Line();

                    if ((j == yDimension && levelMap[i, j - 1].southWall) || (j != yDimension && levelMap[i, j].northWall))
                    {
                        gridLine.Stroke = System.Windows.Media.Brushes.Black;
                        gridLine.StrokeThickness = 2;
                    }
                    else
                    {
                        gridLine.Stroke = System.Windows.Media.Brushes.Gray;
                        gridLine.StrokeThickness = 1;
                    }

                    gridLine.X1 = i * 20;
                    gridLine.X2 = i * 20 + 20;
                    gridLine.Y1 = j * 20;
                    gridLine.Y2 = j * 20;
                    c.Children.Add(gridLine);
                }
            }
            //draw horizontal lines
            for (int i = 0; i <= xDimension; i++)
            {
                for (int j = 0; j < yDimension; j++)
                {
                    Line gridLine = new Line();

                    if ((i == xDimension && levelMap[i - 1, j].eastWall) || (i != xDimension && levelMap[i, j].westWall))
                    {
                        gridLine.Stroke = System.Windows.Media.Brushes.Black;
                        gridLine.StrokeThickness = 2;
                    }
                    else
                    {
                        gridLine.Stroke = System.Windows.Media.Brushes.Gray;
                        gridLine.StrokeThickness = 1;
                    }

                    gridLine.X1 = i * 20;
                    gridLine.X2 = i * 20;
                    gridLine.Y1 = j * 20;
                    gridLine.Y2 = j * 20 + 20;
                    c.Children.Add(gridLine);
                }
            }
        }

        private void drawTile(int x, int y, Canvas c)
        {
            Rectangle drawnTile = new Rectangle();
            SolidColorBrush fillBrush = new SolidColorBrush();
            drawnTile.Height = 20;
            drawnTile.Width = 20;



            switch(levelMap[x,y].tType)
            {
                case TileType.floor:
                    fillBrush.Color = Color.FromArgb(0xFF,0xFF,0xFF,0xFF); //white
                    break;
                case TileType.blocked:
                    fillBrush.Color = Color.FromArgb(0xFF,0,0,0); //black
                    break;
                case TileType.undefined:
                    fillBrush.Color = Color.FromArgb(0xFF, 0xD3, 0xD3, 0xD3); //white
                    break;
            }

            drawnTile.Fill = fillBrush;
            c.Children.Add(drawnTile);
            Canvas.SetLeft(drawnTile, x*20);
            Canvas.SetTop(drawnTile,y*20);
        }
    }
}
