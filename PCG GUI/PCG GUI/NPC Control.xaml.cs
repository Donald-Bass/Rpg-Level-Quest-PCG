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
    /// Interaction logic for NPC_Control.xaml
    /// </summary>
    public partial class NPC_Control : UserControl
    {
        public ViewModelParent viewModel { get; set;  }

        public NPC_Control()
        {
            InitializeComponent();
        }
    }
}
