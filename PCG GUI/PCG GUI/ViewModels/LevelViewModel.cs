using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCG_GUI.Facts;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;

namespace PCG_GUI.ViewModels
{
    class LevelViewModel : INotifyPropertyChanged
    {
        public List<Shape> levelGraphic { get; private set;}
        public String X_Dimension { get; private set; }
        public String Y_Dimension { get; private set; }

        private World world;
        private int curLevel;

        public LevelViewModel()
        {
            System.Console.WriteLine("Test");

            levelGraphic = new List<Shape>();

            System.IO.StreamReader file = new System.IO.StreamReader("results.txt");

            world = new World();
            world.parseClingoFile(file);

            changeLevel(0);

            file.Close();
        }

        private void changeLevel(int level)
        {
            curLevel = level;
            X_Dimension = world.getLevel(curLevel).xDimension.ToString();
            Y_Dimension = world.getLevel(curLevel).yDimension.ToString();
            drawLevel(curLevel);

            RaisePropertyChanged("X_Dimension");
            RaisePropertyChanged("Y_Dimension");
        }

        internal void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void drawLevel(int levelNum)
        {
            levelGraphic.Clear();

            Level levelToDraw = world.getLevel(levelNum);

            for (int i = 0; i < levelToDraw.xDimension; i++)
            {
                for (int j = 0; j < levelToDraw.yDimension; j++)
                {
                    drawTile(levelToDraw.levelMap[i,j], i, j);
                }
            }

            drawWallGrid(levelToDraw);

            RaisePropertyChanged("levelGraphic");
        }
        private void drawTile(Tile t, int x, int y)
        {
            Rectangle drawnTile = new Rectangle();
            SolidColorBrush fillBrush = new SolidColorBrush();
            drawnTile.Height = 20;
            drawnTile.Width = 20;

            switch (t.tType)
            {
                case TileType.floor:
                    fillBrush.Color = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF); //white
                    break;
                case TileType.blocked:
                    fillBrush.Color = Color.FromArgb(0xFF, 0, 0, 0); //black
                    break;
                case TileType.undefined:
                    fillBrush.Color = Color.FromArgb(0xFF, 0xD3, 0xD3, 0xD3); //white
                    break;
            }

            drawnTile.Fill = fillBrush;
            levelGraphic.Add(drawnTile);
            Canvas.SetLeft(drawnTile, x * 20);
            Canvas.SetTop(drawnTile, y * 20);
        }

        private void drawWallGrid(Level levelToDraw)
        {
            //draw horizontal lines
            for (int i = 0; i < levelToDraw.xDimension; i++)
            {
                for (int j = 0; j <= levelToDraw.yDimension; j++)
                {
                    Line gridLine = new Line();

                    if ((j == levelToDraw.yDimension && levelToDraw.levelMap[i, j - 1].southWall) || (j != levelToDraw.yDimension && levelToDraw.levelMap[i, j].northWall))
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
                    levelGraphic.Add(gridLine);
                }
            }
            //draw horizontal lines
            for (int i = 0; i <= levelToDraw.xDimension; i++)
            {
                for (int j = 0; j < levelToDraw.yDimension; j++)
                {
                    Line gridLine = new Line();

                    if ((i == levelToDraw.xDimension && levelToDraw.levelMap[i - 1, j].eastWall) || (i != levelToDraw.xDimension && levelToDraw.levelMap[i, j].westWall))
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
                    levelGraphic.Add(gridLine);
                }
            }
        }


    }

}
