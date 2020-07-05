using System;
using System.ComponentModel;

namespace LabMan_WPF_VialSimulator_Naive
{
    public class ViewModel_Arm : INotifyPropertyChanged
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

        public delegate void OnArmUpdateEventHandler(ViewModel_Arm sender, bool success, string messageArgs);
        public event OnArmUpdateEventHandler OnArmUpdateEvent;
        #endregion

        #region Private Members
        private System.Timers.Timer _ArmTimer = new System.Timers.Timer();
        #endregion

        #region Public Properties
        private Model_Arm.ArmStatus _ArmStatus;
        public Model_Arm.ArmStatus ArmStatus
        {
            get
            {
                return _ArmStatus;
            }
            set
            {
                if (value == _ArmStatus)
                    return;

                _ArmStatus = value;
                OnPropertyChanged("ArmStatus");
            }
        }

        private Model_Arm.ArmPosition _ArmPosition;
        public Model_Arm.ArmPosition ArmPosition
        {
            get
            {
                return _ArmPosition;
            }
            set
            {
                if (value == _ArmPosition)
                    return;

                _ArmPosition = value;
                OnPropertyChanged("ArmPosition");
            }
        }

        private Model_Arm.ArmPosition _FutureArmPosition;
        public Model_Arm.ArmPosition FutureArmPosition
        {
            get
            {
                return _FutureArmPosition;
            }
            set
            {
                if (value == _FutureArmPosition)
                    return;

                _FutureArmPosition = value;
                OnPropertyChanged("FutureArmPosition");
            }
        }

        private Model_Arm.GripStatus _GripStatus;
        public Model_Arm.GripStatus GripStatus
        {
            get
            {
                return _GripStatus;
            }
            set
            {
                if (value == _GripStatus)
                    return;

                _GripStatus = value;
                OnPropertyChanged("GripStatus");
            }
        }

        private Model_Arm.GripPosition _GripPosition;
        public Model_Arm.GripPosition GripPosition
        {
            get
            {
                return _GripPosition;
            }
            set
            {
                if (value == _GripPosition)
                    return;

                _GripPosition = value;
                OnPropertyChanged("GripPosition");
            }
        }

        private Model_Arm.GripPosition _FutureGripPosition;
        public Model_Arm.GripPosition FutureGripPosition
        {
            get
            {
                return _FutureGripPosition;
            }
            set
            {
                if (value == _FutureGripPosition)
                    return;

                _FutureGripPosition = value;
                OnPropertyChanged("FutureGripPosition");
            }
        }
        #endregion

        #region Constructor
        public ViewModel_Arm()
        {
            SetRest();

            _ArmTimer.Elapsed += _Armtimer_Elapsed;
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
        /// Fires when the arm movement has simulated and the appropriate time has elapsed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Armtimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _ArmTimer.Stop();
            _ArmTimer.Enabled = false;

            // Arm and Grip have finished movement 
            ArmPosition = FutureArmPosition;
            ArmStatus = Model_Arm.ArmStatus.HALT;
            GripPosition = FutureGripPosition;
            GripStatus = Model_Arm.GripStatus.HALT;

            // Clear future positions
            FutureArmPosition = Model_Arm.ArmPosition.UNKNOWN;
            FutureGripPosition = Model_Arm.GripPosition.UNKNOWN;

            // Notify top level state machine of success
            RaiseEventOnTopLevel(OnArmUpdateEvent, new object[] { this, true, string.Format("\nAt {0}: Arm is at {1} and Grip is {2}", e.SignalTime, ArmPosition, GripPosition) });
        }
        #endregion

        #region Private Methods
        private void SetRest()
        {
            ArmStatus = Model_Arm.ArmStatus.HALT;
            ArmPosition = Model_Arm.ArmPosition.INPUT_RACK;
            FutureArmPosition = Model_Arm.ArmPosition.UNKNOWN;
            GripStatus = Model_Arm.GripStatus.HALT;
            GripPosition = Model_Arm.GripPosition.OPEN;
        }

        private void BeginArmMovement(int timeForMovement, Model_Arm.ArmPosition newPosn)
        {
            // Should only get here with a positive time and the _ArmTimer not running
            // Double check the arm and grip are not already moving
            if((timeForMovement >= 0) && 
                (Model_Arm.ArmStatus.HALT == ArmStatus)&&
                (Model_Arm.GripStatus.HALT == GripStatus)&&
                (false == _ArmTimer.Enabled))
            {
                // Set the Arm status to moving as we simulate the arm movement
                ArmStatus = Model_Arm.ArmStatus.MOVING;

                // Set the current arm position to unknown as we simulate that we cannot know where it is
                ArmPosition = Model_Arm.ArmPosition.UNKNOWN;

                // Do not change grip position
                FutureGripPosition = GripPosition;

                // Future arm position set
                FutureArmPosition = newPosn;

                _ArmTimer.Interval = timeForMovement;
                _ArmTimer.Enabled = true;
                _ArmTimer.Start();
            }
        }

        private void BeginGripMovement(int timeForGripAction, Model_Arm.GripPosition newPosn, Model_Arm.GripStatus newStatus)
        {
            // Should only get here with a positive time and the _ArmTimer not running
            // Double check the arm and grip are not already moving
            if ((timeForGripAction >= 0) &&
                (Model_Arm.ArmStatus.HALT == ArmStatus) &&
                (Model_Arm.GripStatus.HALT == GripStatus) &&
                (false == _ArmTimer.Enabled))
            {
                // Set the grip status and next state
                GripPosition = Model_Arm.GripPosition.UNKNOWN;
                GripStatus = newStatus;
                FutureGripPosition = newPosn;

                // Do not overwrite arm position
                FutureArmPosition = ArmPosition;

                _ArmTimer.Interval = timeForGripAction;
                _ArmTimer.Enabled = true;
                _ArmTimer.Start();
            }
        }
        #endregion

        #region Public Methods
        public int MoveArmToNewPosition(Model_Arm.ArmPosition newPosn)
        {
            int ElapsingTime = -1;

            if (newPosn == ArmPosition)
            {
                ElapsingTime = 0;
            }
            else
            {
                switch (ArmPosition)
                {
                    case Model_Arm.ArmPosition.INPUT_RACK:
                        // From the Input Rack position the arm can only move to the Grind Station
                        if (Model_Arm.ArmPosition.GRIND_STATION == newPosn &&
                            Model_Arm.ArmStatus.HALT == ArmStatus &&
                            Model_Arm.GripPosition.CLOSED == GripPosition &&
                            Model_Arm.GripStatus.HALT == GripStatus)
                        {
                            ElapsingTime = (int)Model_Arm.TIME_FROM_INPUT_TO_GRIND_ms;
                        }
                        break;

                    case Model_Arm.ArmPosition.GRIND_STATION:
                        // From the Grind Station position the arm can only move to the Dispense Station
                        if (Model_Arm.ArmPosition.DISPENSE_STATION == newPosn &&
                            Model_Arm.ArmStatus.HALT == ArmStatus &&
                            Model_Arm.GripPosition.CLOSED == GripPosition &&
                            Model_Arm.GripStatus.HALT == GripStatus)
                        {
                            ElapsingTime = (int)Model_Arm.TIME_FROM_GRIND_TO_DISPENSE_ms;
                        }
                        break;

                    case Model_Arm.ArmPosition.DISPENSE_STATION:
                        // From the Dispense Station position the arm can move to Input or Output Racks

                        // Must be carrying "empty" input vial back to input rack
                        if (Model_Arm.ArmPosition.INPUT_RACK == newPosn &&
                            Model_Arm.ArmStatus.HALT == ArmStatus &&
                            Model_Arm.GripPosition.CLOSED == GripPosition &&
                            Model_Arm.GripStatus.HALT == GripStatus)
                        {
                            ElapsingTime = (int)Model_Arm.TIME_FROM_DISPENSE_TO_INPUT_ms;
                        }
                        // Must be carrying "full" output vial back to output rack
                        else if (Model_Arm.ArmPosition.OUTPUT_RACK == newPosn &&
                            Model_Arm.ArmStatus.HALT == ArmStatus &&
                            Model_Arm.GripPosition.CLOSED == GripPosition &&
                            Model_Arm.GripStatus.HALT == GripStatus)
                        {
                            ElapsingTime = (int)Model_Arm.TIME_FROM_DISPENSE_TO_OUTPUT_ms;
                        }
                        else
                        {
                            // N/A
                        }
                        break;

                    case Model_Arm.ArmPosition.OUTPUT_RACK:
                        // From the Output Rack position the arm can only move to the Dispense station
                        if (Model_Arm.ArmPosition.DISPENSE_STATION == newPosn &&
                            Model_Arm.ArmStatus.HALT == ArmStatus &&
                            Model_Arm.GripPosition.CLOSED == GripPosition &&
                            Model_Arm.GripStatus.HALT == GripStatus)
                        {
                            ElapsingTime = (int)Model_Arm.TIME_FROM_OUTPUT_TO_DISPENSE_ms;
                        }
                        break;

                    case Model_Arm.ArmPosition.UNKNOWN:
                        break;

                }
            }

            if (ElapsingTime >= 0)
            {
                BeginArmMovement(ElapsingTime, newPosn);
            }

            return ElapsingTime;
        }

        public int GrabVialFromCurrentRack(int index)
        {
            int ElapsingTime = -1;

            if ((ArmPosition == Model_Arm.ArmPosition.INPUT_RACK || ArmPosition == Model_Arm.ArmPosition.OUTPUT_RACK) &&
                GripStatus == Model_Arm.GripStatus.HALT &&
                GripPosition == Model_Arm.GripPosition.OPEN)
            {
                ElapsingTime = Model_Arm.TIME_TO_GRAB_VIAL_ms;
            }

            if (ElapsingTime >= 0)
            {
                BeginGripMovement(ElapsingTime, Model_Arm.GripPosition.CLOSED, Model_Arm.GripStatus.CLOSING);
            }

            return ElapsingTime;
        }

        public int ReleseVialToCurrentRack(uint index)
        {
            int ElapsingTime = -1;

            if ((ArmPosition == Model_Arm.ArmPosition.INPUT_RACK || ArmPosition == Model_Arm.ArmPosition.OUTPUT_RACK) &&
                GripStatus == Model_Arm.GripStatus.HALT &&
                GripPosition == Model_Arm.GripPosition.CLOSED)
            {
                ElapsingTime = Model_Arm.TIME_TO_RELEASE_VIAL_ms;
            }

            if (ElapsingTime >= 0)
            {
                BeginGripMovement(ElapsingTime, Model_Arm.GripPosition.OPEN, Model_Arm.GripStatus.OPENING);
            }

            return ElapsingTime;
        }
        #endregion
    }
}
