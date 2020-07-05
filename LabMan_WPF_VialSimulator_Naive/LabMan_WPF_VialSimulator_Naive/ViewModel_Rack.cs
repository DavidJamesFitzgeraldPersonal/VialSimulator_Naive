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
        private uint _RowCount = 0;
        private uint _ColCount = 0;
        private int _CurrentRow = 0;
        private int _CurrentCol = 0;
        private float _Weight = 0.0f;
        #endregion

        #region Constructor
        public ViewModel_Rack(List<List<ViewModel_Vial>> vials, float weight, Model_Rack.RackPurpose position, uint capacity, uint rowCount, uint colCount)
        {
            Position = position;
            Vials = vials;

            _Capacity = capacity;
            _RowCount = rowCount;
            _ColCount = colCount;
            _Weight = weight;
        }
        #endregion

        #region Public Methods
        public void ResetRackVars()
        {
            for (int i = 0; i < _RowCount; i++)
            {
                for (int j = 0; j < _ColCount; j++)
                {
                    _CurrentRow = i;
                    _CurrentCol = j;
                    if (Model_Rack.RackPurpose.INPUT == Position)
                    {
                        SetCurrentVialFullCoarse(_Weight);
                    }
                    else
                    {
                        SetCurrentVialEmpty();
                    }
                }
            }

            SetCurrentID(0);
        }

        public void SetCurrentVialFullCoarse(float weight)
        {
            _Vials[_CurrentRow][_CurrentCol].Weight = weight;
            _Vials[_CurrentRow][_CurrentCol].State = Model_Vial.VialState.COARSE;
        }

        public void SetCurrentVialFullFine(float weight)
        {
            _Vials[_CurrentRow][_CurrentCol].Weight = weight;
            _Vials[_CurrentRow][_CurrentCol].State = Model_Vial.VialState.FINE;
        }

        public void SetCurrentVialEmpty()
        {
            _Vials[_CurrentRow][_CurrentCol].Weight = 0.0f;
            _Vials[_CurrentRow][_CurrentCol].State = Model_Vial.VialState.EMPTY;
        }

        public float GetCurrentVialWeight()
        {
            return _Vials[_CurrentRow][_CurrentCol].Weight;
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
                _CurrentCol = ID % (int)(_ColCount);
                _CurrentRow = ID / (int)(_ColCount);
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
