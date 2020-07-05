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
        
        private int _IDInUse;
        public int IDInUse
        {
            get
            {
                return _IDInUse;
            }
            set
            {
                if (value == _IDInUse)
                    return;

                _IDInUse = value;
                OnPropertyChanged("IDInUse");
            }
        }
        #endregion

        #region Private Properties
        private uint _Capacity = 0;
        private int _CurrentRow = 0;
        private int _CurrentCol = 0;
        #endregion

        #region Constructor
        public ViewModel_Rack(List<List<ViewModel_Vial>> vials, Model_Rack.RackPurpose position, uint capacity)
        {
            Position = position;
            Vials = vials;

            _Capacity = capacity;
        }
        #endregion

        #region Public Methods
        public void ResetRackVars()
        {
            _CurrentRow = 0;
            _CurrentCol = 0;
            IDInUse = GetCurrentID();
        }

        public void SetCurrentVialFull()
        {
            _Vials[_CurrentRow][_CurrentCol].State = Model_Vial.VialState.FINE;
        }

        public void SetCurrentVialEmpty()
        {
            _Vials[_CurrentRow][_CurrentCol].State = Model_Vial.VialState.EMPTY;
        }

        public bool IsCurrentVialEmpty()
        {
            bool ret = false;

            if(Model_Vial.VialState.EMPTY == _Vials[_CurrentRow][_CurrentCol].State)
            {
                ret = true;
            }

            return ret;
        }

        public void SetCurrentID(int ID)
        {
            if(ID <= _Capacity)
            {
                _CurrentCol = ID % 20;
                _CurrentRow = ID / 20;
                _IDInUse = ID;
            }
        }
        #endregion

        private int GetCurrentID()
        {
            return _IDInUse;
        }
    }
}
