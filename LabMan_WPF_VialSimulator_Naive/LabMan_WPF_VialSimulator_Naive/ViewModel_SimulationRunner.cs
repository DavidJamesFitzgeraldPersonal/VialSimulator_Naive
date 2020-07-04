using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace LabMan_WPF_VialSimulator_Naive
{
    public class ViewModel_SimulationRunner : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public ViewModel_SimulationParameters SimulationParameters
        {
            get;
            set;
        }
        public ViewModel_Rack InputRack
        {
            get;
            set;
        }
        public ViewModel_Rack OutputRack
        {
            get;
            set;
        }

        #region Constructor
        public ViewModel_SimulationRunner()
        {
            // Fill in some defaults
            SimulationParameters = new ViewModel_SimulationParameters(96,288,3,1,10,0);
        }
        #endregion
    }
}
