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
        public ViewModelParent viewModel { get; set;  }

        public LevelControl()
        {
            InitializeComponent();
        }

        private void Map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            viewModel.LevelView.editLevel((int)e.GetPosition(LevelViewer).X, (int)e.GetPosition(LevelViewer).Y);

        }

         private void AddStep_Click(object sender, RoutedEventArgs e)
        {
            viewModel.LevelView.plan.addStep();
        }

         private void ClearStep_Click(object sender, RoutedEventArgs e)
         {
             viewModel.LevelView.plan.clearStep();
         }

         private void DeleteStep_Click(object sender, RoutedEventArgs e)
         {
             viewModel.LevelView.plan.deleteStep();
         }

         private void LevelPlan_SelectionChanged(object sender, SelectionChangedEventArgs e)
         {
             viewModel.LevelView.plan.stepIndex = LevelPlan.SelectedIndex;
         }

         private void BossRoom_Click(object sender, RoutedEventArgs e)
         {
             viewModel.LevelView.plan.addBossRoom();
         }

         private void TreasureRoom_Click(object sender, RoutedEventArgs e)
         {
             viewModel.LevelView.plan.addTreasureRoom();
         }

         private void Gauntlet_Click(object sender, RoutedEventArgs e)
         {
             viewModel.LevelView.plan.addGauntlet();
         }


    }
}
