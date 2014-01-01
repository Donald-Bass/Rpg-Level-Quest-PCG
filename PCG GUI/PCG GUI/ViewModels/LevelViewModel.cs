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
using System.Collections.ObjectModel;

namespace PCG_GUI.ViewModels
{
    class LevelViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Shape> levelGraphic { get; private set; }
        public ObservableCollection<levelData> levelList { get; private set; }
        public String X_Dimension { get; private set; }
        public String Y_Dimension { get; private set; }
        public int NumberOfLevels { get; private set; } //TODO: refactor this out and replace with references to number of levels in world?

        public Boolean levelInterior { get; private set;}


        public Boolean levelExterior { get; private set; }

        public Boolean worldAttached { get; private set; } //is there a world attached to the view model

        private World world;
        private int curLevel;
        public int selectedLevel  {
            get 
            {
                return curLevel; 
            }
            set 
            {
                 curLevel = value;
                 changeLevel(value);
                 RaisePropertyChanged("selectedLevel");

            }
        }
        

        public LevelViewModel()
        {
            NumberOfLevels = 0;
            world = null;
            levelGraphic = new ObservableCollection<Shape>();
            levelList = new ObservableCollection<levelData>();
            X_Dimension = "";
            Y_Dimension = "";
            selectedLevel = -1;

            levelInterior = false;
            levelExterior = false;
            worldAttached = false;
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

            if (NumberOfLevels >= 1) //if there is at least one level
            {
                selectedLevel = 0; //open the first level by default
            }
            file.Close();

            setUpLevelList();
            worldAttached = true;
            RaisePropertyChanged("worldAttached");
        }

        public void setUpLevelList()
        {
            if(world != null)
            {
                levelList.Clear();

                for(int i = 0; i < world.numLevels; i++)
                {
                    levelData curLevelData = new levelData();
                    curLevelData.levelName = world.getLevel(i).levelName;
                    curLevelData.levelNumber = i;
                    levelList.Add(curLevelData);
                }
            }
        }

        public void save(string Filename)
        {
            if (world != null)
            {           
                System.IO.StreamWriter file = new System.IO.StreamWriter(Filename);
                world.writeClingoFile(file);
                file.Close();
            }

        }

        public void newWorld()
        {
            closeWorld(); //close the previous world
            world = new World();
            worldAttached = true;
            NumberOfLevels = 0;
            RaisePropertyChanged("worldAttached");
        }

        public void changeLevel(int level)
        {
            if (world != null && level != -1) //sanity check
            {

                if(level < NumberOfLevels) //if the level to change to actually exists
                {

                //curLevel = level;
                X_Dimension = world.getLevel(curLevel).xDimension.ToString();
                Y_Dimension = world.getLevel(curLevel).yDimension.ToString();
                drawLevel(curLevel);

                if(world.getLevel(curLevel).typeOfLevel == levelType.interior)
                {
                    levelInterior = true;
                    levelExterior = false;
                }
                    
                else
                {
                    levelInterior = false;
                    levelExterior = true;

                }

                RaisePropertyChanged("X_Dimension");
                RaisePropertyChanged("Y_Dimension");
                RaisePropertyChanged("levelInterior");
                RaisePropertyChanged("levelExterior");
                }
            }
        }

        internal void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void setLevelType(levelType type)
        {
            if (selectedLevel != -1)
            {

                if (type == levelType.interior)
                {
                    levelInterior = true;
                    levelExterior = false;
                }

                else
                {
                    levelInterior = false;
                    levelExterior = true;
                }

                world.getLevel(selectedLevel).typeOfLevel = type;

                RaisePropertyChanged("levelInterior");
                RaisePropertyChanged("levelExterior");

            }
        }

        //Removes the world model attached to the view model
        public void closeWorld()
        {
            //remove all stored values
            world = null;
            curLevel = -1;
            NumberOfLevels = 0;
            X_Dimension = "";
            Y_Dimension = "";
            levelInterior = false;
            levelExterior = false;

            levelGraphic.Clear();
            levelList.Clear();

            //tell gui values have been removed
            RaisePropertyChanged("X_Dimension");
            RaisePropertyChanged("Y_Dimension");
            RaisePropertyChanged("levelInterior");
            RaisePropertyChanged("levelExterior");

            worldAttached = false;
            RaisePropertyChanged("worldAttached");

        }

        private void drawLevel(int levelNum)
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

        //level display list type stuff
        public class levelData
        {
            public int levelNumber { get; set; }
            public string levelName { get; set; }
        }


        public void addLevel(int x, int y)
        {
            //validate numbers here eventually
            world.addLevel(x, y);
            NumberOfLevels++;

            levelData newLevelData = new levelData();
            newLevelData.levelNumber = NumberOfLevels - 1;
            newLevelData.levelName = "";
            levelList.Add(newLevelData);
            selectedLevel = NumberOfLevels - 1; //select the new level
        }

    }

}
