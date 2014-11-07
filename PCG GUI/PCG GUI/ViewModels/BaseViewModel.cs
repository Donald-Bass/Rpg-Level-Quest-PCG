/* The BaseViewModel class is poorly named. This project originally had a bigger scope that would require multiple types of views with BaseViewModel holding the code they shared in common. That
 * didn't end up happening and I was able to refactor all the code into a single class, but the original name currently remains
 * 
 * The BaseViewModel class acts as the interface between the UI and the actual data we're working with.
 *
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
using PCG_GUI.PlanModel;
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
 
        public World world;                         //Once a level is generated it will be stored in this object

        public PlanLevel plan { get; set; }         //the user generated plan for a level
       
        //these two strings are used to display the dimensions of the level
        public String X_Dimension { get; private set; }
        public String Y_Dimension { get; private set; }

        //while these hold the actual dimensions
        public int X_Length { get; private set; }
        public int Y_Length { get; private set; }


        public BaseViewModel()
        {
            world = null; //there is no world at the start
            levelGraphic = new SmartObservableCollection<System.Windows.FrameworkElement>();
            plan = new PlanLevel();

            X_Dimension = "";
            X_Length = 0;
            Y_Dimension = "";
            Y_Length = 0;


            //worldAttached = false;
        }

        //open the world contained in Filename
        public void open(string Filename)
        {
            if (world != null) //if we have a wor
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
            plan.clear();
 
            //RaisePropertyChanged("worldAttached");

            //remove any saved dimensions of the level

            //remove all stored values
            X_Dimension = "";
            Y_Dimension = "";
            X_Length = 0;
            Y_Length = 0;

            //tell gui values have been removed
            RaisePropertyChanged("X_Dimension");
            RaisePropertyChanged("Y_Dimension");
        }


        //The next several functions handle drawing the map of the level displayed in the gui

        //I honestly have no idea why this function exists instead of drawLevelBody being called directly. I really should have documented some of this
        //code while it was fresh in my mind rather then a year later
        public void drawLevel()
        {
            drawLevelBody(levelGraphic);
        }

        //This is the main function for drawing the map
        public void drawLevelBody(SmartObservableCollection<System.Windows.FrameworkElement> graphic)
        {
            if (world != null) //sanity check to make sure there is a level to draw
            {
                graphic.Clear(); //remove any previous drawings

                List<Shape> toDraw = new List<Shape>();  //start a list of shapes to draw
                List<TextBlock> allRoomNums = new List<TextBlock>(); //start a list of room numbers to place

                Level levelToDraw = world.getLevel(); //get the actual level

                //for each tile draw the tile
                for (int i = 0; i < levelToDraw.xDimension; i++)
                {
                    for (int j = 0; j < levelToDraw.yDimension; j++)
                    {
                        drawTile(levelToDraw.levelMap[i, j], i, j, toDraw, allRoomNums);
                    }
                }

                //add in gridlines
                drawWallGrid(levelToDraw, toDraw);

                graphic.AddRange(toDraw); //add all the shapes to the actual graphic
                graphic.AddRange(allRoomNums); //add the room numbers to the graphic as well

            }
        }

        //This handles drawing the specific tile t, located at coordinates x,y in the level
        private void drawTile(Tile t, int x, int y, List<Shape> toDraw, List<TextBlock> allRoomNums)
        {
            //create a shape to represent the tile
            Rectangle drawnTile = new Rectangle();
            SolidColorBrush fillBrush = new SolidColorBrush();
            drawnTile.Height = GRID_SIZE;
            drawnTile.Width = GRID_SIZE;

            switch (t.tType) //pick a color for the tile based on it's type. Many of these are placeholders but that should be fixed by the time the code is handed over
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

            //fill the tile with the chosen color
            drawnTile.Fill = fillBrush;
            Canvas.SetLeft(drawnTile, x * GRID_SIZE); //set the coordinates. The coordinates are soley the coordinates within the canvas being drawn, so the upper left? of the canvas is 0,0 and the math is simple
            Canvas.SetTop(drawnTile, y * GRID_SIZE);            
            toDraw.Add(drawnTile); //save the tile to be drawn

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

        //Draws grid lines on the level graphic to make it clear where each tile starts and ends
        private void drawWallGrid(Level levelToDraw, List<Shape> toDraw)
        {
            //draw horizontal lines
            for (int i = 0; i < levelToDraw.xDimension; i++) //for each column
            {
                for (int j = 0; j <= levelToDraw.yDimension; j++) //for each row
                {
                    //draw a line from one edge of the column to the other
                    Line gridLine = new Line();

                    gridLine.Stroke = System.Windows.Media.Brushes.Gray;
                    gridLine.StrokeThickness = 1;
                  
                    gridLine.X1 = i * GRID_SIZE;
                    gridLine.X2 = i * GRID_SIZE + GRID_SIZE;
                    gridLine.Y1 = j * GRID_SIZE;
                    gridLine.Y2 = j * GRID_SIZE;
                    toDraw.Add(gridLine);
                }
            }
            //draw vertical lines lines
            for (int i = 0; i <= levelToDraw.xDimension; i++) //for each column
            {
                for (int j = 0; j < levelToDraw.yDimension; j++) //for each row
                {
                    //draw a line from the top of the row to the bottom
                    Line gridLine = new Line(); 

                    gridLine.Stroke = System.Windows.Media.Brushes.Gray;
                    gridLine.StrokeThickness = 1;

                    gridLine.X1 = i * GRID_SIZE;
                    gridLine.X2 = i * GRID_SIZE;
                    gridLine.Y1 = j * GRID_SIZE;
                    gridLine.Y2 = j * GRID_SIZE + GRID_SIZE;
                    toDraw.Add(gridLine);
                }
            }
        }

        //This code allows the GUI to call and run Clingo to generate a level
        public void runClingo()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("TempWorldDef.txt"); //create a text file to store the rules we generate to feed to Clingo

            plan.writePlan(file); //write the necessary ru;es
            file.Close();

            //get the unix time to use as a seed for Clingos RNG
            String unixTimestamp = ((Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds).ToString();

            //start a process that calls Clingo from the command line, and wait for the process to exit before continuing.
            //This will currently completly freeze the GUI. It would be nice if it didn't but that's not a priority
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
