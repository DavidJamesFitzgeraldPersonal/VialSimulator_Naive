using System;
using System.ComponentModel;

namespace LabMan_WPF_VialSimulator_Naive
{
    public class ViewModel_DispenseStation : INotifyPropertyChanged
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

        public delegate void OnDispenseUpdateEventHandler(ViewModel_DispenseStation sender, bool success, string messageArgs);
        public event OnDispenseUpdateEventHandler OnDispenseUpdateEvent;
        #endregion

        #region Private Properties
        private int _DispenseFromID = 0;
        private int _DispenseToID = 0;
        private System.Timers.Timer _DispenseTimer = new System.Timers.Timer();
        #endregion

        #region Public Properties
        private Model_DispenseStation.DispenserStatus _DispenserStatus;
        public Model_DispenseStation.DispenserStatus DispenserStatus
        {
            get
            {
                return _DispenserStatus;
            }
            set
            {
                if (value == _DispenserStatus)
                    return;

                _DispenserStatus = value;
                OnPropertyChanged("DispenserStatus");
            }
        }
        #endregion

        #region Constructor
        public ViewModel_DispenseStation()
        {
            SetRest();

            _DispenseTimer.Elapsed += _Dispensetimer_Elapsed;
        }
        #endregion

        #region Private Events
        private void RaiseEventOnTopLevel(Delegate theEvent, object[] args)
        {
            if (theEvent != null)
            {
                foreach (Delegate d in theEvent.GetInvocationList())
                {
                    ISynchronizeInvoke syncer = d.Target as ISynchronizeInvoke;
                    if (syncer == null)
                    {
                        d.DynamicInvoke(args);
                    }
                    else
                    {
                        syncer.BeginInvoke(d, args);
                    }
                }
            }
        }

        /// <summary>
        /// Fires when the grind has simulated and the appropriate time has elapsed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Dispensetimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _DispenseTimer.Stop();
            _DispenseTimer.Enabled = false;

            // Dispenser has dispensed !
            DispenserStatus = Model_DispenseStation.DispenserStatus.HALT;

            // Notify top level state machine of success
            RaiseEventOnTopLevel(OnDispenseUpdateEvent, new object[] { this, true, string.Format("\nAt {0}: Dispensed from Input Vial ID{1} to Output Vial ID{2}", e.SignalTime, _DispenseFromID, _DispenseToID) });
        }
        #endregion

        #region Private Methods
        private void SetRest()
        {
            _DispenseFromID = 0;
            _DispenseToID = 0;
            DispenserStatus = Model_DispenseStation.DispenserStatus.HALT;
        }

        private void BeginDispense(double timeForDispense)
        {
            // Should only get here with a positive time and the _Dispensetimer not running
            // Needs valid vial IDs
            if ((timeForDispense >= 0) &&
                (0 != _DispenseFromID)&&
                (0 != _DispenseToID)&&
                (Model_DispenseStation.DispenserStatus.HALT == DispenserStatus) &&
                (false == _DispenseTimer.Enabled))
            {
                DispenserStatus = Model_DispenseStation.DispenserStatus.DISPENSING;

                _DispenseTimer.Interval = timeForDispense;
                _DispenseTimer.Enabled = true;
                _DispenseTimer.Start();

                // Notify top level state machine of success
                RaiseEventOnTopLevel(OnDispenseUpdateEvent, new object[] { this, true, string.Format("\nBegin dispense from Input Vial ID{0} to Output Vial ID{0}", _DispenseFromID, _DispenseToID) });
            }
        }
        #endregion

        #region Public Methods
        public int Dispense(int inputID, int outputID, float target, float rate)
        {
            double ElapsingTime = -1.0d;

            // If not already dispensing
            if (DispenserStatus == Model_DispenseStation.DispenserStatus.HALT)
            {
                _DispenseFromID = inputID;
                _DispenseToID = outputID;

                ElapsingTime = (((double)target / (double)rate)*1000.0d);
            }

            if (ElapsingTime >= 0)
            {
                BeginDispense(ElapsingTime);
            }

            return (int)ElapsingTime;
        }
        #endregion
    }
}
