using PCG_GUI.Facts;
using PCG_GUI.FlowModel;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModelParent viewModel;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new ViewModelParent();
            DataContext = viewModel;
            LevelView.viewModel = this.viewModel;
            LevelView.DataContext = viewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_New(object sender, RoutedEventArgs e)
        {
            //viewModel.newWorld();

            System.IO.StreamWriter file = new System.IO.StreamWriter("WorldDef.txt");
            FlowGraph testGraph = new FlowGraph();
            testGraph.writeFlow(file);


            file.Close();
        }

        private void Menu_Open(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension
            dlg.Filter = "PCG files (*.pcg)| *.pcg| All files (*.*)| *.*";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                viewModel.Open(filename);
            }
        }

        private void Menu_Save(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            // Set filter for file extension and default file extension
            dlg.Filter = "PCG files (*.pcg)| *.pcg";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                viewModel.BaseView.save(filename);
            }
        }

        private void Menu_Close(object sender, RoutedEventArgs e)
        {
            viewModel.Close();
        }

        private void Menu_Run(object sender, RoutedEventArgs e)
        {


            RunPCGWindow runWin = new RunPCGWindow(viewModel);
            runWin.Show();
        }

      

    }
}
