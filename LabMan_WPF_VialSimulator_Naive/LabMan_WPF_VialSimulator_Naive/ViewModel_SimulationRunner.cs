using System.ComponentModel;

namespace LabMan_WPF_VialSimulator_Naive
{
    public class ViewModel_SimulationRunner : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        #region Private Members
        private System.Timers.Timer _SimulationTimer = new System.Timers.Timer();
        #endregion

        #region Public Properties
        private ViewModel_SimulationParameters _SimulationParameters;
        public ViewModel_SimulationParameters SimulationParameters
        {
            get
            {
                return _SimulationParameters;
            }
            set
            {
                if (value == _SimulationParameters)
                    return;

                _SimulationParameters = value;
                OnPropertyChanged("SimulationParameters");
            }
        }

        private ViewModel_Rack _InputRack;
        public ViewModel_Rack InputRack
        {
            get
            {
                return _InputRack;
            }
            set
            {
                if (value == _InputRack)
                    return;

                _InputRack = value;
                OnPropertyChanged("InputRack");
            }
        }

        private ViewModel_Rack _OutputRack;
        public ViewModel_Rack OutputRack
        {
            get
            {
                return _OutputRack;
            }
            set
            {
                if (value == _OutputRack)
                    return;

                _OutputRack = value;
                OnPropertyChanged("OutputRack");
            }
        }

        private ViewModel_Arm _Arm;
        public ViewModel_Arm Arm
        {
            get
            {
                return _Arm;
            }
            set
            {
                if (value == _Arm)
                    return;

                _Arm = value;
                OnPropertyChanged("Arm");
            }
        }
        #endregion

        #region Constructor
        public ViewModel_SimulationRunner()
        {
            // Fill in some defaults
            SimulationParameters = new ViewModel_SimulationParameters(96,288,3,1,10,0);

            _SimulationTimer.Elapsed += _Simulationtimer_Elapsed;
        }
        #endregion

        #region Private Events
        /// <summary>
        /// Fires when the arm movement has simulated and the appropriate time has elapsed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Simulationtimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _SimulationTimer.Stop();
            Arm.ToggleGrip();
            _SimulationTimer.Start();
        }
        #endregion

        #region Public Methods
        public void BeginSimulation()
        {
            _SimulationTimer.Stop();
            _SimulationTimer.Interval = 5000;
            _SimulationTimer.Start();
        }
        #endregion
    }
}
