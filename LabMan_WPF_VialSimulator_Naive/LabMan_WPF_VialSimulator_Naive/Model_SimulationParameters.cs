using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LabMan_WPF_VialSimulator_Naive
{
    public class Model_SimulationParameters : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Constants
        public const uint MIN_INPUT_RACK_CAPACITY        = 1;
        public const uint MAX_INPUT_RACK_CAPACITY        = 300;

        public const uint MIN_OUTPUT_RACK_CAPACITY       = 1;
        public const uint MAX_OUTPUT_RACK_CAPACITY       = 300;

        public const uint MIN_OUTPUT_DIV_FACTOR          = 1;

        public const float MIN_BALANCE_ERROR_mg          = 0.0f;

        public const uint MIN_TARGET_OUTPUT_WEIGHT_mg    = 0;
        public const uint MAX_TARGET_OUTPUT_WEIGHT_mg    = 1000;

        public const uint MAX_FLOW_RATE_mgs              = 10;
        #endregion

        #region Public Members
        /// <summary>
        /// The number of vials the input rack can hold
        /// </summary>
        public uint InputRackCapacity
        {
            get;
            set;
        }

        /// <summary>
        /// The number of vials the output rack can hold
        /// </summary>
        public uint OutputRackCapacity
        {
            get;
            set;
        }

        /// <summary>
        /// 1 input vial will be seperated into OutputDivisionFactor output vials
        /// </summary>
        public uint OutputDivisionFactor
        {
            get;
            set;
        }

        /// <summary>
        /// The target weight for an output vial
        /// </summary>
        public float TargetOutputVialWeight_mg
        {
            get;
            set;
        }

        /// <summary>
        /// The assumed flow rate of the dispenser
        /// </summary>
        public float DispenserFlowRate_mgs
        {
            get;
            set;
        }

        /// <summary>
        /// +/- mg read by balance
        /// </summary>
        public float BalanceError_mg
        {
            get;
            set;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Ensure the specified input rack capacity is within limits
        /// </summary>
        /// <param name="inputRackCapacity"></param>
        /// <returns></returns>
        public bool ParseInputRackCapacity()
        {
            // Assume fail
            bool ret = false;

            if((InputRackCapacity <= MAX_INPUT_RACK_CAPACITY) && (InputRackCapacity >= MIN_INPUT_RACK_CAPACITY))
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
        public bool ParseOutputRackCapacity()
        {
            // Assume fail
            bool ret = false;

            if ((OutputRackCapacity <= MAX_OUTPUT_RACK_CAPACITY) && (OutputRackCapacity >= MIN_OUTPUT_RACK_CAPACITY))
            {
                ret = true;
            }

            return ret;
        }

        public uint GetMaxOutputDivFactor()
        {
            if(0 == InputRackCapacity)
            {
                InputRackCapacity = 1;
            }

            return (uint)(OutputRackCapacity / InputRackCapacity);
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
        public int ParseOutputDivisionFactor()
        {
            // Assume fail
            int ret = 1;

            if((OutputRackCapacity <= InputRackCapacity) && (1 != OutputDivisionFactor))
            {
                // In this case there are more input vials than output vials so output division factor must be 1.
                ret = -1;
            }
            else if ((OutputDivisionFactor >= MIN_OUTPUT_DIV_FACTOR) && (OutputDivisionFactor <= GetMaxOutputDivFactor()))
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
        public bool ParseBalanceError()
        {            
            // Assume fail
            bool ret = false;

            if ((BalanceError_mg >= MIN_BALANCE_ERROR_mg))
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
        public int ParseTargetOutputWeight()
        {
            // Assume fail
            int ret = 1;

            if(TargetOutputVialWeight_mg < BalanceError_mg)
            {
                ret = -1;
            }
            else if ((TargetOutputVialWeight_mg >= MIN_TARGET_OUTPUT_WEIGHT_mg) && (TargetOutputVialWeight_mg <= MAX_TARGET_OUTPUT_WEIGHT_mg))
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
        public bool ParseFlowRate(float flowRate)
        {
            // Assume fail
            bool ret = false;

            if ((flowRate <= MAX_FLOW_RATE_mgs) && (flowRate >= 0))
            {
                DispenserFlowRate_mgs = flowRate;
                ret = true;
            }

            return ret;
        }
        #endregion
    }
}
