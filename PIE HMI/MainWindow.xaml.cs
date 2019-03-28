using ACS.SPiiPlusNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using LiveCharts;
using LiveCharts.Wpf;
using PIE_HMI.Util;
using System.Windows.Threading;
using PIE_HMI.Screens;

namespace PIE_HMI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Communication SPIComms;
        private DispatcherTimer telemetryTimer;

        public MainWindow()
        {
            SPIComms = Communication.Instance;
            InitializeComponent();
            telemetryTimer = new DispatcherTimer();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(SPIComms.Connected())
            {
                SPIComms.Disconnect();
            }
        }

        private static readonly Regex _numerics = new Regex("[^0-9.]+");
        private bool isNumeric(string text)
        {
            return !_numerics.IsMatch(text);
        }
        private void numericEntry(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !isNumeric(e.Text);
        }
        private void numericPasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!isNumeric(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            telemetryTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            telemetryTimer.Interval = new TimeSpan(0, 0, 1);
            telemetryTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if(SPIComms.Connected())
            {
                try
                {
                    SPIComms.ReadGlobal();
                    SPIComms.ReadPersist();

                    wobReadout.Content = SPIComms.global.drillWOB.ToString();
                    rpmReadout.Content = SPIComms.global.drillRPM.ToString();
                    heatTemp.Content = SPIComms.global.heatTemp.ToString();

                    framePwr.Content = SPIComms.global.framePwr.ToString();
                    drillPwr.Content = SPIComms.global.drillPwr.ToString();
                    waterExPwr.Content = SPIComms.global.waterExPwr.ToString();
                    filterPwr.Content = SPIComms.global.filterPwr.ToString();
                    totalPwr.Content = SPIComms.global.totalPwr.ToString();

                    int state = (int)SPIComms.global.machineState;
                    updateMachineState(state);
                }
                catch (Exception ex)
                {
                    LogScreen.PushMessage(ex.StackTrace, MessageType.Error);
                }
            }
            else
            {
                updateMachineState(-1);
            }

            // if the communicatioin is connected, disable communication controls
            if (SPIComms.Connected())
            {
                Settings.ConnTypeCmB.IsEnabled = false;
                Settings.CommPortCmB.IsEnabled = false;
                Settings.ComTypeCmB.IsEnabled = false;
                Settings.BaudRateCmB.IsEnabled = false;
                Settings.SlotNumberTB.IsEnabled = false;
                Settings.RemoteAddressTB.IsEnabled = false;
                Settings.ConnectBtn.IsEnabled = false;
            }
            else
            {
                Settings.ConnTypeCmB.IsEnabled = true;
                Settings.CommPortCmB.IsEnabled = true;
                Settings.ComTypeCmB.IsEnabled = true;
                Settings.BaudRateCmB.IsEnabled = true;
                Settings.SlotNumberTB.IsEnabled = true;
                Settings.RemoteAddressTB.IsEnabled = true;
                Settings.ConnectBtn.IsEnabled = true;
            }
        }

        //TODO: Move this to Operator.xaml.cs, change Messages instead of integer states
        private void updateMachineState(int state)
        {
            switch (state)
            {
                case -1:
                    Operator.machineState.Content = "NOT CONNECTED";
                    break;
                case 0:
                    Operator.machineState.Content = "STOPPED";
                    break;
                case 10:
                    Operator.machineState.Content = "OPERATOR CONTROL STATE";
                    break;
                case 50:
                    Operator.machineState.Content = "INIT AUTONOMOUS STATE";
                    break;
                case 100:
                    Operator.machineState.Content = "DRILLING AND CORING STATE";
                    break;
                case 150:
                    Operator.machineState.Content = "WATER EXTRACTION STATE";
                    break;
                case 200:
                    Operator.machineState.Content = "FILTRATION STATE";
                    break;
            }
        }
    }
}
