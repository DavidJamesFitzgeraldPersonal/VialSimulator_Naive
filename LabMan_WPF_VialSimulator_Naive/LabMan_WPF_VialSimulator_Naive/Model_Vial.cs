using System.ComponentModel;
namespace LabMan_WPF_VialSimulator_Naive
{
    public enum VialState
    {
        VIAL_STATE_EMPTY,
        VIAL_STATE_COARSE,
        VIAL_STATE_FINE
    }

    public enum VialPurpose
    {
        VIAL_INPUT,
        VIAL_OUTPUT
    }
    public class Model_Vial : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
        #endregion

        public int ID
        {
            get;
            set;
        }

        public VialState State
        {
            get;
            set;
        }

        public VialPurpose Position
        {
            get;
            set;
        }
        public object DataContext { get; internal set; }

        public Model_Vial(int id, VialState state, VialPurpose posn)
        {
            ID = id;
            State = state;
            Position = posn;
        }
    }
}
