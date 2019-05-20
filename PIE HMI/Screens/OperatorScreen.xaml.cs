using PIE_HMI.Util;
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

namespace PIE_HMI.Screens
{
    /// <summary>
    /// Interaction logic for OperatorScreen.xaml
    /// </summary>
    public partial class OperatorScreen : UserControl
    {

        private Communication SPIComms;

        public OperatorScreen()
        {
            InitializeComponent();
            SPIComms = Communication.Instance;
            xAxisStep.init(0, "mm");
            z1AxisStep.init(0, "mm");
            z2AxisStep.init(0, "mm");
            drillValue.init(0, "RPM");
        }

        private void enableControl()
        {
            axesJog.IsEnabled = true;
            jogControls.IsEnabled = true;
        }
       
        private void disableControl()
        {
            axesJog.IsEnabled = false;
            jogControls.IsEnabled = false;
        }
        
        public void updateMachineState(int state)
        {
            switch (state)
            {
                case -1:
                    machineState.Content = "NOT CONNECTED";
                    disableControl();
                    break;
                case 0:
                    machineState.Content = "STOPPED";
                    disableControl();
                    break;
                case 10:
                    machineState.Content = "OPERATOR CONTROL STATE";
                    enableControl();
                    break;
                case 50:
                    machineState.Content = "INIT AUTONOMOUS STATE";
                    disableControl();
                    break;
                case 100:
                    machineState.Content = "DRILLING AND CORING STATE";
                    disableControl();
                    break;
                case 150:
                    machineState.Content = "WATER EXTRACTION STATE";
                    disableControl();
                    break;
                case 200:
                    machineState.Content = "FILTRATION STATE";
                    disableControl();
                    break;
            }
        }

        private void StartAuton_Click(object sender, RoutedEventArgs e)
        {
            if (SPIComms.Connected())
            {
                SPIComms.global.advanceState = 1;
                SPIComms.WriteGlobal();
            }
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            if(SPIComms.Connected())
            {
                SPIComms.global.running = 0;
                SPIComms.WriteGlobal();
            }
        }

        private void jogXFwd_Click(object sender, RoutedEventArgs e)
        {
            double jogX = xAxisStep.getNumeric();

            if(SPIComms.Connected())
            {
                SPIComms.global.jogX = jogX;
                SPIComms.WriteGlobal();
            }
        }

        private void jogXRev_Click(object sender, RoutedEventArgs e)
        {
            double jogX = -xAxisStep.getNumeric();

            if (SPIComms.Connected())
            {
                SPIComms.global.jogX = jogX;
                SPIComms.WriteGlobal();
            }
        }

        private void jogZ1Fwd_Click(object sender, RoutedEventArgs e)
        {
            double jogZ1 = z1AxisStep.getNumeric();

            if (SPIComms.Connected())
            {
                SPIComms.global.jogZ1 = jogZ1;
                SPIComms.WriteGlobal();
            }
        }

        private void jogZ1Rev_Click(object sender, RoutedEventArgs e)
        {
            double jogZ1 = -z1AxisStep.getNumeric();

            if (SPIComms.Connected())
            {
                SPIComms.global.jogZ1 = jogZ1;
                SPIComms.WriteGlobal();
            }
        }

        private void jogZ2Fwd_Click(object sender, RoutedEventArgs e)
        {
            double jogZ2 = z2AxisStep.getNumeric();

            if (SPIComms.Connected())
            {
                SPIComms.global.jogZ2 = jogZ2;
                SPIComms.WriteGlobal();
            }
        }

        private void jogZ2Rev_Click(object sender, RoutedEventArgs e)
        {
            double jogZ2 = -z2AxisStep.getNumeric();

            if (SPIComms.Connected())
            {
                SPIComms.global.jogZ2 = jogZ2;
                SPIComms.WriteGlobal();
            }
        }

        private void drillJogStart_Click(object sender, RoutedEventArgs e)
        {
            double drill = 0;
            int direction = (bool)drillJogDir.IsChecked? -1 : 1;

            if(SPIComms.Connected())
            {

                if ((string)drillJogStart.Content == "Off")
                {
                    drill = 0;
                    drillJogStart.Content = "On";
                }
                else
                {
                    drill = direction * drillValue.getNumeric();
                    drillJogStart.Content = "Off";
                }
                
                SPIComms.global.jogDrill = drill;
                SPIComms.WriteGlobal();
            }
        }

        private void DrillValue_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (SPIComms == null) return;
            if (drillJogDir == null) return;

            double drill = 0;
            int direction = (bool)drillJogDir.IsChecked ? -1 : 1;


            if (SPIComms.Connected())
            {

                if ((string)drillJogStart.Content == "Off")
                {
                    drill = direction * drillValue.getNumeric();
                }

                SPIComms.global.jogDrill = drill;
            }
        }

        private void HomeX_Click(object sender, RoutedEventArgs e)
        {
            if (SPIComms.Connected())
            {
                SPIComms.global.homeX = 1;
                SPIComms.WriteGlobal();
            }
        }

        private void HomeZ1_Click(object sender, RoutedEventArgs e)
        {
            if (SPIComms.Connected())
            {
                SPIComms.global.homeZ1 = 1;
                SPIComms.WriteGlobal();
            }
        }

        private void HomeZ2_Click(object sender, RoutedEventArgs e)
        {
            if (SPIComms.Connected())
            {
                SPIComms.global.homeZ2 = 1;
                SPIComms.WriteGlobal();
            }
        }
    }

}
