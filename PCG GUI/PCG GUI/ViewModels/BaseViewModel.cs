/* The BaseViewModel class is unfortantly not very distinct from the LevelViewModel class. The original plans for this were very opened and far too ambitious including multiple levels, npcs, and quests. The
 * level view model would have all the code needed specifically for viewing an individual level, while the BaseViewModel would have the code all the different types of views would need. Unfortantly the 
 * scope ultimately changed to generating a single level so the code needed to interface with the GUI is essentially randomly scattered across the two
 */ 
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
        public const int GRID_SIZE = 20; //size of the squares in the grid in the map of the level (in pixels)

        public SmartObservableCollection<System.Windows.FrameworkElement> levelGraphic { get;  private set; }

        //public Boolean worldAttached { get; set; } //is there a world attached to the view model. This was intended to allow
                                                     //the gui to be open without a world actually being generated, and prevent the gui from doing
                                                     //anything till the user goes to menu and hits new. This is stupid. I've disabled this and programed the gui to automatically
                                                     //create an empty PlanLevel to start off with, and I've disabled the Close menu option so there will never be nothing to interact with
        public World world;

        public BaseViewModel()
        {
            world = null;
            levelGraphic = new SmartObservableCollection<System.Windows.FrameworkElement>();
            //worldAttached = false;
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

            file.Close();

            //worldAttached = true;
            RaisePropertyChanged("worldAttached");

            drawLevel();
        }


        public void save(string Filename) //saves the current world to a file. This doesn't really work right currently, but it should have been fixed by the time the code is turned over
        {
            if (world != null)
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(Filename);
                world.writeWorldFile(file);
                file.Close();
            }

        }

        //creates a new world
        internal void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;


        //Removes the world model attached to the view model
        public void closeWorld()
        {
            //remove all stored values
            world = null;

            levelGraphic.Clear();

            RaisePropertyChanged("worldAttached");
        }


        public void drawLevel()
        {
            drawLevelBody(levelGraphic);
        }

        public void drawLevelBody(SmartObservableCollection<System.Windows.FrameworkElement> graphic)
        {
            if (world != null) //sanity check
            {
                graphic.Clear();

                List<Shape> toDraw = new List<Shape>();
                List<TextBlock> allRoomNums = new List<TextBlock>();

                Level levelToDraw = world.getLevel();

                for (int i = 0; i < levelToDraw.xDimension; i++)
                {
                    for (int j = 0; j < levelToDraw.yDimension; j++)
                    {
                        drawTile(levelToDraw.levelMap[i, j], i, j, toDraw, allRoomNums);
                    }
                }

                drawWallGrid(levelToDraw, toDraw);

                graphic.AddRange(toDraw);
                graphic.AddRange(allRoomNums);

            }
        }
        private void drawTile(Tile t, int x, int y, List<Shape> toDraw, List<TextBlock> allRoomNums)
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
                case TileType.wall:
                    fillBrush.Color = Color.FromArgb(0xFF, 0, 0, 0); //black
                    //fillBrush.Color = Color.FromArgb(0xFF, 0xD3, 0xD3, 0xD3); //grey
                    break;
                case TileType.undefined: 
                    fillBrush.Color = Color.FromArgb(0xFF, 0xFF, 0x69, 0xB4); //hot pink. Nothing should be undefined so make that case stand out
                    break;
                case TileType.door:
                    fillBrush.Color = Color.FromArgb(0xFF, 0xA5, 0x2A, 0x2A); //brown
                    break;
                case TileType.treasureRoom:
                    fillBrush.Color = Color.FromArgb(0xFF, 0xFF, 0xFF, 0x00); //yellow
                    break;
                case TileType.arena:
                    fillBrush.Color = Color.FromArgb(0xFF, 0xFF, 0, 0); //yellow
                    break;

            }

            drawnTile.Fill = fillBrush;
            Canvas.SetLeft(drawnTile, x * GRID_SIZE);
            Canvas.SetTop(drawnTile, y * GRID_SIZE);            
            toDraw.Add(drawnTile);

            //temp code to display room numbers of each tile for debugging purposes
            if (t.RoomNumber != -1)
            {
                TextBlock roomNumber = new TextBlock();
                roomNumber.Text = t.RoomNumber.ToString();
                Canvas.SetLeft(roomNumber, x * GRID_SIZE + 2);
                Canvas.SetTop(roomNumber, y * GRID_SIZE + 2);
                allRoomNums.Add(roomNumber);
            }

        }

        private void drawWallGrid(Level levelToDraw, List<Shape> toDraw)
        {
            //draw horizontal lines
            for (int i = 0; i < levelToDraw.xDimension; i++)
            {
                for (int j = 0; j <= levelToDraw.yDimension; j++)
                {
                    Line gridLine = new Line();

                    if ((j == levelToDraw.yDimension && levelToDraw.levelMap[i, j - 1].southWall == WallType.wall) || (j != levelToDraw.yDimension && levelToDraw.levelMap[i, j].northWall == WallType.wall))
                    {
                        gridLine.Stroke = System.Windows.Media.Brushes.Black;
                        gridLine.StrokeThickness = 2;
                    }

                    else if ((j == levelToDraw.yDimension && levelToDraw.levelMap[i, j - 1].southWall == WallType.door) || (j != levelToDraw.yDimension && levelToDraw.levelMap[i, j].northWall == WallType.door))
                    {
                        gridLine.Stroke = System.Windows.Media.Brushes.Brown;
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
                    toDraw.Add(gridLine);
                }
            }
            //draw horizontal lines
            for (int i = 0; i <= levelToDraw.xDimension; i++)
            {
                for (int j = 0; j < levelToDraw.yDimension; j++)
                {
                    Line gridLine = new Line();

                    if ((i == levelToDraw.xDimension && levelToDraw.levelMap[i - 1, j].eastWall == WallType.wall) || (i != levelToDraw.xDimension && levelToDraw.levelMap[i, j].westWall == WallType.wall))
                    {
                        gridLine.Stroke = System.Windows.Media.Brushes.Black;
                        gridLine.StrokeThickness = 2;
                    }

                    else if ((i == levelToDraw.xDimension && levelToDraw.levelMap[i - 1, j].eastWall == WallType.door) || (i != levelToDraw.xDimension && levelToDraw.levelMap[i, j].westWall == WallType.door))
                    {
                        gridLine.Stroke = System.Windows.Media.Brushes.Brown;
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
                    toDraw.Add(gridLine);
                }
            }
        }

        //the body of the runClingo code
        //numberOfLevels - number of levels to generate. If -1 generate as many as clingo feels is necessary
        public void runClingo(PCG_GUI.PlanModel.PlanLevel plan)
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("TempWorldDef.txt");

            plan.writePlan(file);
            file.Close();

            //get the unix time to use as a seed
            String unixTimestamp = ((Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();


            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C clingo.exe PCG.txt TempWorldDef.txt --seed=" + unixTimestamp + " --rand-freq .01 > TempResults.pcg";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
    }

}
