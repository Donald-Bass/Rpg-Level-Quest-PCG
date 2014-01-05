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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LevelViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new LevelViewModel();
            DataContext = viewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_New(object sender, RoutedEventArgs e)
        {
            viewModel.newWorld();
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
                viewModel.open(filename);
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
                viewModel.save(filename);
            }
        }

        private void Menu_Close(object sender, RoutedEventArgs e)
        {
            viewModel.closeWorld();
        }

        private void Menu_Run(object sender, RoutedEventArgs e)
        {
            RunPCGWindow runWin = new RunPCGWindow(viewModel);
            runWin.Show();
        }

        private void InteriorButton_Checked(object sender, RoutedEventArgs e)
        {
            viewModel.setLevelType(levelType.interior);
        }

        private void ExteriorButton_Checked(object sender, RoutedEventArgs e)
        {
            viewModel.setLevelType(levelType.exterior);
        }

        private void CreateLevel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                viewModel.addLevel(Convert.ToInt32(NewLevelSizeX.Text), Convert.ToInt32(NewLevelSizeY.Text));
            }
            catch (FormatException exception)
            {
                System.Console.WriteLine(exception.ToString());
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            viewModel.editLevel((int)e.GetPosition(LevelViewer).X, (int)e.GetPosition(LevelViewer).Y);

        }

    }
}
