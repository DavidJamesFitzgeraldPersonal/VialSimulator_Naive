using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LabMan_WPF_VialSimulator_Naive
{
    public class Model_SimulationParameters
    {

        #region Constants
        public const uint MIN_INPUT_RACK_CAPACITY        = 1;
        public const uint MAX_INPUT_RACK_CAPACITY        = 100;

        public const uint MIN_OUTPUT_RACK_CAPACITY       = 1;
        public const uint MAX_OUTPUT_RACK_CAPACITY       = 300;

        public const uint MIN_OUTPUT_DIV_FACTOR          = 1;

        public const float MIN_BALANCE_ERROR_mg          = 0.0f;

        public const uint MIN_TARGET_OUTPUT_WEIGHT_mg    = 0;
        public const uint MAX_TARGET_OUTPUT_WEIGHT_mg    = 1000;

        public const uint MIN_FLOW_RATE_mgs              = 0;
        public const uint MAX_FLOW_RATE_mgs              = 10;
        #endregion

        #region Public Methods
        /// <summary>
        /// Ensure the specified input rack capacity is within limits
        /// </summary>
        /// <param name="inputRackCapacity"></param>
        /// <returns></returns>
        public static bool ParseInputRackCapacity(int inputRackCapacity)
        {
            // Assume fail
            bool ret = false;

            if((inputRackCapacity <= MAX_INPUT_RACK_CAPACITY) && (inputRackCapacity >= MIN_INPUT_RACK_CAPACITY))
            {
                ret = true;
            }

            return ret;
        }

        
        /// <summary>
        /// Ensure the specified input rack capacity is within limits
        /// </summary>
        /// <param name="outputRackCapacity"></param>
        /// <returns></returns>
        public static bool ParseOutputRackCapacity(int outputRackCapacity)
        {
            // Assume fail
            bool ret = false;

            if ((outputRackCapacity <= MAX_OUTPUT_RACK_CAPACITY) && (outputRackCapacity >= MIN_OUTPUT_RACK_CAPACITY))
            {
                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// Helper to calculate maximum integer division of input vial possible
        /// </summary>
        /// <param name="inputRackCapacity"></param>
        /// <param name="outputRackCapacity"></param>
        /// <returns></returns>
        public static uint GetMaxOutputDivFactor(int inputRackCapacity, int outputRackCapacity)
        {
            if(0 == inputRackCapacity)
            {
                inputRackCapacity = 1;
            }

            return (uint)(outputRackCapacity / inputRackCapacity);
        }

        /// <summary>
        /// Ensure the specified output division factor is within limits
        /// </summary>
        /// <param name="divFactor"></param>
        /// <returns>
        /// -1 = special case, there are more input vials than output vials so assume div of 1
        /// 0 = set correctly
        /// 1 = out of bounds 
        /// </returns>
        public static int ParseOutputDivisionFactor(int outputDivFactor, int inputRackCapacity, int outputRackCapacity)
        {
            // Assume fail
            int ret = 1;

            if(outputRackCapacity <= inputRackCapacity)
            {
                if (outputDivFactor != 1)
                {
                    // In this case there are more input vials than output vials so output division factor must be 1.
                    ret = -1;
                }
                else
                {
                    ret = 0;
                }
            }
            else if ((outputDivFactor >= MIN_OUTPUT_DIV_FACTOR) && (outputDivFactor <= GetMaxOutputDivFactor(inputRackCapacity, outputRackCapacity)))
            {
                ret = 0;
            }
            else
            {
                // N/A
            }
            return ret;
        }

        /// <summary>
        /// Ensure Error of balance is positive. This is in mg.
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool ParseBalanceError(float balError)
        {            
            // Assume fail
            bool ret = false;

            if ((balError >= MIN_BALANCE_ERROR_mg))
            {
                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// Ensure the specified target output weight is within limits. Must have set BalaceError first
        /// TargetOutputVialWeight is in mg.
        /// </summary>
        /// <param name="targetWeight"></param>
        /// <returns>
        /// -1 = special case, the specified output is less than the balance error
        /// 0 = set correctly
        /// 1 = out of bounds 
        ///</returns>
        public static int ParseTargetOutputWeight(float targetOutput, float balError)
        {
            // Assume fail
            int ret = 1;

            if(targetOutput < balError)
            {
                ret = -1;
            }
            else if ((targetOutput >= MIN_TARGET_OUTPUT_WEIGHT_mg) && (targetOutput <= MAX_TARGET_OUTPUT_WEIGHT_mg))
            {
                ret = 0;
            }
            else
            {
                //N/A
            }
            return ret;
        }
        /// <summary>
        /// Ensure the specified target output weight is within limits. Must have set BalaceError first
        /// </summary>
        /// <param name="targetWeight"></param>
        /// <returns></returns>
        public static bool ParseFlowRate(int flowRate)
        {
            // Assume fail
            bool ret = false;
            
            if ((flowRate <= MAX_FLOW_RATE_mgs) && (flowRate >= MIN_FLOW_RATE_mgs))
            {
                ret = true;
            }
            
            return ret;
        }
        #endregion
    }
}
