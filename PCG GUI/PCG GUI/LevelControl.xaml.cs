using PCG_GUI.Facts;
using PCG_GUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PCG_GUI
{
    /// <summary>
    /// Interaction logic for LevelControl.xaml
    /// </summary>
    public partial class LevelControl : UserControl
    {
        public BaseViewModel viewModel { get; set;  } //this is essential. This is the object containing all of the actual data and that serves as the interface 
                                                     //between that data and the GUI. This variable is set by the MainWindow 

        public LevelControl()
        {
            InitializeComponent();
        }

        //Below is code for the user to interaction with the levelPlan

        //add a step to the plan
         private void AddStep_Click(object sender, RoutedEventArgs e)
        {
            viewModel.plan.addStep();
            LevelPlan.SelectedIndex = viewModel.plan.stepList.Count - 1; //set the selected step of the plan to the new step
        }

        //remove all rooms from the current step of the plan
         private void ClearStep_Click(object sender, RoutedEventArgs e)
         {
             viewModel.plan.clearStep();
         }

         //remove a step from the plan
         private void DeleteStep_Click(object sender, RoutedEventArgs e)
         {
             viewModel.plan.deleteStep();
         }

         //Function called when the user changes the step they have selected
         private void LevelPlan_SelectionChanged(object sender, SelectionChangedEventArgs e)
         {
             viewModel.plan.stepIndex = LevelPlan.SelectedIndex; //let the plan know that the current step in the user's mind has changed
         }

         //add different types of rooms to the current step of the plan

         private void BossRoom_Click(object sender, RoutedEventArgs e)
         {
             viewModel.plan.addBossRoom();
         }

         private void TreasureRoom_Click(object sender, RoutedEventArgs e)
         {
             viewModel.plan.addTreasureRoom();
         }

         private void Gauntlet_Click(object sender, RoutedEventArgs e)
         {
             viewModel.plan.addGauntlet();
         }


    }
}
