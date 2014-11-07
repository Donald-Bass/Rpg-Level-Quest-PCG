/*This class was used originally to power a window that popped up when runPCG was selected in the menu to get additional input. It is no longer necessary
 * 
 * using PCG_GUI.ViewModels;
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
using System.Windows.Shapes;

namespace PCG_GUI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class RunPCGWindow : Window
    {
        public ViewModelParent viewModel {get; set;}
        public RunPCGWindow(ViewModelParent viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            int numLevels = -1; //by default no number of levels will be specified

            try
            {
                if (numLevelsEnabled.IsChecked.HasValue && (bool)numLevelsEnabled.IsChecked)
                {
                    numLevels = Convert.ToInt32(numLevelsField.Text);
                }
            }
            catch (FormatException exception)
            {
                System.Console.WriteLine(exception.ToString());
            }
            //viewModel.runClingo(numLevels);
            this.Close();
        }


    }
}
*/