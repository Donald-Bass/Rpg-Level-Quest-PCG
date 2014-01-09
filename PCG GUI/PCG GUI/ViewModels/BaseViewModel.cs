using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using PCG_GUI.Facts;
using System.Windows.Media;
using System.Windows.Controls;


namespace PCG_GUI.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public const int GRID_SIZE = 20;
        public const int WALL_PIXEL_RANGE = 3; //Range from wall at which a click registers

        public ObservableCollection<Shape> levelGraphic { get;  set; }
        public Boolean worldAttached { get; set; } //is there a world attached to the view model
        public World world;
        public int NumberOfLevels { get;  set; } //TODO: refactor this out and replace with references to number of levels in world?


        public BaseViewModel()
        {
            world = null;
            levelGraphic = new ObservableCollection<Shape>();
            worldAttached = false;
        }

        public virtual void openView() //handles any additional steps needed for opening a file for a specific view
        {
            
        }

        //open the world contained in Filename
        public void open(string Filename)
        {
            if (world != null)
            {
                closeWorld(); //close the world currently open
            }

            //load the new world
            world = new World();
            System.IO.StreamReader file = new System.IO.StreamReader(Filename);
            world.parseClingoFile(file);

            NumberOfLevels = world.numLevels;

            file.Close();

            worldAttached = true;
            RaisePropertyChanged("worldAttached");
        }


        public void save(string Filename)
        {
            if (world != null)
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(Filename);
                world.writeWorldFile(file);
                file.Close();
            }

        }

        public void newWorld()
        {
            world = new World();
            worldAttached = true;
            NumberOfLevels = 0;
            RaisePropertyChanged("worldAttached");
        }

        internal void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void closeWorldView() //function handles any additional cleanup the specific view model needs when a world is closed
        {

        }

        //Removes the world model attached to the view model
        public void closeWorld()
        {
            //remove all stored values
            world = null;
            NumberOfLevels = 0;

            levelGraphic.Clear();

            worldAttached = false;
            RaisePropertyChanged("worldAttached");
        }




        public void drawLevel(int levelNum)
        {
            if (world != null) //sanity check
            {
                System.Console.WriteLine("Test");

                levelGraphic.Clear();

                Level levelToDraw = world.getLevel(levelNum);

                for (int i = 0; i < levelToDraw.xDimension; i++)
                {
                    for (int j = 0; j < levelToDraw.yDimension; j++)
                    {
                        drawTile(levelToDraw.levelMap[i, j], i, j);
                    }
                }

                drawWallGrid(levelToDraw);
            }
        }
        private void drawTile(Tile t, int x, int y)
        {
            Rectangle drawnTile = new Rectangle();
            SolidColorBrush fillBrush = new SolidColorBrush();
            drawnTile.Height = GRID_SIZE;
            drawnTile.Width = GRID_SIZE;

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
            Canvas.SetLeft(drawnTile, x * GRID_SIZE);
            Canvas.SetTop(drawnTile, y * GRID_SIZE);
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

                    gridLine.X1 = i * GRID_SIZE;
                    gridLine.X2 = i * GRID_SIZE + GRID_SIZE;
                    gridLine.Y1 = j * GRID_SIZE;
                    gridLine.Y2 = j * GRID_SIZE;
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

                    gridLine.X1 = i * GRID_SIZE;
                    gridLine.X2 = i * GRID_SIZE;
                    gridLine.Y1 = j * GRID_SIZE;
                    gridLine.Y2 = j * GRID_SIZE + GRID_SIZE;
                    levelGraphic.Add(gridLine);
                }
            }
        }

        //the body of the runClingo code
        //numberOfLevels - number of levels to generate. If -1 generate as many as clingo feels is necessary
        public void runClingo(int numberOfLevels)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("TempWorldDef.txt");

            world.writeClingoInputFile(file, numberOfLevels);

            file.Close();

            //get the unix time to use as a seed
            String unixTimestamp = ((Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();


            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C clingo.exe PCG.txt TempWorldDef.txt --seed=" + unixTimestamp + " --rand-freq .1 > TempResults.pcg";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
    }

}
