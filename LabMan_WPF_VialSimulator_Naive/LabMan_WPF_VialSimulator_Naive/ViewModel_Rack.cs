using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace LabMan_WPF_VialSimulator_Naive
{
    public class ViewModel_Rack : INotifyPropertyChanged
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
        private Model_Rack.RackPurpose _Position;
        public Model_Rack.RackPurpose Position
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

        private List<List<ViewModel_Vial>> _Vials;
        public List<List<ViewModel_Vial>> Vials
        {
            get
            {
                return _Vials;
            }
            set
            {
                if (value == _Vials)
                    return;

                _Vials = value;
                OnPropertyChanged("Vials");
            }
        }
        public int IDInUse = 0;
        #endregion


        private int _CurrentRow = 0;
        private int _CurrentCol = 0;

        #region Constructor
        public ViewModel_Rack(List<List<ViewModel_Vial>> vials, Model_Rack.RackPurpose position)
        {
            Position = position;
            Vials = vials;
        }
        #endregion

        #region Public Methods
        public void ResetRackVars()
        {
            _CurrentRow = 0;
            _CurrentCol = 0;
            IDInUse = GetCurrentID();
        }

        public void GroundVial()
        {
            _Vials[_CurrentRow][_CurrentCol].State = Model_Vial.VialState.FINE;
        }
        #endregion

        private int GetCurrentID()
        {
            return _Vials[_CurrentRow][_CurrentCol].ID;
        }
    }
}
