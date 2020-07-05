using System;
using System.ComponentModel;

namespace LabMan_WPF_VialSimulator_Naive
{
    public class ViewModel_SimulationRunner : INotifyPropertyChanged
    {
        private enum SimulationState
        {
            STAGE0_INIT,
            STAGE1_PICK_UP_INPUT_VIAL,
            STAGE2_MOVE_FROM_INPUT_TO_GRIND,
            STAGE3_GRIND_INPUT,
            STAGE4_MOVE_FROM_GRIND_TO_DISPENSE,
            STAGE5_MOVE_FROM_DISPENSE_TO_OUTPUT,
            STAGE6_MOVE_FROM_OUTPUT_TO_DISPENSE,
            STAGE7_TRANSFER_FROM_INPUT_TO_OUTPUT,
            STAGE8_TRANSFER_FROM_DISPENSE_TO_OUTPUT, // TODO Can this be rolled into STAGE5_MOVE_FROM_DISPENSE_TO_OUTPUT
            STAGE9_TRANSFER_FROM_OUTPUT_TO_DISPENSE, // TODO tie into STAGE6_MOVE_FROM_OUTPUT_TO_DISPENSE 
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

        private ViewModel_GrindStation _GrindStation;
        public ViewModel_GrindStation GrindStation
        {
            get
            {
                return _GrindStation;
            }
            set
            {
                if (value == _GrindStation)
                    return;

                _GrindStation = value;
                OnPropertyChanged("GrindStation");
            }
        }

        private ViewModel_DispenseStation _DispenseStation;
        public ViewModel_DispenseStation DispenseStation
        {
            get
            {
                return _DispenseStation;
            }
            set
            {
                if (value == _DispenseStation)
                    return;

                _DispenseStation = value;
                OnPropertyChanged("DispenseStation");
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

            Arm = new ViewModel_Arm();
            GrindStation = new ViewModel_GrindStation();
            DispenseStation = new ViewModel_DispenseStation();
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
            _SimulationTimer.Enabled = false;

            if (false == _semaphore)
            {
                RunSimulationStateMachine();
            }

            if (false == _StopReceived)
            {
                // If not trying to stop, start again
                _SimulationTimer.Enabled = true;
                _SimulationTimer.Start();
            }
            else
            {

            }
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
                        syncer.BeginInvoke(d, args);
                    }
                }
            }
        }

        private void DoProceed(string message)
        {
            UpdateTopState();
            RaiseEventOnTopLevel(OnSimulationUpdateEvent, new object[] { this, message });
        }
        private void Arm_OnArmUpdateEventReceived(ViewModel_Arm sender, bool proceed, string message)
        {
            if (proceed)
            {
                DoProceed(message);
            }
        }

        private void GrindStation_OnGrindUpdateEventReceived(ViewModel_GrindStation sender, bool proceed, string message)
        {
            if (proceed)
            {
                // Update vial to be ground
                _InputRack.GroundVial();

                DoProceed(message);
            }
        }
        private void DispenseStation_OnDispenseUpdateEventReceived(ViewModel_DispenseStation sender, bool proceed, string message)
        {
            if (proceed)
            {
                // Update new vial of material
                _OutputRack.GroundVial();

                DoProceed(message);
            }
        }
        #endregion

        #region Private Methods
        private void UpdateTopState()
        {
            _CurrentState = _FutureState;
            _FutureState = SimulationState.STAGE_UNKNOWN;
            _AbortTicks = -1;
        }


        /// <summary>
        /// The state machine handler. Will be called every _SimulationTimer.Interval. Added semaphore
        /// accesss due to strange behaviour of _AbortTicks. Seems far too long.
        /// </summary>
        static bool _semaphore = false;
        private void RunSimulationStateMachine()
        {
            _semaphore = true;
            // If there is an abort time then decrement, if this ever reaches 0
            //then something has gone drastically wrong!
            // TODO there is something wrong here - The Abort Timer seems to take 10x longer than it should
            if (_AbortTicks > 0)
            {
                _AbortTicks = _AbortTicks - (int)(_SimulationTimer.Interval);
                if(_AbortTicks < 0)
                {
                    _AbortTicks = 0;
                }
            }

            if(_AbortTicks == 0)
            {
                StopSimulation();
                RaiseEventOnTopLevel(OnSimulationUpdateEvent, new object[] { this, string.Format("\nERROR moving from state {0} to state {1}", _CurrentState, _FutureState) });
            }
            else
            {
                // Only work through the state transitions if not already waiting on a transition
                if (false == _StopReceived && _FutureState == SimulationState.STAGE_UNKNOWN)
                {
                    switch (_CurrentState)
                    {
                        case SimulationState.STAGE1_PICK_UP_INPUT_VIAL:
                            _AbortTicks = _Arm.GrabVialFromCurrentRack(_InputRack.IDInUse);
                            _FutureState = SimulationState.STAGE2_MOVE_FROM_INPUT_TO_GRIND;
                            break;

                        case SimulationState.STAGE2_MOVE_FROM_INPUT_TO_GRIND:
                            _AbortTicks = _Arm.MoveArmToNewPosition(Model_Arm.ArmPosition.GRIND_STATION);
                            _FutureState = SimulationState.STAGE3_GRIND_INPUT;
                            break;

                        case SimulationState.STAGE3_GRIND_INPUT:
                            _AbortTicks = _GrindStation.GrindVial(_InputRack.IDInUse);
                            _FutureState = SimulationState.STAGE4_MOVE_FROM_GRIND_TO_DISPENSE;
                            break;

                        case SimulationState.STAGE4_MOVE_FROM_GRIND_TO_DISPENSE:
                            _AbortTicks = _Arm.MoveArmToNewPosition(Model_Arm.ArmPosition.DISPENSE_STATION);
                            _FutureState = SimulationState.STAGE5_MOVE_FROM_DISPENSE_TO_OUTPUT;
                            break;

                        case SimulationState.STAGE5_MOVE_FROM_DISPENSE_TO_OUTPUT:
                            _AbortTicks = _Arm.MoveArmToNewPosition(Model_Arm.ArmPosition.OUTPUT_RACK);
                            _FutureState = SimulationState.STAGE6_MOVE_FROM_OUTPUT_TO_DISPENSE;
                            break;

                        case SimulationState.STAGE6_MOVE_FROM_OUTPUT_TO_DISPENSE:
                            _AbortTicks = _Arm.MoveArmToNewPosition(Model_Arm.ArmPosition.DISPENSE_STATION);
                            _FutureState = SimulationState.STAGE7_TRANSFER_FROM_INPUT_TO_OUTPUT;
                            break;

                        case SimulationState.STAGE7_TRANSFER_FROM_INPUT_TO_OUTPUT:
                            _AbortTicks = _DispenseStation.Dispense(_InputRack.IDInUse, _OutputRack.IDInUse,
                                _SimulationParameters.TargetOutputVialWeight_mg, _SimulationParameters.DispenserFlowRate_mgs);
                            _FutureState = SimulationState.STAGE8_TRANSFER_FROM_DISPENSE_TO_OUTPUT;
                            break;

                        case SimulationState.STAGE8_TRANSFER_FROM_DISPENSE_TO_OUTPUT:
                            _AbortTicks = _Arm.MoveArmToNewPosition(Model_Arm.ArmPosition.OUTPUT_RACK);
                            _FutureState = SimulationState.STAGE9_TRANSFER_FROM_OUTPUT_TO_DISPENSE;
                            break;
                    }

                    // Have new state
                    if(_AbortTicks > 0)
                    {
                        _AbortTicks = (int)(_AbortTicks * 1.5);
                    }
                }
                
            }
            _semaphore = false;
        }
        #endregion

        #region Public Methods
        public void StopSimulation()
        {
            _StopReceived = true;
        }
        public void BeginSimulation()
        {
            _CurrentState = SimulationState.STAGE1_PICK_UP_INPUT_VIAL;
            _FutureState = SimulationState.STAGE_UNKNOWN;

            InputRack.ResetRackVars();
            OutputRack.ResetRackVars();

            // Tie in events when parts of the system have completed their job
            _Arm.OnArmUpdateEvent += new ViewModel_Arm.OnArmUpdateEventHandler(Arm_OnArmUpdateEventReceived);
            _GrindStation.OnGrindUpdateEvent += new ViewModel_GrindStation.OnGrindUpdateEventHandler(GrindStation_OnGrindUpdateEventReceived);
            _DispenseStation.OnDispenseUpdateEvent += new ViewModel_DispenseStation.OnDispenseUpdateEventHandler(DispenseStation_OnDispenseUpdateEventReceived);

            _AbortTicks = -1;

            _SimulationTimer.Stop();
            _SimulationTimer.Interval = 1;
            _SimulationTimer.Start();
        }
        #endregion
    }
}
