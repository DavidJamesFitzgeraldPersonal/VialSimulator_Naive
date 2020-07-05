namespace LabMan_WPF_VialSimulator_Naive
{
    public static class Model_GrindStation
    {
        #region Constants
        public const int TIME_TO_GRIND_VIAL_ms = 1000;
        #endregion

        #region Enums
        public enum GrindStatus
        {
            HALT,
            GRINDING,
            UNKNOWN
        }
        #endregion
    }
}
