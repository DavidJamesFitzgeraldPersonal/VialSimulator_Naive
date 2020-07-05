using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace LabMan_WPF_VialSimulator_Naive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ViewModel_SimulationRunner _VM;

        #region Initialiser
        public MainWindow()
        {
            InitializeComponent();
            _VM = new ViewModel_SimulationRunner();

            _VM.OnSimulationUpdateEvent += Simulation_OnSimulationUpdateEventReceived;

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

        #region UI Events and Methods
        /// <summary>
        /// On load simulate a button click to setup a test with default properties
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Simulate a button click to load a deafault test setup
            DoGenerateClick();
        }

        /// <summary>
        /// Update the debug output in a thread safe manner allowing events raised from simulation runner
        /// to be written to the output.
        /// </summary>
        /// <param name="messageToAppend"></param>
        private void AppendDebugOutput(string messageToAppend)
        {
            try
            {
                TextBlock_DebugOutput.Dispatcher.Invoke(new Action(() => TextBlock_DebugOutput.Text += messageToAppend));
            }
            catch(Exception ex)
            {
                // This is really really hacky TODO
            }
        }

        /// <summary>
        /// Raised when the simulation runner state is updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void Simulation_OnSimulationUpdateEventReceived(ViewModel_SimulationRunner sender, string message)
        {
            AppendDebugOutput(message);
        }

        /// <summary>
        /// Locks the simulation parameter inputs on start of simulations
        /// </summary>
        private void FreezeSimulationParameters()
        {
            TextBox_InputRackCapacity.Text = _VM.SimulationParameters.InputRackCapacity.ToString();
            TextBox_InputRackCapacity.IsReadOnly = true;
            TextBox_InputRackCapacity.Background = Brushes.LightGray;

            TextBox_OutputRackCapacity.Text = _VM.SimulationParameters.OutputRackCapacity.ToString();
            TextBox_OutputRackCapacity.IsReadOnly = true;
            TextBox_OutputRackCapacity.Background = Brushes.LightGray;

            TextBox_OutputDivison.Text = _VM.SimulationParameters.OutputDivisionFactor.ToString();
            TextBox_OutputDivison.IsReadOnly = true;
            TextBox_OutputDivison.Background = Brushes.LightGray;

            TextBox_BalanceError.Text = _VM.SimulationParameters.BalanceError_mg.ToString();
            TextBox_BalanceError.IsReadOnly = true;
            TextBox_BalanceError.Background = Brushes.LightGray;

            TextBox_OutputWeight.Text = _VM.SimulationParameters.TargetOutputVialWeight_mg.ToString();
            TextBox_OutputWeight.IsReadOnly = true;
            TextBox_OutputWeight.Background = Brushes.LightGray;

            TextBox_DispenserFlowRate.Text = _VM.SimulationParameters.DispenserFlowRate_mgs.ToString();
            TextBox_DispenserFlowRate.IsReadOnly = true;
            TextBox_DispenserFlowRate.Background = Brushes.LightGray;
        }
        private void GenerateRacks()
        {
            #region Input Rack
            List<List<ViewModel_Vial>> InputVials = new List<List<ViewModel_Vial>>();

            int RowCount = (int)(_VM.SimulationParameters.InputRackCapacity / 20);
            int Rem = (int)(_VM.SimulationParameters.InputRackCapacity % 20);
            int i = 0;

            for(i = 0; i < RowCount; i++)
            {
                InputVials.Add(new List<ViewModel_Vial>());

                for (int j = 0; j < 20; j++)
                {
                    ViewModel_Vial vial = new ViewModel_Vial(1+((i * (20)) + j), Model_Vial.VialState.COARSE, Model_Vial.VialPurpose.INPUT);
                    InputVials[i].Add(vial);
                }
            }

            InputVials.Add(new List<ViewModel_Vial>());
            for (int k = 0; k < Rem; k++)
            {
                ViewModel_Vial vial = new ViewModel_Vial(1+((i * (20)) + k), Model_Vial.VialState.COARSE, Model_Vial.VialPurpose.INPUT);
                InputVials[i].Add(vial);
            }

            _VM.InputRack = new ViewModel_Rack(InputVials, Model_Rack.RackPurpose.INPUT);

            RackTemplate_Input.ItemsSource = _VM.InputRack.Vials;
            #endregion

            #region Output Racks
            List<List<ViewModel_Vial>> OutputVials = new List<List<ViewModel_Vial>>();

            RowCount = (int)(_VM.SimulationParameters.OutputRackCapacity / 20);
            Rem = (int)(_VM.SimulationParameters.OutputRackCapacity % 20);
            i = 0;

            for (i = 0; i < RowCount; i++)
            {
                OutputVials.Add(new List<ViewModel_Vial>());

                for (int j = 0; j < 20; j++)
                {
                    ViewModel_Vial vial = new ViewModel_Vial(1+((i * (20)) + j), Model_Vial.VialState.EMPTY, Model_Vial.VialPurpose.OUTPUT);
                    OutputVials[i].Add(vial);
                }
            }

            OutputVials.Add(new List<ViewModel_Vial>());
            for (int k = 0; k < Rem; k++)
            {
                ViewModel_Vial vial = new ViewModel_Vial(1+((i * (20)) + k), Model_Vial.VialState.EMPTY, Model_Vial.VialPurpose.OUTPUT);
                OutputVials[i].Add(vial);
            }

            _VM.OutputRack = new ViewModel_Rack(OutputVials, Model_Rack.RackPurpose.OUTPUT);

            RackTemplate_Output.ItemsSource = _VM.OutputRack.Vials;
            #endregion
        }
        private void DoGenerateClick()
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
                GenerateRacks();
                TextBlock_DebugOutput.Text += "\n..Done!";
            }
        }
        private void Button_Generate_Click(object sender, RoutedEventArgs e)
        {
            TextBlock_DebugOutput.Text = "Simulate selected. Checking input parameters..";

            DoGenerateClick();         
        }
        private void DoButtonVialClick(ViewModel_Vial selected)
        {
            TextBlock_VialInfo.Text = string.Format("Vial {0} from {1} rack.\nContents = {2}",
            selected.ID,
            selected.Position   == Model_Vial.VialPurpose.INPUT ? "Input" : "Output",
            selected.State      == Model_Vial.VialState.COARSE ? "Coarse" :
            (selected.State     == Model_Vial.VialState.FINE ? "Ground" : "Empty"));
        }
        private void Button_Vial_Click(object sender, RoutedEventArgs e)
        {
            ViewModel_Vial selected = (ViewModel_Vial)((sender as FrameworkElement).DataContext);

            DoButtonVialClick(selected);
        }
        private void Button_Simulate_Click(object sender, RoutedEventArgs e)
        {
            // Incase the user has overridden some settings but not selected simulate..
            FreezeSimulationParameters();

            _VM.BeginSimulation();

        }
        private void Button_Stop_Click(object sender, RoutedEventArgs e)
        {
            if(_VM.IsRunning)
            {
                _VM.StopSimulation();
            }
            else
            {
                TextBlock_DebugOutput.Text += "\nSimulation cannot be stopped - not yet running!";
            }
        }
        #endregion
    }
}
