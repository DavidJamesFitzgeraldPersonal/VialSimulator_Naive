using LabMan_WPF_VialSimulator_Naive.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LabMan_WPF_VialSimulator_Naive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Model_SimulationParameters _simulationParameters;

        public MainWindow()
        {
            InitializeComponent();

            // Create a new instance of simulation parameters, populate some defaults
            _simulationParameters = new Model_SimulationParameters()
            {
                InputRackCapacity = 96,
                OutputRackCapacity = 288,
                OutputDivisionFactor = 3,
                TargetOutputVialWeight_mg = 10,
                DispenserFlowRate_mgs = 1.0f,
                BalanceError_mg = 0.0f
            };

            this.DataContext = _simulationParameters;
        }

        
        #region Test Setup - ViewModel type
        /// <summary>
        /// Check validity of input rack count, inform user if not correct
        /// </summary>
        /// <returns></returns>
        private bool SetInputRackCapacity()
        {
            bool success = false;
            try
            {
                Int32.Parse(TextBox_InputRackCapacity.Text.ToString());
                if (_simulationParameters.ParseInputRackCapacity())
                {
                    success = true;
                    TextBlock_DebugOutput.Text += string.Format("\n Input Rack Capacity set to {0}", _simulationParameters.InputRackCapacity);
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
                Int32.Parse(TextBox_OutputRackCapacity.Text.ToString());
                if (_simulationParameters.ParseOutputRackCapacity())
                {
                    success = true;
                    TextBlock_DebugOutput.Text += string.Format("\n Output Rack Capacity set to {0}", _simulationParameters.OutputRackCapacity);
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
                Int32.Parse(TextBox_OutputDivison.Text.ToString());
                result = _simulationParameters.ParseOutputDivisionFactor();
                if (0 == result)
                {
                    success = true;
                    TextBlock_DebugOutput.Text += string.Format("\n Output Division Factor set to {0}", _simulationParameters.OutputDivisionFactor);
                }
                else if(1 == result)
                {
                    TextBlock_DebugOutput.Text += string.Format("\nERROR! Output Division Factor must be an integer between {0} and {1}",
                        Model_SimulationParameters.MIN_OUTPUT_DIV_FACTOR,
                        _simulationParameters.GetMaxOutputDivFactor());
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
                float.Parse(TextBox_BalanceError.Text.ToString());
                if (_simulationParameters.ParseBalanceError())
                {
                    success = true;
                    TextBlock_DebugOutput.Text += string.Format("\n Balance Error set to {0} mg", _simulationParameters.BalanceError_mg);
                }
                else
                {
                    TextBlock_DebugOutput.Text += string.Format("\nERROR! Output Rack Capacity must be greater than {0} mg",
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
                float.Parse(TextBox_OutputWeight.Text.ToString());
                result = _simulationParameters.ParseTargetOutputWeight();
                if (0 == result)
                {
                    success = true;
                    TextBlock_DebugOutput.Text += string.Format("\n Target Output Weight set to {0} mg", _simulationParameters.TargetOutputVialWeight_mg);
                }
                else if (1 == result)
                {
                    TextBlock_DebugOutput.Text += string.Format("\nERROR! Target Output Weight must be between {0} and {1} mg",
                        Model_SimulationParameters.MIN_TARGET_OUTPUT_WEIGHT_mg,
                        _simulationParameters.BalanceError_mg);
                }
                else if(-1 == result)
                {
                    TextBlock_DebugOutput.Text += string.Format("\nERROR! Target Output Weight must be greater than the specified balance error of {0} mg",
                        _simulationParameters.BalanceError_mg);
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

        private bool SetDispenserFlowRate()
        {
            return true;
        }

        #endregion

        #region UI Events

        private void Button_Simulate_Click(object sender, RoutedEventArgs e)
        {            
            TextBlock_DebugOutput.Text = "Simulate selected. Checking input parameters..";

            if (SetInputRackCapacity() && 
                SetOutputRackCapacity() && 
                SetOutputDivisionFactor() &&
                SetBalanceError() &&
                SetDispenserFlowRate() &&
                SetTargetOutputWeight())
            {
                TextBlock_DebugOutput.Text += "\nAll input parameters set correctly!";
            }
        }
        #endregion
    }
}
