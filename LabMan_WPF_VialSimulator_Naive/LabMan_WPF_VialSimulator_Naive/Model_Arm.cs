namespace LabMan_WPF_VialSimulator_Naive
{
    public static class Model_Arm
    {
        #region Constants
        public const int TIME_TO_GRAB_VIAL_ms                  = 1000;
        public const int TIME_TO_RELEASE_VIAL_ms               = 1000;
        public const int TIME_FROM_REST_TO_INPUT_ms            = 0;
        public const int TIME_FROM_INPUT_TO_GRIND_ms           = 1000;
        public const int TIME_FROM_GRIND_TO_DISPENSE_ms        = 1000;
        public const int TIME_FROM_DISPENSE_TO_OUTPUT_ms       = 1000;
        public const int TIME_FROM_OUTPUT_TO_DISPENSE_ms       = 1000;
        public const int TIME_FROM_DISPENSE_TO_INPUT_ms        = 1000;
        #endregion

        #region Enums
        public enum ArmStatus
        {
            // Would probably have some rest position
            HALT,
            MOVING,
            UNKNOWN
        }

        public enum ArmPosition
        {
            // Would probably have some rest position
            INPUT_RACK,
            GRIND_STATION,
            DISPENSE_STATION,
            OUTPUT_RACK,
            UNKNOWN,
        }

        public enum GripStatus
        {
            HALT,
            OPENING,
            CLOSING,
            UNKNOWN
        }

        public enum GripPosition
        {
            CLOSED,
            OPEN,
            UNKNOWN
        }
        #endregion
    }
}
