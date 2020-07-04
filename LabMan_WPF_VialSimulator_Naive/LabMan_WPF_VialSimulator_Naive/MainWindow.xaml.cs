using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace LabMan_WPF_VialSimulator_Naive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel_SimulationRunner _VM;

        #region Initialiser
        public MainWindow()
        {
            InitializeComponent();
            _VM = new ViewModel_SimulationRunner();
            this.DataContext = _VM;
        }
        #endregion

        #region View Sanity Checks
        /// <summary>
        /// Check validity of input rack count, inform user if not correct
        /// </summary>
        /// <returns></returns>
        private bool SetInputRackCapacity()
        {
            bool success = false;
            try
            {
                int value = Int32.Parse(TextBox_InputRackCapacity.Text.ToString());
                if (Model_SimulationParameters.ParseInputRackCapacity(value))
                {

                    _VM.SimulationParameters.InputRackCapacity = (uint)value;
                    success = true;
                    TextBlock_DebugOutput.Text += string.Format("\n Input Rack Capacity set to {0}", _VM.SimulationParameters.InputRackCapacity);
                }
                else
                {
                    TextBlock_DebugOutput.Text += string.Format("\nERROR! Input Rack Capacity must be between {0} and {1}", 
                        Model_SimulationParameters.MIN_INPUT_RACK_CAPACITY,
                        Model_SimulationParameters.MAX_INPUT_RACK_CAPACITY);
                }
            }
            catch (FormatException)
            {
                TextBlock_DebugOutput.Text += "\nERROR! Input Rack Capacity must be an integer!";
            }
            return success;
        }
        
        /// <summary>
        /// Check validity of output rack count, inform user if not correct
        /// </summary>
        /// <returns></returns>
        private bool SetOutputRackCapacity()
        {
            bool success = false;
            try
            {
                int value =  Int32.Parse(TextBox_OutputRackCapacity.Text.ToString());
                if (Model_SimulationParameters.ParseOutputRackCapacity(value))
                {
                    _VM.SimulationParameters.OutputRackCapacity = (uint)value;
                    success = true;
                    TextBlock_DebugOutput.Text += string.Format("\n Output Rack Capacity set to {0}", _VM.SimulationParameters.OutputRackCapacity);
                }
                else
                {
                    TextBlock_DebugOutput.Text += string.Format("\nERROR! Output Rack Capacity must be between {0} and {1}",
                        Model_SimulationParameters.MIN_OUTPUT_RACK_CAPACITY,
                        Model_SimulationParameters.MAX_OUTPUT_RACK_CAPACITY);
                }
            }
            catch (FormatException)
            {
                TextBlock_DebugOutput.Text += "\nERROR! Output Rack Capacity must be an integer!";
            }
            return success;
        }

        /// <summary>
        /// Check validity of division factor, inform user if not correct
        /// </summary>
        /// <returns></returns>
        private bool SetOutputDivisionFactor()
        {
            bool success = false;
            try
            {
                int result = 0;
                int value = Int32.Parse(TextBox_OutputDivison.Text.ToString());
                result = Model_SimulationParameters.ParseOutputDivisionFactor(value, (int)_VM.SimulationParameters.InputRackCapacity, (int)_VM.SimulationParameters.OutputRackCapacity);
                if (0 == result)
                {
                    _VM.SimulationParameters.OutputDivisionFactor = (uint)value;
                    success = true;
                    TextBlock_DebugOutput.Text += string.Format("\n Output Division Factor set to {0}", _VM.SimulationParameters.OutputDivisionFactor);
                }
                else if(1 == result)
                {
                    TextBlock_DebugOutput.Text += string.Format("\nERROR! Output Division Factor must be an integer between {0} and {1}",
                        Model_SimulationParameters.MIN_OUTPUT_DIV_FACTOR,
                        Model_SimulationParameters.GetMaxOutputDivFactor((int)_VM.SimulationParameters.InputRackCapacity, (int)_VM.SimulationParameters.OutputRackCapacity));
                }
                else if(-1 == result)
                {
                    TextBlock_DebugOutput.Text += string.Format("\nERROR! Output Rack Capacity is less than Input Capacity.\nDivision Factor must be 1!");
                }
                else
                {
                    TextBlock_DebugOutput.Text += "\nERROR! Output Division Factor unknown error!";
                }
            }
            catch (FormatException)
            {
                TextBlock_DebugOutput.Text += "\nERROR! Output Division Factor must be an integer!";
            }
            return success;
        }

        private bool SetBalanceError()
        {
            bool success = false;
            try
            {
                float value = float.Parse(TextBox_BalanceError.Text.ToString());
                if (Model_SimulationParameters.ParseBalanceError(value))
                {
                    _VM.SimulationParameters.BalanceError_mg = value;
                    success = true;
                    TextBlock_DebugOutput.Text += string.Format("\n Balance Error set to {0} mg", _VM.SimulationParameters.BalanceError_mg);
                }
                else
                {
                    TextBlock_DebugOutput.Text += string.Format("\nERROR! Balance Error must be greater than {0} mg",
                        Model_SimulationParameters.MIN_BALANCE_ERROR_mg);
                }
            }
            catch (FormatException)
            {
                TextBlock_DebugOutput.Text += "\nERROR! Balance Error must be a real number!";
            }
            return success;
        }

        /// <summary>
        /// Check validity of target weight, inform user if not correct
        /// </summary>
        /// <returns></returns>
        private bool SetTargetOutputWeight()
        {
            bool success = false;
            try
            {
                int result = 0;
                float value = float.Parse(TextBox_OutputWeight.Text.ToString());
                result = Model_SimulationParameters.ParseTargetOutputWeight(value, _VM.SimulationParameters.BalanceError_mg);
                if (0 == result)
                {
                    _VM.SimulationParameters.TargetOutputVialWeight_mg = value;
                    success = true;
                    TextBlock_DebugOutput.Text += string.Format("\n Target Output Weight set to {0} mg", _VM.SimulationParameters.TargetOutputVialWeight_mg);
                }
                else if (1 == result)
                {
                    TextBlock_DebugOutput.Text += string.Format("\nERROR! Target Output Weight must be between {0} and {1} mg",
                        Model_SimulationParameters.MIN_TARGET_OUTPUT_WEIGHT_mg,
                        _VM.SimulationParameters.BalanceError_mg);
                }
                else if(-1 == result)
                {
                    TextBlock_DebugOutput.Text += string.Format("\nERROR! Target Output Weight must be greater than the specified balance error of {0} mg",
                        _VM.SimulationParameters.BalanceError_mg);
                }
                else
                {
                    TextBlock_DebugOutput.Text += "\nERROR! Target Output Weight unknown error!";
                }
            }
            catch (FormatException)
            {
                TextBlock_DebugOutput.Text += "\nERROR! Target Output Weight must be a real number!";
            }
            return success;
        }

        /// <summary>
        /// Check validity of flow rate, inform user if not correct
        /// </summary>
        /// <returns></returns>
        private bool SetDispenserFlowRate()
        {
            bool success = false;
            try
            {
                int value = Int32.Parse(TextBox_DispenserFlowRate.Text.ToString());
                if (Model_SimulationParameters.ParseFlowRate(value))
                {
                    _VM.SimulationParameters.DispenserFlowRate_mgs = value;
                    success = true;
                    TextBlock_DebugOutput.Text += string.Format("\n Balance Error set to {0} mg", _VM.SimulationParameters.DispenserFlowRate_mgs);
                }
                else
                {
                    TextBlock_DebugOutput.Text += string.Format("\nERROR! Flow Rate must be between {0} and {1} mg/s",
                        Model_SimulationParameters.MIN_FLOW_RATE_mgs,
                        Model_SimulationParameters.MAX_FLOW_RATE_mgs);
                }
            }
            catch (FormatException)
            {
                TextBlock_DebugOutput.Text += "\nERROR! Balance Error must be a real number!";
            }
            return success;
        }

        #endregion

        #region UI Events
        /// <summary>
        /// On load simulate a button click to setup a test with default properties
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Simulate a button click to load a deafault test setup
            DoSimulateClick();
        }

        private void GeneratetRacks()
        {
            #region Input Rack
            List<List<Model_Vial>> InputVials = new List<List<Model_Vial>>();

            int RowCount = (int)(_VM.SimulationParameters.InputRackCapacity / 20);
            int Rem = (int)(_VM.SimulationParameters.InputRackCapacity % 20);
            int i = 0;

            for(i = 0; i < RowCount; i++)
            {
                InputVials.Add(new List<Model_Vial>());

                for (int j = 0; j < 20; j++)
                {
                    Model_Vial vial = new Model_Vial((i * (20)) + j, VialState.VIAL_STATE_COARSE, VialPurpose.VIAL_INPUT);
                    InputVials[i].Add(vial);
                }
            }

            InputVials.Add(new List<Model_Vial>());
            for (int k = 0; k < Rem; k++)
            {
                Model_Vial vial = new Model_Vial((i * (20)) + k, VialState.VIAL_STATE_COARSE, VialPurpose.VIAL_INPUT);
                InputVials[i].Add(vial);
            }

            _VM.InputRack = new ViewModel_Rack(InputVials, RackPurpose.RACK_INPUT);

            RackTemplate_Input.ItemsSource = _VM.InputRack.Vials;
            #endregion

            #region Output Racks
            List <List<Model_Vial>> OutputVials = new List<List<Model_Vial>>();

            RowCount = (int)(_VM.SimulationParameters.OutputRackCapacity / 20);
            Rem = (int)(_VM.SimulationParameters.OutputRackCapacity % 20);
            i = 0;

            for (i = 0; i < RowCount; i++)
            {
                OutputVials.Add(new List<Model_Vial>());

                for (int j = 0; j < 20; j++)
                {
                    Model_Vial vial = new Model_Vial((i * (20)) + j, VialState.VIAL_STATE_EMPTY, VialPurpose.VIAL_OUTPUT);
                    OutputVials[i].Add(vial);
                }
            }

            OutputVials.Add(new List<Model_Vial>());
            for (int k = 0; k < Rem; k++)
            {
                Model_Vial vial = new Model_Vial((i * (20)) + k, VialState.VIAL_STATE_EMPTY, VialPurpose.VIAL_OUTPUT);
                OutputVials[i].Add(vial);
            }

            _VM.OutputRack = new ViewModel_Rack(OutputVials, RackPurpose.RACK_OUTPUT);

            RackTemplate_Output.ItemsSource = _VM.OutputRack.Vials;
            #endregion
        }

        private void DoSimulateClick()
        {
            // Parse input parameters for validity
            if (
                    SetInputRackCapacity() &&
                    SetOutputRackCapacity() &&
                    SetOutputDivisionFactor() &&
                    SetBalanceError() &&
                    SetTargetOutputWeight() &&
                    SetDispenserFlowRate()
                )
            {
                TextBlock_DebugOutput.Text += "\n..Done!";

                //TODO CalculateTimeForSimulation();

                TextBlock_DebugOutput.Text += "\nGenerating Racks..";
                GeneratetRacks();
                TextBlock_DebugOutput.Text += "\n..Done!";
            }
        }
        private void Button_Simulate_Click(object sender, RoutedEventArgs e)
        {
            TextBlock_DebugOutput.Text = "Simulate selected. Checking input parameters..";

            DoSimulateClick();         
        }

        private void DoButtonVialClick(Model_Vial selected)
        {
            TextBlock_VialInfo.Text = string.Format("Vial {0} from {1} rack.\nContents = {2}",
            selected.ID,
            selected.Position == VialPurpose.VIAL_INPUT ? "Input" : "Output",
            selected.State == VialState.VIAL_STATE_COARSE ? "Coarse" :
            (selected.State == VialState.VIAL_STATE_FINE ? "Ground" : "Empty"));
        }

        private void Button_Vial_Click(object sender, RoutedEventArgs e)
        {
            Model_Vial selected = (Model_Vial)((sender as FrameworkElement).DataContext);

            DoButtonVialClick(selected);
        }
        #endregion
    }
}
