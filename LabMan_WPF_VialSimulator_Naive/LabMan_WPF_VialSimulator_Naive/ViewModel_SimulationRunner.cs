using System;
using System.ComponentModel;

namespace LabMan_WPF_VialSimulator_Naive
{
    public class ViewModel_SimulationRunner : INotifyPropertyChanged
    {
        private enum SimulationState
        {
            STAGE1_PICK_UP_INPUT_VIAL,
            STAGE2_MOVE_FROM_INPUT_TO_GRIND,
            STAGE3_GRIND_INPUT,
            STAGE_UNKNOWN
        }

        #region Events
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public delegate void OnSimulationUpdateEventHandler(ViewModel_SimulationRunner sender, string messageArgs);
        public event OnSimulationUpdateEventHandler OnSimulationUpdateEvent;
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

        public bool IsRunning
        {
            get;
        }
        #endregion

        #region Private Properties
        // Has a stop command been received
        private bool _StopReceived = false;

        private SimulationState _CurrentState;
        private SimulationState _FutureState;

        private int _AbortTicks = 0;
        private System.Timers.Timer _SimulationTimer = new System.Timers.Timer();
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
            RunSimulationStateMachine();
        }

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

        private void Arm_OnArmUpdateEventReceived(ViewModel_Arm sender, bool proceed, string message)
        {
            if(true == proceed)
            {
                UpdateTopState();
            }

            RaiseEventOnTopLevel(OnSimulationUpdateEvent, new object[] { this, message });
        }
        #endregion

        #region Private Methods
        private void UpdateTopState()
        {
            _CurrentState = _FutureState;
            _FutureState = SimulationState.STAGE_UNKNOWN;
            _AbortTicks = 0;
        }
        private void RunSimulationStateMachine()
        {
            // If there is a future state then decrement abort ticks, if this ever reaches 0
            //then something has gone drastically wrong!
            if (_FutureState != SimulationState.STAGE_UNKNOWN)
            {
                if (_AbortTicks > 0)
                {
                    _AbortTicks = _AbortTicks - (int)(_SimulationTimer.Interval);
                }
            }

            if (_AbortTicks >= 0)
            {
                if (false == _StopReceived && _FutureState == SimulationState.STAGE_UNKNOWN)
                {
                    switch (_CurrentState)
                    {
                        case SimulationState.STAGE1_PICK_UP_INPUT_VIAL:
                            _AbortTicks = _Arm.GrabVialFromCurrentRack(InputRack.IndexInUse);
                            _FutureState = SimulationState.STAGE2_MOVE_FROM_INPUT_TO_GRIND;
                            break;

                        case SimulationState.STAGE2_MOVE_FROM_INPUT_TO_GRIND:
                            _AbortTicks = _Arm.MoveArmToNewPosition(Model_Arm.ArmPosition.GRIND_STATION);
                            _FutureState = SimulationState.STAGE3_GRIND_INPUT;
                            break;
                    }

                    // Add some error margin (25%) to the abort ticks!
                    if (_FutureState != SimulationState.STAGE_UNKNOWN)
                    {
                        _AbortTicks = (int)(_AbortTicks * 1.25);
                    }
                }
                else
                {

                }
            }
            else
            {
                RaiseEventOnTopLevel(OnSimulationUpdateEvent, new object[] { this, string.Format("\nERROR moving from state {0} to state {1}", _CurrentState, _FutureState) });
            }
        }
        #endregion

        #region Public Methods
        public void StopSimulation()
        {
            _StopReceived = true;
        }
        public void BeginSimulation()
        {
            _SimulationTimer.Stop();
            _SimulationTimer.Interval = 1;
            _SimulationTimer.Start();

            _CurrentState = SimulationState.STAGE1_PICK_UP_INPUT_VIAL;
            _FutureState = SimulationState.STAGE_UNKNOWN;

            InputRack.ResetRackVars();
            OutputRack.ResetRackVars();

            _Arm.OnArmUpdateEvent += new ViewModel_Arm.OnArmUpdateEventHandler(Arm_OnArmUpdateEventReceived);
        }
        #endregion
    }
}
