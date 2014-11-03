using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCG_GUI.ViewModels
{
    public class ViewModelParent
    {
        public BaseViewModel BaseView { get; private set; }
        public LevelViewModel LevelView { get; private set; }

        public ViewModelParent()
        {
            BaseView = new BaseViewModel();
            LevelView = new LevelViewModel(BaseView);
        }

        public void Close()
        {
            BaseView.closeWorld();
            LevelView.finishClose();
        }

        public void Open(String Filename)
        {
            BaseView.open(Filename);
            LevelView.finishOpen();
        }

        public void newWorld()
        {
            Close();
            //BaseView.newWorld();
        }

        public void runClingo()
        {
            BaseView.runClingo(LevelView.plan);
            //Close();
            Open("TempResults.pcg");
        }
    }
}
