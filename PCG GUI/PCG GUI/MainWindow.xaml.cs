using PCG_GUI.Facts;
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
        public MainWindow()
        {
            InitializeComponent();
            drawGrid(32, 32);
        }

        private void drawGrid(int xLength, int yLength)
        {
            //draw horizontal lines
            for(int i = 0; i < yLength + 1; i++)
            {
                Line gridLine = new Line();
                gridLine.Stroke = System.Windows.Media.Brushes.Black;
                gridLine.X1 = 0;
                gridLine.X2 = xLength * 20;
                gridLine.Y1 = i * 20;
                gridLine.Y2 = i * 20;
                gridLine.StrokeThickness = 1;

                canvasMap.Children.Add(gridLine);
            }

            //draw vertical lines
            for (int i = 0; i < xLength + 1; i++)
            {
                Line gridLine = new Line();
                gridLine.Stroke = System.Windows.Media.Brushes.Black;
                gridLine.X1 = i * 20;
                gridLine.X2 = i * 20;
                gridLine.Y1 = 0;
                gridLine.Y2 = yLength * 20;
                gridLine.StrokeThickness = 1;

                canvasMap.Children.Add(gridLine);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //String[] valueTest = new String[2];

            //valueTest[0] = "5";
           //valueTest[1] = "7";
            System.IO.StreamReader file = new System.IO.StreamReader("results.txt");

            World test = new World();
            test.parseClingoFile(file);

            test.drawLevel(1, canvasMap);

            //Fact[] allFacts = test.parseClingoFile(file);

            file.Close();

            //foreach(Fact f in allFacts)
            //{
            //    System.Console.WriteLine(f.getStringRepresentation());
            //}
        }

    }
}
