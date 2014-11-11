using PCG_GUI.Facts;
using PCG_GUI.PlanModel;
using PCG_GUI.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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
        BaseViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new BaseViewModel();
            DataContext = viewModel;
            LevelView.viewModel = this.viewModel;
            LevelView.DataContext = viewModel;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_New(object sender, RoutedEventArgs e)
        {
            viewModel.closeWorld();

            //System.IO.StreamWriter file = new System.IO.StreamWriter("WorldDef.txt");
            //PlanLevel testPlan = new PlanLevel();
            //testPlan.writePlan(file);


            //file.Close();
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

     
        //Code to save the contents of the scrollviewer the map is displayed in to a png file
        //This is not my code. It was taken from http://stackoverflow.com/questions/24934276/how-do-i-save-all-content-of-a-wpf-scrollviewer-as-an-image
        public static void SnapShotPNG(UIElement source, string destination, int zoom)
        {
            try
            {
                double actualHeight = source.RenderSize.Height;
                double actualWidth = source.RenderSize.Width;

                double renderHeight = actualHeight * zoom;
                double renderWidth = actualWidth * zoom;

                RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)renderWidth, (int)renderHeight, 96, 96, PixelFormats.Pbgra32);
                VisualBrush sourceBrush = new VisualBrush(source);

                DrawingVisual drawingVisual = new DrawingVisual();
                DrawingContext drawingContext = drawingVisual.RenderOpen();

                using (drawingContext)
                {
                    drawingContext.PushTransform(new ScaleTransform(zoom, zoom));
                    drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Point(actualWidth, actualHeight)));
                }
                renderTarget.Render(drawingVisual);

                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTarget));
                using (FileStream stream = new FileStream(destination, FileMode.Create, FileAccess.Write))
                {
                    encoder.Save(stream);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void Menu_SaveImage(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            // Set filter for file extension and default file extension
            dlg.Filter = "PNG files (*.png)| *.png";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and save the image
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                SnapShotPNG((UIElement)LevelView.MapScroll.Content, filename, 1);
            }

        }


        private void Menu_Run(object sender, RoutedEventArgs e)
        {
            viewModel.runClingo();
            viewModel.open("TempResults.pcg");
        }

        private void LevelView_Loaded(object sender, RoutedEventArgs e)
        {

        }

      

    }
}
