using System.ComponentModel;
namespace LabMan_WPF_VialSimulator_Naive
{
    public class ViewModel_Vial : INotifyPropertyChanged
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
        private uint _ID;
        public uint ID
        {
            get
            {
                return _ID;
            }
            set
            {
                if (value == _ID)
                    return;

                _ID = value;
                OnPropertyChanged("ID");
            }
        }

        private Model_Vial.VialState _State;
        public Model_Vial.VialState State
        {
            get
            {
                return _State;
            }
            set
            {
                if (value == _State)
                    return;

                _State = value;
                OnPropertyChanged("State");
            }
        }

        private Model_Vial.VialPurpose _Position;
        public Model_Vial.VialPurpose Position
        {
            get
            {
                return _Position;
            }
            set
            {
                if (value == _Position)
                    return;

                _Position = value;
                OnPropertyChanged("Position");
            }
        }

        private float _Weight;
        public float Weight
        {
            get
            {
                return _Weight;
            }
            set
            {
                if (value == _Weight)
                    return;

                _Weight = value;
                OnPropertyChanged("Weight");
            }
        }
        public object DataContext { get; internal set; }
        #endregion
        
        #region Constructor
        public ViewModel_Vial(uint id, Model_Vial.VialState state, Model_Vial.VialPurpose posn)
        {
            ID = id;
            State = state;
            Position = posn;
        }
        #endregion

        #region Public Mehods
        public void SetWeight(float weight)
        {
            if(weight >= 0.0f)
            {
                _Weight = weight;
            }
        }
        #endregion
    }
}
