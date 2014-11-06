/* The LevelViewModel class is unfortantly not very distinct from the BaseViewModel class. The original plans for this were very opened and far too ambitious including multiple levels, npcs, and quests. The
 * level view model would have all the code needed specifically for viewing an individual level, while the BaseViewModel would have the code all the different types of views would need. Unfortantly the 
 * scope ultimately changed to generating a single level so the code needed to interface with the GUI is essentially randomly scattered across the two
 */ 

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
using PCG_GUI.PlanModel;

namespace PCG_GUI.ViewModels
{
    public class LevelViewModel : INotifyPropertyChanged
    {
        private BaseViewModel baseView;

        //public ObservableCollection<levelData> levelList { get; private set; }
        public String X_Dimension { get; private set; }
        public String Y_Dimension { get; private set; }

        public int X_Length { get; private set; }
        public int Y_Length { get; private set; }


        public PlanLevel plan { get; set; }

        private int selectedLevel { get; set; }
        

        public LevelViewModel(BaseViewModel baseView)
        {
            this.baseView = baseView;

            X_Dimension = "";
            X_Length = 0;
            Y_Dimension = "";
            Y_Length = 0;
            selectedLevel = -1;

            plan = new PlanLevel();
        }

        /*public void changeLevel(int level)
        {
            if (baseView.world != null && level != -1) //sanity check
            {

                if (level < baseView.NumberOfLevels) //if the level to change to actually exists
                {

                //curLevel = level;
                X_Dimension = baseView.world.getLevel(curLevel).xDimension.ToString();
                X_Length = baseView.world.getLevel(curLevel).xDimension * 10;
                Y_Dimension = baseView.world.getLevel(curLevel).yDimension.ToString();
                Y_Length = baseView.world.getLevel(curLevel).yDimension * 10;
                baseView.drawLevel(curLevel);

                /*if (baseView.world.getLevel(curLevel).typeOfLevel == levelType.interior)
                {
                    levelInterior = true;
                    levelExterior = false;
                }
                    
                else
                {
                    levelInterior = false;
                    levelExterior = true;

                }*/
                /*
                RaisePropertyChanged("X_Dimension");
                RaisePropertyChanged("X_Length");
                RaisePropertyChanged("Y_Length");
                RaisePropertyChanged("Y_Dimension");
                }
            }
        }*/


        //Removes the world model attached to the view model
        public void finishClose() 
        {
            //remove all stored values
            X_Dimension = "";
            Y_Dimension = "";
            X_Length = 0;
            Y_Length = 0;

            //tell gui values have been removed
            RaisePropertyChanged("X_Dimension");
            RaisePropertyChanged("Y_Dimension");
            RaisePropertyChanged("X_Length");
            RaisePropertyChanged("Y_Length");

            plan.clear();
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
