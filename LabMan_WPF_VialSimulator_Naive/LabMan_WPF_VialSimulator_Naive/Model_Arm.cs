using System;
using System.Collections.Generic;
using System.Text;

namespace LabMan_WPF_VialSimulator_Naive
{
    public class Model_Arm
    {
        #region Enums
        public enum ArmStatus
        {
            // Would probably have some rest position
            ARM_STATUS_HALT,
            ARM_STATUS_MOVING,
            ARM_STATUS_UNKNOWN
        }

        public enum ArmPosition
        {
            // Would probably have some rest position
            ARM_POSN_INPUT_RACK,
            ARM_POSN_GRIND_STATION,
            ARM_POSN_DISPENSE_STATION,
            ARM_POSN_OUTPUT_RACK,
            ARM_POSN_UNKNOWN,
        }

        public enum GripStatus
        {
            GRIP_STATUS_HAVE_VIAL,
            GRIP_STATUS_NO_VIAL,
            GRIP_STATUS_UKNOWN
        }
        #endregion

        #region Structs
        public struct MovementTimes
        {
            // Assume time taken to move from one position on an input/output rack to the next is negligable
            public uint GrabVial_ms;
            public uint ReleaseVial_ms;
            public uint FromRestToInputRackPosn_ms;
            public uint FromInputRackToGrindStation_ms;
            public uint FromGrindStationToDispenseStation_ms;
            public uint FromDispenseStationToOutputRackPosn_ms;
            public uint FromOutputRackPosnToDispenseStation_ms;
            public uint FromDispenseStationToInputRackPosn_ms;
        }
        #endregion

        #region Members
        private ArmStatus _CurrentStatus;
        public ArmStatus CurrentStatus
        {
            get { return _CurrentStatus; }
        }

        private ArmPosition _CurrentPosition;
        public ArmPosition CurrentPosition
        {
            get { return _CurrentPosition; }
        }

        private ArmPosition _FuturePosition;
        public ArmPosition FuturePosition
        {
            get { return _FuturePosition; }
        }

        private GripStatus _CurrentGrip;
        public GripStatus CurrentGrip
        {
            get { return _CurrentGrip; }
        }

        private MovementTimes _ArmTiming;
        private System.Timers.Timer _ArmTimer = new System.Timers.Timer();

        #endregion

        #region Constructor
        public Model_Arm()
        {
            LoadDefaultArmTimings();
            SetRest();

            _ArmTimer.Elapsed += _Armtimer_Elapsed;
        }
        #endregion

        
        private void SetRest()
        {
            _CurrentStatus = ArmStatus.ARM_STATUS_HALT;
            _CurrentPosition = ArmPosition.ARM_POSN_INPUT_RACK;
            _FuturePosition = ArmPosition.ARM_POSN_UNKNOWN;
            _CurrentGrip = GripStatus.GRIP_STATUS_NO_VIAL;
        }
        private void LoadDefaultArmTimings()
        {
            _ArmTiming.GrabVial_ms                               = 1000;
            _ArmTiming.ReleaseVial_ms                            = 1000;
            _ArmTiming.FromInputRackToGrindStation_ms            = 1000;
            _ArmTiming.FromGrindStationToDispenseStation_ms      = 1000;
            _ArmTiming.FromDispenseStationToOutputRackPosn_ms    = 1000;
            _ArmTiming.FromOutputRackPosnToDispenseStation_ms    = 1000;
            _ArmTiming.FromDispenseStationToInputRackPosn_ms     = 1000;
        }

        #region Private Events
        private void _Armtimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _ArmTimer.Stop();
            _ArmTimer.Enabled = false;

            _CurrentPosition = _FuturePosition;
            _FuturePosition = ArmPosition.ARM_POSN_UNKNOWN;

            _CurrentStatus = ArmStatus.ARM_STATUS_HALT;
        }
        #endregion

        #region Private Methods
        private void BeginArmMovement(int timeForMovement)
        {
            // We should only get here with a positive time and the _ArmTimer not running
            if((timeForMovement >= 0) && 
                (ArmStatus.ARM_STATUS_HALT == _CurrentStatus)&&
                (false == _ArmTimer.Enabled))
            {
                _CurrentStatus = ArmStatus.ARM_STATUS_MOVING;
                _CurrentPosition = ArmPosition.ARM_POSN_UNKNOWN;

                _ArmTimer.Interval = timeForMovement;
                _ArmTimer.Enabled = true;
                _ArmTimer.Start();
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="newPosn"></param>
        /// <returns></returns>
        public int MoveArmToNewPosition(ArmPosition newPosn)
        {
            int ElapsingTime = -1;

            switch(_CurrentPosition)
            {
                case ArmPosition.ARM_POSN_INPUT_RACK:
                    // From the Input Rack position the arm can only move to the Grind Station
                    if(ArmPosition.ARM_POSN_GRIND_STATION == newPosn)
                    {
                        ElapsingTime = (int)_ArmTiming.FromInputRackToGrindStation_ms;
                    }
                    break;

                case ArmPosition.ARM_POSN_GRIND_STATION:
                    // From the Grin Station position the arm can only move to the Dispense Station
                    if (ArmPosition.ARM_POSN_DISPENSE_STATION == newPosn)
                    {
                        ElapsingTime = (int)_ArmTiming.FromInputRackToGrindStation_ms;
                    }
                    break;

                case ArmPosition.ARM_POSN_DISPENSE_STATION:
                    // From the Dispense Station position the arm can move to Input or Output Racks
                    if (ArmPosition.ARM_POSN_INPUT_RACK == newPosn)
                    {
                        ElapsingTime = (int)_ArmTiming.FromDispenseStationToInputRackPosn_ms;
                    }
                    else if (ArmPosition.ARM_POSN_OUTPUT_RACK == newPosn)
                    {
                        ElapsingTime = (int)_ArmTiming.FromDispenseStationToOutputRackPosn_ms;
                    }
                    else
                    {
                        // N/A
                    }
                    break;

                case ArmPosition.ARM_POSN_OUTPUT_RACK:
                    // From the Output Rack position the arm can only move to the Dispense station
                    if (ArmPosition.ARM_POSN_DISPENSE_STATION == newPosn)
                    {
                        ElapsingTime = (int)_ArmTiming.FromOutputRackPosnToDispenseStation_ms;
                    }
                    break;

                case ArmPosition.ARM_POSN_UNKNOWN:
                    break;

            }

            if(ElapsingTime >= 0)
            {
                _FuturePosition = newPosn;
                BeginArmMovement(ElapsingTime);
            }

            return ElapsingTime;
        }
        #endregion
    }
}
