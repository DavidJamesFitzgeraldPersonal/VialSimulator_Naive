using System;
using System.ComponentModel;

namespace LabMan_WPF_VialSimulator_Naive
{
    public class ViewModel_GrindStation : INotifyPropertyChanged
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

        public delegate void OnGrindUpdateEventHandler(ViewModel_GrindStation sender, bool success, string messageArgs);
        public event OnGrindUpdateEventHandler OnGrindUpdateEvent;
        #endregion

        #region Private Properties
        private int _GrindingID = 0;
        private System.Timers.Timer _GrindTimer = new System.Timers.Timer();
        #endregion

        #region Public Properties
        private Model_GrindStation.GrindStatus _GrindStatus;
        public Model_GrindStation.GrindStatus GrindStatus
        {
            get
            {
                return _GrindStatus;
            }
            set
            {
                if (value == _GrindStatus)
                    return;

                _GrindStatus = value;
                OnPropertyChanged("GrindStatus");
            }
        }       
        #endregion

        #region Constructor
        public ViewModel_GrindStation()
        {
            SetRest();

            _GrindTimer.Elapsed += _Grindtimer_Elapsed;
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
                        syncer.BeginInvoke(d, args);  // cleanup omitted
                    }
                }
            }
        }

        /// <summary>
        /// Fires when the grind has simulated and the appropriate time has elapsed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Grindtimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _GrindTimer.Stop();
            _GrindTimer.Enabled = false;

            // Grind has ground !
            GrindStatus = Model_GrindStation.GrindStatus.HALT;

            // Notify top level state machine of success
            RaiseEventOnTopLevel(OnGrindUpdateEvent, new object[] { this, true, string.Format("\nAt {0}: Ground Input Vial ID {1}", e.SignalTime, _GrindingID) });
        }
        #endregion

        #region Private Methods
        private void SetRest()
        {
            _GrindingID = 0;
            GrindStatus = Model_GrindStation.GrindStatus.HALT;
        }

        private void BeginGrind(int timeForGrind)
        {
            // Should only get here with a positive time and the _Grindimer not running
            if ((timeForGrind >= 0) &&
                (Model_GrindStation.GrindStatus.HALT == GrindStatus) &&
                (false == _GrindTimer.Enabled))
            {
                GrindStatus = Model_GrindStation.GrindStatus.GRINDING;

                _GrindTimer.Interval = timeForGrind;
                _GrindTimer.Enabled = true;
                _GrindTimer.Start();

                // Notify top level state machine of success
                RaiseEventOnTopLevel(OnGrindUpdateEvent, new object[] { this, true, string.Format("\nBegin grind of Input Vial ID {0}", _GrindingID) });
            }
        }
        #endregion

        #region Public Methods
        public int GrindVial(int index)
        {
            int ElapsingTime = -1;

            // If not already grinding
            if (GrindStatus == Model_GrindStation.GrindStatus.HALT)
            {
                _GrindingID = index;
                ElapsingTime = Model_GrindStation.TIME_TO_GRIND_VIAL_ms;
            }

            if (ElapsingTime >= 0)
            {
               BeginGrind(ElapsingTime);
            }

            return ElapsingTime;
        }
        #endregion
    }
}
