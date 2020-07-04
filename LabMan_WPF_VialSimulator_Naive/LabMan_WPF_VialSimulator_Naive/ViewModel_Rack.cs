using System.Collections.Generic;
using System.ComponentModel;

namespace LabMan_WPF_VialSimulator_Naive
{
    public enum RackState
    {
        RACK_STATE_POSN_EMPTY,
        RACK_STATE_POSN_FULL
    }

    public enum RackPurpose
    {
        RACK_INPUT,
        RACK_OUTPUT,
    }

    public class ViewModel_Rack : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public RackPurpose Position
        {
            get;
            set;
        }

        public List<List<Model_Vial>> Vials
        {
            get;
            set;
        }

        public ViewModel_Rack(List<List<Model_Vial>> vials, RackPurpose position)
        {
            Position = position;
            Vials = vials;
        }
    }
}
