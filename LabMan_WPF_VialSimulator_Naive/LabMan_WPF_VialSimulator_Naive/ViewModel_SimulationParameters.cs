using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LabMan_WPF_VialSimulator_Naive
{
    public class ViewModel_SimulationParameters : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        #region Public Properties
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

        #region Commands

        #endregion

        public ViewModel_SimulationParameters(uint inputRackCapacity, uint outputRackCapacity, uint outputDivisionFactor,
            float targetWeight, float dispenserRate, float balError)
        {
            InputRackCapacity = inputRackCapacity;
            OutputRackCapacity = outputRackCapacity;
            OutputDivisionFactor = outputDivisionFactor;
            TargetOutputVialWeight_mg = targetWeight;
            DispenserFlowRate_mgs = dispenserRate;
            BalanceError_mg = balError;
        }
    }
}
