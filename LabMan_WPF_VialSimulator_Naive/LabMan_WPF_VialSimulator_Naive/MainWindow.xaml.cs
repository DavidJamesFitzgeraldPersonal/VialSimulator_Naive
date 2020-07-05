using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
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

        private void UpdateDebugOutput(string message)
        {
            TextBlock_DebugOutput.AppendText(message);
            TextBlock_DebugOutput.ScrollToEnd();
        }
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
                    UpdateDebugOutput(string.Format("\n Input Rack Capacity set to {0}", _VM.SimulationParameters.InputRackCapacity));
                }
                else
                {
                    UpdateDebugOutput(string.Format("\nERROR! Input Rack Capacity must be between {0} and {1}", 
                        Model_SimulationParameters.MIN_INPUT_RACK_CAPACITY,
                        Model_SimulationParameters.MAX_INPUT_RACK_CAPACITY));
                }
            }
            catch (FormatException)
            {
                UpdateDebugOutput("\nERROR! Input Rack Capacity must be an integer!");
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
                    UpdateDebugOutput(string.Format("\n Output Rack Capacity set to {0}", _VM.SimulationParameters.OutputRackCapacity));
                }
                else
                {
                    UpdateDebugOutput(string.Format("\nERROR! Output Rack Capacity must be between {0} and {1}",
                        Model_SimulationParameters.MIN_OUTPUT_RACK_CAPACITY,
                        Model_SimulationParameters.MAX_OUTPUT_RACK_CAPACITY));
                }
            }
            catch (FormatException)
            {
                UpdateDebugOutput("\nERROR! Output Rack Capacity must be an integer!");
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
                    UpdateDebugOutput(string.Format("\n Output Division Factor set to {0}", _VM.SimulationParameters.OutputDivisionFactor));
                }
                else if(1 == result)
                {
                    UpdateDebugOutput(string.Format("\nERROR! Output Division Factor must be an integer between {0} and {1}",
                        Model_SimulationParameters.MIN_OUTPUT_DIV_FACTOR,
                        Model_SimulationParameters.GetMaxOutputDivFactor((int)_VM.SimulationParameters.InputRackCapacity, (int)_VM.SimulationParameters.OutputRackCapacity)));
                }
                else if(-1 == result)
                {
                    UpdateDebugOutput(string.Format("\nERROR! Output Rack Capacity is less than Input Capacity.\nDivision Factor must be 1!"));
                }
                else
                {
                    UpdateDebugOutput("\nERROR! Output Division Factor unknown error!");
                }
            }
            catch (FormatException)
            {
                UpdateDebugOutput("\nERROR! Output Division Factor must be an integer!");
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
                    UpdateDebugOutput(string.Format("\n Balance Error set to {0} mg", _VM.SimulationParameters.BalanceError_mg));
                }
                else
                {
                    UpdateDebugOutput(string.Format("\nERROR! Balance Error must be greater than {0} mg",
                        Model_SimulationParameters.MIN_BALANCE_ERROR_mg));
                }
            }
            catch (FormatException)
            {
                UpdateDebugOutput("\nERROR! Balance Error must be a real number!");
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
                    UpdateDebugOutput(string.Format("\n Target Output Weight set to {0} mg", _VM.SimulationParameters.TargetOutputVialWeight_mg));
                }
                else if (1 == result)
                {
                    UpdateDebugOutput(string.Format("\nERROR! Target Output Weight must be between {0} and {1} mg",
                        Model_SimulationParameters.MIN_TARGET_OUTPUT_WEIGHT_mg,
                        _VM.SimulationParameters.BalanceError_mg));
                }
                else if(-1 == result)
                {
                    UpdateDebugOutput(string.Format("\nERROR! Target Output Weight must be greater than the specified balance error of {0} mg",
                        _VM.SimulationParameters.BalanceError_mg));
                }
                else
                {
                    UpdateDebugOutput("\nERROR! Target Output Weight unknown error!");
                }
            }
            catch (FormatException)
            {
                UpdateDebugOutput("\nERROR! Target Output Weight must be a real number!");
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
                    UpdateDebugOutput(string.Format("\n Balance Error set to {0} mg", _VM.SimulationParameters.DispenserFlowRate_mgs));
                }
                else
                {
                    UpdateDebugOutput(string.Format("\nERROR! Flow Rate must be between {0} and {1} mg/s",
                        Model_SimulationParameters.MIN_FLOW_RATE_mgs,
                        Model_SimulationParameters.MAX_FLOW_RATE_mgs));
                }
            }
            catch (FormatException)
            {
                UpdateDebugOutput("\nERROR! Balance Error must be a real number!");
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
                this.Dispatcher.Invoke(new Action(() => UpdateDebugOutput(messageToAppend)));
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

        private void GenerateRacks()
        {
            #region Input Rack
            List<List<ViewModel_Vial>> InputVials = new List<List<ViewModel_Vial>>();

            uint ColCount = 20; // Nominal
            uint RowCount = _VM.SimulationParameters.InputRackCapacity / ColCount;
            uint Rem = _VM.SimulationParameters.InputRackCapacity % ColCount;
            int i = 0;

            for(i = 0; i < RowCount; i++)
            {
                InputVials.Add(new List<ViewModel_Vial>());

                for (int j = 0; j < ColCount; j++)
                {
                    ViewModel_Vial vial = new ViewModel_Vial((uint)((i * (ColCount)) + j), Model_Vial.VialState.COARSE, Model_Vial.VialPurpose.INPUT);
                    InputVials[i].Add(vial);
                }
            }

            InputVials.Add(new List<ViewModel_Vial>());
            for (int k = 0; k < Rem; k++)
            {
                ViewModel_Vial vial = new ViewModel_Vial((uint)((i * (ColCount)) + k), Model_Vial.VialState.COARSE, Model_Vial.VialPurpose.INPUT);
                InputVials[i].Add(vial);
            }

            _VM.InputRack = new ViewModel_Rack(InputVials, _VM.SimulationParameters.TargetOutputVialWeight_mg * _VM.SimulationParameters.OutputDivisionFactor,
                                                Model_Rack.RackPurpose.INPUT, _VM.SimulationParameters.InputRackCapacity, RowCount, ColCount);

            RackTemplate_Input.ItemsSource = _VM.InputRack.Vials;
            #endregion

            #region Output Racks
            List<List<ViewModel_Vial>> OutputVials = new List<List<ViewModel_Vial>>();

            RowCount = _VM.SimulationParameters.OutputRackCapacity / ColCount;
            Rem = _VM.SimulationParameters.OutputRackCapacity % ColCount;
            i = 0;

            for (i = 0; i < RowCount; i++)
            {
                OutputVials.Add(new List<ViewModel_Vial>());

                for (int j = 0; j < 20; j++)
                {
                    ViewModel_Vial vial = new ViewModel_Vial((uint)((i * (ColCount)) + j), Model_Vial.VialState.EMPTY, Model_Vial.VialPurpose.OUTPUT);
                    OutputVials[i].Add(vial);
                }
            }

            OutputVials.Add(new List<ViewModel_Vial>());
            for (int k = 0; k < Rem; k++)
            {
                ViewModel_Vial vial = new ViewModel_Vial((uint)((i * (ColCount)) + k), Model_Vial.VialState.EMPTY, Model_Vial.VialPurpose.OUTPUT);
                OutputVials[i].Add(vial);
            }

            _VM.OutputRack = new ViewModel_Rack(OutputVials, 0.0f, Model_Rack.RackPurpose.OUTPUT, _VM.SimulationParameters.OutputRackCapacity, RowCount, ColCount);

            RackTemplate_Output.ItemsSource = _VM.OutputRack.Vials;
            #endregion
        }
        private bool DoGenerateClick()
        {
            bool ret = false;

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
                UpdateDebugOutput("\n..Done!");

                //TODO CalculateTimeForSimulation();

                UpdateDebugOutput("\nGenerating Racks..");
                GenerateRacks();
                UpdateDebugOutput("\n..Done!");
                ret = true;
            }

            return ret;
        }
        private void Button_Generate_Click(object sender, RoutedEventArgs e)
        {
            TextBlock_DebugOutput.Clear();
            UpdateDebugOutput("Simulate selected. Checking input parameters..");

            DoGenerateClick();         
        }
        private void DoButtonVialClick(ViewModel_Vial selected)
        {
            //TODO - If a vial is selected and the process starts, the contents of this window will not be updated!

            TextBlock_VialInfo.Text = string.Format("Vial ID{0} from {1} rack.\nContents = {2}mg of {3}",
            selected.ID,
            selected.Position   == Model_Vial.VialPurpose.INPUT ? "Input" : "Output",
            selected.Weight,
            selected.State      == Model_Vial.VialState.COARSE ? "Coarse" :
            (selected.State     == Model_Vial.VialState.FINE ? "Ground" : "Empty"));
        }
        private void Button_Vial_Click(object sender, RoutedEventArgs e)
        {
            ViewModel_Vial selected = (ViewModel_Vial)((sender as FrameworkElement).DataContext);

            DoButtonVialClick(selected);
        }

        private void DoSimulateClick()
        {
            if (DoGenerateClick())
            {
                _VM.BeginSimulation();
            }
            else
            {
                UpdateDebugOutput("\nSimulation cannot be started - check above for incorrect parameters");
            }
        }
        private void Button_Simulate_Click(object sender, RoutedEventArgs e)
        {
            DoSimulateClick();
        }
        
        private void DoStop()
        {
            if (_VM.IsRunning)
            {
                _VM.StopSimulation();
            }
            else
            {
                UpdateDebugOutput("\nSimulation cannot be stopped - not yet running!");
            }
        }
        private void Button_Stop_Click(object sender, RoutedEventArgs e)
        {
            DoStop();
        }

        private void DoExport()
        {
            throw new NotImplementedException();
        }
        private void Button_Export_Click(object sender, RoutedEventArgs e)
        {
            DoExport();
        }
        #endregion
    }
}
