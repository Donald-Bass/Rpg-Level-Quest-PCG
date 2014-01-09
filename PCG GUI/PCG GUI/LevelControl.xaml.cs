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
        public ViewModelParent viewModel;

        public LevelControl()
        {
            InitializeComponent();
        }

        private void InteriorButton_Checked(object sender, RoutedEventArgs e)
        {
            viewModel.LevelView.setLevelType(levelType.interior);
        }

        private void ExteriorButton_Checked(object sender, RoutedEventArgs e)
        {
            viewModel.LevelView.setLevelType(levelType.exterior);
        }

        private void CreateLevel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                viewModel.LevelView.addLevel(Convert.ToInt32(NewLevelSizeX.Text), Convert.ToInt32(NewLevelSizeY.Text));
            }
            catch (FormatException exception)
            {
                System.Console.WriteLine(exception.ToString());
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            viewModel.LevelView.editLevel((int)e.GetPosition(LevelViewer).X, (int)e.GetPosition(LevelViewer).Y);

        }
    }
}
