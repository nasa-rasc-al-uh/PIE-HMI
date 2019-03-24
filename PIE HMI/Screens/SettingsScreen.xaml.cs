using ACS.SPiiPlusNET;
using PIE_HMI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace PIE_HMI.Screens
{
    /// <summary>
    /// Interaction logic for SettingsScreen.xaml
    /// </summary>
    public partial class SettingsScreen : UserControl
    {
        private Communication SPIComms;

        private int ComTypeOld;

        private BitmapImage bitConnected;
        private BitmapImage bitDisconnected;


        public SettingsScreen()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SPIComms = new Communication();
            SPIComms.Init();
            try
            {
                bitConnected = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/Icons/Green.ico", UriKind.Absolute));
                bitDisconnected = new BitmapImage(new Uri("pack://siteoforigin:,,,/Resources/Icons/Grey.ico", UriKind.Absolute));
            }
            catch(Exception ex)
            {
                LogScreen.PushMessage(ex.StackTrace, MessageType.Error);
                Console.WriteLine(ex.StackTrace);
            }

            BaudRateCmB.SelectedIndex = 0;   //  Baud rate
            ConnTypeCmB.SelectedIndex = 0;   //  Ethernet connection type
            ComTypeCmB.SelectedIndex = 0;    //  Communication medium (e.g. Serial, Ethernet)
            CommPortCmB.SelectedIndex = 0;   //  COM channel
        }

        private void updateSettings()
        {
            if (SPIComms == null) return;

            if(SPIComms.Connected())
            {
                // Update drill settings
                drillVel.Text = SPIComms.ReadVariable("drillVel").ToString();
                drillAccel.Text = SPIComms.ReadVariable("drillAccel").ToString();
                drillJerk.Text = SPIComms.ReadVariable("drillJerk").ToString();

                // Update z1 axis settings
                z1TravelVel.Text = SPIComms.ReadVariable("Z1TravelVel").ToString();
                z1DrillVel.Text = SPIComms.ReadVariable("Z1DrillVel").ToString();
                z1Accel.Text = SPIComms.ReadVariable("Z1Accel").ToString();
                z1Jerk.Text = SPIComms.ReadVariable("Z1Jerk").ToString();

                // Update z2 axis settings
                z2TravelVel.Text = SPIComms.ReadVariable("Z2TravelVel").ToString();
                z2ProbeVel.Text = SPIComms.ReadVariable("Z2ProbeVel").ToString();
                z2Accel.Text = SPIComms.ReadVariable("Z2Accel").ToString();
                z2Jerk.Text = SPIComms.ReadVariable("Z2Jerk").ToString();

                // Update X-Axis settings
                xVel.Text = SPIComms.ReadVariable("XVel").ToString();
                xAccel.Text = SPIComms.ReadVariable("XAccel").ToString();
                xJerk.Text = SPIComms.ReadVariable("XJerk").ToString();

            }
        }

        private void ComTypeCmB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Show/hide the form's controls relevant for the selected communication media
            // Set all visibility state to false
            CommPortCmB.Visibility = Visibility.Hidden;
            BaudRateCmB.Visibility = Visibility.Hidden;
            ConnTypeCmB.Visibility = Visibility.Hidden;
            RemoteAddressTB.Visibility = Visibility.Hidden;
            SlotNumberTB.Visibility = Visibility.Hidden;
            PortLb.Visibility = Visibility.Hidden;
            RateLb.Visibility = Visibility.Hidden;

            //  Turn on visibility based on user selection
            if (ComTypeCmB.SelectedIndex == 0)       // serial
            {
                CommPortCmB.Visibility = Visibility.Visible;
                BaudRateCmB.Visibility = Visibility.Visible;
                PortLb.Visibility = Visibility.Visible;
                RateLb.Visibility = Visibility.Visible;
                PortLb.Text = "Port";
                RateLb.Text = "Rate (bps)";
            }
            else if (ComTypeCmB.SelectedIndex == 1)         // Ethernet
            {
                ConnTypeCmB.Visibility = Visibility.Visible;
                RemoteAddressTB.Visibility = Visibility.Visible;
                PortLb.Visibility = Visibility.Visible;
                RateLb.Visibility = Visibility.Visible;
                PortLb.Text = "Remote Address";
                RateLb.Text = "Connection";
            }
            else if (ComTypeCmB.SelectedIndex == 2)     // PCI
            {
                SlotNumberTB.Visibility = Visibility.Visible;
                PortLb.Visibility = Visibility.Visible;
                PortLb.Text = "Slot Number";
            }
        }

        private void DisconnectBtn_Click(object sender, RoutedEventArgs e)
        {
            if(SPIComms.Connected())
            {

                SPIComms.Disconnect();
                if (!SPIComms.Connected())
                    ConnectionState.Source = bitDisconnected;
            }
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            //if communication is closed
            if (!SPIComms.Connected())
            {
                try
                {
                    if (ComTypeCmB.SelectedIndex == 0)                  //  Serial
                    {
                        int Baud_Rate;
                        if (BaudRateCmB.SelectedValue.ToString() == "Auto")
                            Baud_Rate = -1;
                        else
                            Baud_Rate = Convert.ToInt32(BaudRateCmB.SelectedValue.ToString());

                        // Open serial communuication.
                        // CommPortCmB.SelectedIndex + 1 defines the COM port.
                        SPIComms.OpenCommSerial(CommPortCmB.SelectedIndex + 1, Baud_Rate);
                    }
                    else if (ComTypeCmB.SelectedIndex == 1)             //  Ethernet
                    {
                        int Protocol;
                        if (ConnTypeCmB.SelectedIndex == 0)             //  Point to Point
                            Protocol = (int)EthernetCommOption.ACSC_SOCKET_DGRAM_PORT;
                        else
                            Protocol = (int)EthernetCommOption.ACSC_SOCKET_STREAM_PORT;
                        // Open ethernet communuication.
                        // RemoteAddress.Text defines the controller's TCP/IP address.
                        // Protocol is TCP/IP in case of network connection, and UDP in case of point-to-point connection.
                        SPIComms.OpenCommNetwork(RemoteAddressTB.Text, Protocol);
                    }
                    else if (ComTypeCmB.SelectedIndex == 2)             //  PCI
                    {

                        //Open PCI Bus communuication
                        int SlotNumber = Convert.ToInt32(SlotNumberTB.Text);
                        SPIComms.OpenCommPCI(SlotNumber);
                    }
                    else //(CComTypeCmB.SelectedIndex == 3)
                         //Open communuication with Simulator
                        SPIComms.OpenCommSimulator();

                    // Save new communication media
                    ComTypeOld = ComTypeCmB.SelectedIndex;
                    if (SPIComms.Connected())
                    {
                        ConnectionState.Source = bitConnected;
                        updateSettings();
                    }

                }
                catch (Exception Ex)
                {
                    LogScreen.PushMessage(Ex.StackTrace, MessageType.Error);
                    Console.WriteLine(Ex);			//  Throw exception if this fails
                }
            }
        }

        private void UpdateDrillVel_Click(object sender, RoutedEventArgs e)
        {
            if (SPIComms == null) return;

            if(SPIComms.Connected())
            {
                SPIComms.WriteVariable((object)Double.Parse(drillVel.Text), "s_drillVel");
            }
        }

        private void UpdateDrillAccel_Click(object sender, RoutedEventArgs e)
        {
            if (SPIComms == null) return;

            if (SPIComms.Connected())
            {
                SPIComms.WriteVariable((object)Double.Parse(drillAccel.Text), "s_drillAccel");
            }
        }

        private void UpdateDrillJerk_Click(object sender, RoutedEventArgs e)
        {
            if (SPIComms == null) return;

            if (SPIComms.Connected())
            {
                SPIComms.WriteVariable((object)Double.Parse(drillJerk.Text), "s_drillJerk");
            }
        }

        private void UpdateZ1TravelVel_Click(object sender, RoutedEventArgs e)
        {
            if (SPIComms == null) return;

            if (SPIComms.Connected())
            {
                SPIComms.WriteVariable((object)Double.Parse(z1TravelVel.Text), "s_Z1TravelVel");
            }
        }

        private void UpdateZ1DrillVel_Click(object sender, RoutedEventArgs e)
        {
            if (SPIComms == null) return;

            if (SPIComms.Connected())
            {
                SPIComms.WriteVariable((object)Double.Parse(z1DrillVel.Text), "Z1DrillVel");
            }
        }

        private void UpdateZ1Accel_Click(object sender, RoutedEventArgs e)
        {
            if (SPIComms == null) return;

            if (SPIComms.Connected())
            {
                SPIComms.WriteVariable((object)Double.Parse(z1Accel.Text), "s_Z1Accel");
            }
        }

        private void UpdateZ1Jerk_Click(object sender, RoutedEventArgs e)
        {
            if (SPIComms == null) return;

            if (SPIComms.Connected())
            {
                SPIComms.WriteVariable((object)Double.Parse(z1Jerk.Text), "s_Z1Jerk");
            }
        }

        private void UpdateZ2TravelVel_Click(object sender, RoutedEventArgs e)
        {
            if (SPIComms == null) return;

            if(SPIComms.Connected())
            {
                SPIComms.WriteVariable((object)Double.Parse(z2TravelVel.Text), "s_Z2TravelVel");
            }
        }

        private void UpdateZ2Accel_Click(object sender, RoutedEventArgs e)
        {
            if (SPIComms == null) return;

            if (SPIComms.Connected())
            {
                SPIComms.WriteVariable((object)Double.Parse(z2Accel.Text), "s_Z2Accel");
            }
        }

        private void UpdateZ2Jerk_Click(object sender, RoutedEventArgs e)
        {
            if (SPIComms == null) return;

            if (SPIComms.Connected())
            {
                SPIComms.WriteVariable((object)Double.Parse(z2Jerk.Text), "s_Z2Jerk");
            }
        }

        private void UpdateXlVel_Click(object sender, RoutedEventArgs e)
        {
            if (SPIComms == null) return;

            if (SPIComms.Connected())
            {
                SPIComms.WriteVariable((object)Double.Parse(xVel.Text), "s_XVel");
            }
        }

        private void UpdateXAccel_Click(object sender, RoutedEventArgs e)
        {
            if (SPIComms == null) return;

            if (SPIComms.Connected())
            {
                SPIComms.WriteVariable((object)Double.Parse(xAccel.Text), "s_XAccel");
            }
        }

        private void UpdateXJerk_Click(object sender, RoutedEventArgs e)
        {
            if (SPIComms == null) return;

            if (SPIComms.Connected())
            {
                SPIComms.WriteVariable((object)Double.Parse(xJerk.Text), "s_XJerk");
            }
        }

        private void UpdateZ2ProbeVel_Click(object sender, RoutedEventArgs e)
        {
            if (SPIComms == null) return;

            if(SPIComms.Connected())
            {
                SPIComms.WriteVariable((object)Double.Parse(z2ProbeVel.Text), "Z2ProbeVel");
            }
        }
    }
}
