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
            STAGE6_PICK_UP_OUTPUT_VIAL,
            STAGE7_MOVE_FROM_OUTPUT_TO_DISPENSE,
            STAGE8_DISPENSE_INPUT_TO_OUTPUT,
            STAGE9_TRANSFER_FROM_DISPENSE_TO_OUTPUT,
            STAGE10_PLACE_VIAL_AT_OUTPUT,

            // When a single cycle has completed
            STAGE11_MOVE_FROM_DISPENSE_TO_INPUT,

            // Simulation complete
            STAGE12_SIMULATION_COMPLETE,

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

        private bool _IsRunning = false;
        public bool IsRunning
        {
            get
            {
                return _IsRunning;
            }

            set
            {
                if (value == _IsRunning)
                    return;

                _IsRunning = value;
                OnPropertyChanged("IsRunning");
            }
        }

        private bool _IsStopped = true;
        public bool IsStopped
        {
            get
            {
                return _IsStopped;
            }

            set
            {
                if (value == _IsStopped)
                    return;

                _IsStopped = value;
                OnPropertyChanged("IsStopped");
            }
        }

        private bool _CanExport;
        public bool CanExport
        {
            get
            {
                return _CanExport;
            }

            set
            {
                if (value == _CanExport)
                    return;

                _CanExport = value;
                OnPropertyChanged("CanExport");
            }
        }
        #endregion

        #region Private Properties
        // Has a stop command been received
        private bool _StopReceived = false;

        // Has a stop command been executed
        private bool _StopInitiated = false;

        // Used when a new input vial is needed
        private bool _RefreshInput = false;

        private bool _SimulationComplete = false;

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

            // Tie in events when parts of the system have completed their job
            _Arm.OnArmUpdateEvent += new ViewModel_Arm.OnArmUpdateEventHandler(Arm_OnArmUpdateEventReceived);
            _Arm.OnGripUpdateEvent += new ViewModel_Arm.OnGripUpdateEventHandler(Arm_OnGripUpdateEventReceived);
            _GrindStation.OnGrindUpdateEvent += new ViewModel_GrindStation.OnGrindUpdateEventHandler(GrindStation_OnGrindUpdateEventReceived);
            _DispenseStation.OnDispenseUpdateEvent += new ViewModel_DispenseStation.OnDispenseUpdateEventHandler(DispenseStation_OnDispenseUpdateEventReceived);

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

            if (true == _StopReceived && false == _StopInitiated)
            {
                _StopInitiated = true;

                 // Stopping
                RaiseEventOnTopLevel(OnSimulationUpdateEvent, new object[] { this, "\nSTOPPING on completion of current operation" });
            }

            if (true == _IsRunning)
            {
                // If not trying to stop, start again
                _SimulationTimer.Enabled = true;
                _SimulationTimer.Start();
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

        private void DoProceed()
        {
            UpdateTopState();
        }

        /// <summary>
        /// Received when the arm has completed a movement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="proceed"></param>
        /// <param name="message"></param>
        private void Arm_OnArmUpdateEventReceived(ViewModel_Arm sender, bool proceed, string message)
        {
            if (proceed)
            {
                DoProceed();
            }
            RaiseEventOnTopLevel(OnSimulationUpdateEvent, new object[] { this, message });
        }

        /// <summary>
        /// Received then the grip has completed a movement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="proceed"></param>
        /// <param name="message"></param>
        private void Arm_OnGripUpdateEventReceived(ViewModel_Arm sender, bool proceed, int index, Model_Arm.GripPosition grip, Model_Rack.RackPurpose rack, string message)
        {
            if (proceed)
            {
                if(SimulationState.STAGE1_PICK_UP_INPUT_VIAL == _CurrentState)
                {
                    // Belt and braces
                    if (Model_Rack.RackPurpose.INPUT == rack)
                    {
                        // Just picked up from input so set current input ID
                        _InputRack.SetCurrentID(index);
                        _RefreshInput = false;
                    }
                }

                if(SimulationState.STAGE6_PICK_UP_OUTPUT_VIAL == _CurrentState)
                {
                    // Belt and braces
                    if(Model_Rack.RackPurpose.OUTPUT == rack)
                    {
                        // Just picked up from output so set current output ID
                        _OutputRack.SetCurrentID(index);
                    }
                }

                if (SimulationState.STAGE10_PLACE_VIAL_AT_OUTPUT == _CurrentState)
                {
                    // Belt and braces
                    if (Model_Rack.RackPurpose.OUTPUT == rack)
                    {
                        _OutputRack.SetCurrentID(index + 1);
                    }
                }

                DoProceed();
            }
            RaiseEventOnTopLevel(OnSimulationUpdateEvent, new object[] { this, message });
        }

        /// <summary>
        /// Received when the grind station has completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="proceed"></param>
        /// <param name="message"></param>
        private void GrindStation_OnGrindUpdateEventReceived(ViewModel_GrindStation sender, bool proceed, string message)
        {
            if (proceed)
            {
                // Update vial to be ground
                _InputRack.SetCurrentVialFullFine((_SimulationParameters.TargetOutputVialWeight_mg)*_SimulationParameters.OutputDivisionFactor);

                DoProceed();
            }
            RaiseEventOnTopLevel(OnSimulationUpdateEvent, new object[] { this, message });
        }
        /// <summary>
        /// Received when the Dispenser has dispensed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="proceed"></param>
        /// <param name="message"></param>
        private void DispenseStation_OnDispenseUpdateEventReceived(ViewModel_DispenseStation sender, bool proceed, string message)
        {
            if (proceed)
            {
                // Is this the last fine material in the input vial?
                if (0 == ((_OutputRack.IDInUse+1) % _SimulationParameters.OutputDivisionFactor))
                {
                    _InputRack.SetCurrentVialEmpty();
                }
                else
                {
                    // Update input vial 
                    _InputRack.SetCurrentVialFullFine(_InputRack.GetCurrentVialWeight() - _SimulationParameters.TargetOutputVialWeight_mg);
                }

                // Update output vial of material
                _OutputRack.SetCurrentVialFullFine(_SimulationParameters.TargetOutputVialWeight_mg);
                               
                DoProceed();
            }
            RaiseEventOnTopLevel(OnSimulationUpdateEvent, new object[] { this, message });
        }
        #endregion

        #region Private Methods
        private void UpdateTopState()
        {
            _CurrentState = _FutureState;
            _FutureState = SimulationState.STAGE_UNKNOWN;
            _AbortTicks = -1;
        }

        private SimulationState CalculateNextStep()
        {
            SimulationState nextStep = _CurrentState;

            // At this point a full output vial is being moved to the output rack.
            // Decide if another output vial is to be filled, i.e. the input vial still has material, or
            // grab another input vial to grind.
            if (SimulationState.STAGE10_PLACE_VIAL_AT_OUTPUT == _CurrentState)
            {
                if(_InputRack.IsCurrentVialEmpty())
                {
                    // Was this the last vial ?
                    if (_InputRack.IDInUse == (_SimulationParameters.InputRackCapacity-1))
                    {
                        _SimulationComplete = true;
                    }
                    else
                    {
                        _InputRack.SetCurrentID(InputRack.IDInUse + 1);
                        _RefreshInput = true;
                    }

                    nextStep = SimulationState.STAGE7_MOVE_FROM_OUTPUT_TO_DISPENSE;
                }
                else
                {
                    // The input vial can fill more output vials
                    nextStep = SimulationState.STAGE6_PICK_UP_OUTPUT_VIAL;
                }
            }

            return nextStep;
        }

        private void CleanUpSimulation()
        {
            _StopReceived = false;
            _StopInitiated = false;

            IsRunning = false;
            IsStopped = true;
            CanExport = true;
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
                if (false == _StopInitiated)
                {
                    // Only work through the state transitions if not already waiting on a transition and not stopping
                    if (_FutureState == SimulationState.STAGE_UNKNOWN)
                    {
                        switch (_CurrentState)
                        {
                            case SimulationState.STAGE1_PICK_UP_INPUT_VIAL:
                                _AbortTicks = _Arm.GrabVialFromRack(_InputRack.IDInUse, _InputRack.Position);
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
                                _FutureState = SimulationState.STAGE6_PICK_UP_OUTPUT_VIAL;
                                break;

                            case SimulationState.STAGE6_PICK_UP_OUTPUT_VIAL:
                                _AbortTicks = _Arm.GrabVialFromRack(_OutputRack.IDInUse, _OutputRack.Position);
                                _FutureState = SimulationState.STAGE7_MOVE_FROM_OUTPUT_TO_DISPENSE;
                                break;

                            case SimulationState.STAGE7_MOVE_FROM_OUTPUT_TO_DISPENSE:
                                _AbortTicks = _Arm.MoveArmToNewPosition(Model_Arm.ArmPosition.DISPENSE_STATION);

                                if (true == _RefreshInput || true == _SimulationComplete)
                                {
                                    // This input vial is empty
                                    _FutureState = SimulationState.STAGE11_MOVE_FROM_DISPENSE_TO_INPUT;                               
                                }
                                else
                                {
                                    // Carry on throught the cycle
                                    _FutureState = SimulationState.STAGE8_DISPENSE_INPUT_TO_OUTPUT;
                                }
                                break;

                            case SimulationState.STAGE8_DISPENSE_INPUT_TO_OUTPUT:
                                _AbortTicks = _DispenseStation.Dispense(_InputRack.IDInUse, _OutputRack.IDInUse,
                                    _SimulationParameters.TargetOutputVialWeight_mg, _SimulationParameters.DispenserFlowRate_mgs);
                                _FutureState = SimulationState.STAGE9_TRANSFER_FROM_DISPENSE_TO_OUTPUT;
                                break;

                            case SimulationState.STAGE9_TRANSFER_FROM_DISPENSE_TO_OUTPUT:
                                _AbortTicks = _Arm.MoveArmToNewPosition(Model_Arm.ArmPosition.OUTPUT_RACK);
                                _FutureState = SimulationState.STAGE10_PLACE_VIAL_AT_OUTPUT;
                                break;

                            case SimulationState.STAGE10_PLACE_VIAL_AT_OUTPUT:
                                _AbortTicks = _Arm.ReleseVialToRack(_OutputRack.IDInUse, _OutputRack.Position);
                                _FutureState = CalculateNextStep();
                                break;

                            case SimulationState.STAGE11_MOVE_FROM_DISPENSE_TO_INPUT:
                                _AbortTicks = _Arm.MoveArmToNewPosition(Model_Arm.ArmPosition.INPUT_RACK);
                                if (false == _SimulationComplete)
                                {
                                    // Grab the next input vial
                                    _FutureState = SimulationState.STAGE1_PICK_UP_INPUT_VIAL;
                                }
                                else
                                {
                                    _FutureState = SimulationState.STAGE12_SIMULATION_COMPLETE;
                                }
                                break;

                            case SimulationState.STAGE12_SIMULATION_COMPLETE:
                                RaiseEventOnTopLevel(OnSimulationUpdateEvent, new object[] { this, string.Format("\nSimulation Complete!") });
                                CleanUpSimulation();
                                break;

                            default:
                                throw new NotImplementedException();
                                break;
                        }

                        // Have new state
                        if (_AbortTicks > 0)
                        {
                            _AbortTicks = (int)(_AbortTicks * 1.5);
                        }
                    }
                }
                else
                {
                    // _AbortTicks is used as an indication of a process currently running in the back ground.
                    // Only stop when it is safe to do so
                    if (_AbortTicks < 0)
                    {                       
                        RaiseEventOnTopLevel(OnSimulationUpdateEvent, new object[] { this, string.Format("\nSTOPPED!") });

                        CleanUpSimulation();
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

            _AbortTicks = -1;

            _SimulationTimer.Stop();
            _SimulationTimer.Interval = 1;
            _SimulationTimer.Start();

            _SimulationComplete = false;

            IsRunning = true;
            IsStopped = false;
            CanExport = false;
        }
        #endregion
    }
}
