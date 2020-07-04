using System.ComponentModel;

namespace LabMan_WPF_VialSimulator_Naive
{
    public class ViewModel_SimulationParameters : INotifyPropertyChanged
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
        #endregion

        #region Public Properties
        /// <summary>
        /// The number of vials the input rack can hold
        /// </summary>
        private uint _InputRackCapacity;
        public uint InputRackCapacity
        {
            get
            {
                return _InputRackCapacity;
            }
            set
            {
                if (value == _InputRackCapacity)
                    return;

                _InputRackCapacity = value;
                OnPropertyChanged("InputRackCapacity");
            }
        }

        /// <summary>
        /// The number of vials the output rack can hold
        /// </summary>
        private uint _OutputRackCapacity;
        public uint OutputRackCapacity
        {
            get
            {
                return _OutputRackCapacity;
            }
            set
            {
                if (value == _OutputRackCapacity)
                    return;
                
                _OutputRackCapacity = value;
                OnPropertyChanged("OutputRackCapacity");
            }
        }

        /// <summary>
        /// 1 input vial will be seperated into OutputDivisionFactor output vials
        /// </summary>
        private uint _OutputDivisionFactor;
        public uint OutputDivisionFactor
        {
            get
            {
                return _OutputDivisionFactor;
            }
            set
            {
                if (value == _OutputDivisionFactor)
                    return;

                _OutputDivisionFactor = value;
                OnPropertyChanged("OutputDivisionFactor");
            }
        }

        /// <summary>
        /// The target weight for an output vial
        /// </summary>
        private float _TargetOutputVialWeight_mg;
        public float TargetOutputVialWeight_mg
        {
            get
            {
                return _TargetOutputVialWeight_mg;
            }
            set
            {
                if (value == _TargetOutputVialWeight_mg)
                    return;

                _TargetOutputVialWeight_mg = value;
                OnPropertyChanged("TargetOutputVialWeight_mg");
            }
        }

        /// <summary>
        /// The assumed flow rate of the dispenser
        /// </summary>
        private float _DispenserFlowRate_mgs;
        public float DispenserFlowRate_mgs
        {
            get
            {
                return _DispenserFlowRate_mgs;
            }
            set
            {
                if (value == _DispenserFlowRate_mgs)
                    return;

                _DispenserFlowRate_mgs = value;
                OnPropertyChanged("DispenserFlowRate_mgs");
            }
        }

        /// <summary>
        /// +/- mg read by balance
        /// </summary>
        private float _BalanceError_mg;
        public float BalanceError_mg
        {
            get
            {
                return _BalanceError_mg;
            }
            set
            {
                if (value == _BalanceError_mg)
                    return;

                _BalanceError_mg = value;
                OnPropertyChanged("BalanceError_mg");
            }
        }
        #endregion

        #region Constructor
        public ViewModel_SimulationParameters(uint inputRackCapacity, 
                                                uint outputRackCapacity,
                                                uint outputDivisionFactor,
                                                float targetWeight,
                                                float dispenserRate,
                                                float balError)
        {
            InputRackCapacity = inputRackCapacity;
            OutputRackCapacity = outputRackCapacity;
            OutputDivisionFactor = outputDivisionFactor;
            TargetOutputVialWeight_mg = targetWeight;
            DispenserFlowRate_mgs = dispenserRate;
            BalanceError_mg = balError;
        }
        #endregion
    }
}
