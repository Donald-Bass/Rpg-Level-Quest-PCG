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
using System.Diagnostics;

namespace PCG_GUI.ViewModels
{
    public class LevelViewModel : INotifyPropertyChanged
    {
        private BaseViewModel baseView;

        public ObservableCollection<levelData> levelList { get; private set; }
        public String X_Dimension { get; private set; }
        public String Y_Dimension { get; private set; }

        public Boolean levelInterior { get; private set;}


        public Boolean levelExterior { get; private set; }


        //map editing modes
        public Boolean editFloorTrueValue = false;
        public Boolean editFloor
        {
            get
            {
                return editFloorTrueValue;
            }
            set
            {
                editFloorTrueValue = value;
                RaisePropertyChanged("editFloor");
            }
        }
        public Boolean addWalls { get; set; }
        public Boolean removeWalls { get; set; }
        public Boolean floorTiles { get; set; }
        public Boolean blockedTiles { get; set; }
        public Boolean undefinedTiles { get; set; }

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
        

        public LevelViewModel(BaseViewModel baseView)
        {
            this.baseView = baseView;

            levelList = new ObservableCollection<levelData>();
            X_Dimension = "";
            Y_Dimension = "";
            selectedLevel = -1;

            levelInterior = false;
            levelExterior = false;
        }

        public void setUpLevelList()
        {
            if (baseView.world != null)
            {
                levelList.Clear();

                for (int i = 0; i < baseView.world.numLevels; i++)
                {
                    levelData curLevelData = new levelData();
                    curLevelData.levelName = baseView.world.getLevel(i).levelName;
                    curLevelData.levelNumber = i;
                    levelList.Add(curLevelData);
                }
            }
        }

        public void finishOpen()
        {
            if (baseView.NumberOfLevels >= 1) //if there is at least one level
            {
                selectedLevel = 0; //open the first level by default
            }

            setUpLevelList();
        }

        public void changeLevel(int level)
        {
            if (baseView.world != null && level != -1) //sanity check
            {

                if (level < baseView.NumberOfLevels) //if the level to change to actually exists
                {

                //curLevel = level;
                X_Dimension = baseView.world.getLevel(curLevel).xDimension.ToString();
                Y_Dimension = baseView.world.getLevel(curLevel).yDimension.ToString();
                baseView.drawLevel(curLevel);

                if (baseView.world.getLevel(curLevel).typeOfLevel == levelType.interior)
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

                baseView.world.getLevel(selectedLevel).typeOfLevel = type;

                RaisePropertyChanged("levelInterior");
                RaisePropertyChanged("levelExterior");

            }
        }

        //Removes the world model attached to the view model
        public void finishClose() 
        {
            //remove all stored values
            curLevel = -1;
            X_Dimension = "";
            Y_Dimension = "";
            levelInterior = false;
            levelExterior = false;

            levelList.Clear();

            //tell gui values have been removed
            RaisePropertyChanged("X_Dimension");
            RaisePropertyChanged("Y_Dimension");
            RaisePropertyChanged("levelInterior");
            RaisePropertyChanged("levelExterior");

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
            baseView.world.addLevel(x, y);
            baseView.NumberOfLevels++;

            levelData newLevelData = new levelData();
            newLevelData.levelNumber = baseView.NumberOfLevels - 1;
            newLevelData.levelName = "";
            levelList.Add(newLevelData);
            selectedLevel = baseView.NumberOfLevels - 1; //select the new level
        }


        public void editLevel(int x, int y)
        {
            if (baseView.worldAttached && selectedLevel != -1) //only edit when a level actually is selected
            {

                if (editFloor)
                {
                    int tileX = x / BaseViewModel.GRID_SIZE;
                    int tileY = y / BaseViewModel.GRID_SIZE;
                    
                    if(floorTiles)
                    {
                        baseView.world.getLevel(selectedLevel).setTileType(tileX, tileY, TileType.floor);
                    }

                    else if(blockedTiles)
                    {
                        baseView.world.getLevel(selectedLevel).setTileType(tileX, tileY, TileType.blocked);
                    }

                    else if (undefinedTiles)
                    {
                        baseView.world.getLevel(selectedLevel).setTileType(tileX, tileY, TileType.undefined);
                    }
                }

                else //editing walls
                {
                    if ((x % BaseViewModel.GRID_SIZE <= 3 || x % BaseViewModel.GRID_SIZE >= BaseViewModel.GRID_SIZE - 3) && !(y % BaseViewModel.GRID_SIZE <= 3 || y % BaseViewModel.GRID_SIZE >= BaseViewModel.GRID_SIZE - 3)) //if close to a wall along the y axis and not to a wall along the x axis
                    {
                        int wallX = (int)Math.Round((double)x / (double)BaseViewModel.GRID_SIZE);
                        int wallY = (int)Math.Floor((double)y / (double)BaseViewModel.GRID_SIZE);

                        if(addWalls)
                        {
                            baseView.world.getLevel(selectedLevel).addWallY(wallX, wallY);
                        }

                        else if(removeWalls)
                        {
                            baseView.world.getLevel(selectedLevel).removeWallY(wallX, wallY);
                        }
                    }

                    else if (!(x % BaseViewModel.GRID_SIZE <= 3 || x % BaseViewModel.GRID_SIZE >= BaseViewModel.GRID_SIZE - 3) && (y % BaseViewModel.GRID_SIZE <= 3 || y % BaseViewModel.GRID_SIZE >= BaseViewModel.GRID_SIZE - 3)) //if close to a wall along the x axis and not to a wall along the y axis
                    {
                        int wallX = (int)Math.Floor((double)x / (double)BaseViewModel.GRID_SIZE);
                        int wallY = (int)Math.Round((double)y / (double)BaseViewModel.GRID_SIZE);

                        if (addWalls)
                        {
                            baseView.world.getLevel(selectedLevel).addWallX(wallX, wallY);
                        }

                        else if (removeWalls)
                        {
                            baseView.world.getLevel(selectedLevel).removeWallY(wallX, wallY);
                        }
                    }
                    
                }

                baseView.drawLevel(selectedLevel); //redraw the level
            }
        }

        //numberOfLevels - number of levels to generate. If -1 generate as many as clingo feels is necessary
        //public override void runClingo(int numberOfLevels)
        //{
            //runClingoBody(numberOfLevels);
            //open("TempResults.pcg");
        //}

        internal void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

}
