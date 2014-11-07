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
        public BaseViewModel viewModel { get; set;  }

        public LevelControl()
        {
            InitializeComponent();
        }

         private void AddStep_Click(object sender, RoutedEventArgs e)
        {
            viewModel.plan.addStep();
            LevelPlan.SelectedIndex = viewModel.plan.stepList.Count - 1;
        }

         private void ClearStep_Click(object sender, RoutedEventArgs e)
         {
             viewModel.plan.clearStep();
         }

         private void DeleteStep_Click(object sender, RoutedEventArgs e)
         {
             viewModel.plan.deleteStep();
         }

         private void LevelPlan_SelectionChanged(object sender, SelectionChangedEventArgs e)
         {
             viewModel.plan.stepIndex = LevelPlan.SelectedIndex;
         }

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
