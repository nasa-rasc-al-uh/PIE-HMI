using ACS.SPiiPlusNET;
using Microsoft.Win32;
using PIE_HMI.Util;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml.Serialization;

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

        public struct Settings
        {
            public int ComTypeIndex;
            public int ConnTypeIndex;
            public int BaudRateIndex;
            public int ComPortIndex;
            public string slotNumber;
            public string remoteAddress;
            public Persist persist;
        }


        public SettingsScreen()
        {
            InitializeComponent();
            drillVel.init(0, "RPM");
            drillAccel.init(0, "RPM/s");
            drillJerk.init(0, "RPM/s^2");

            z1TravelVel.init(0, "mm/s");
            z1DrillVel.init(0, "mm/s");
            z1Accel.init(0, "mm/s^2");
            z1Jerk.init(0, "mm/s^3");

            z2TravelVel.init(0, "mm/s");
            z2ProbeVel.init(0, "mm/s");
            z2Accel.init(0, "mm/s^2");
            z2Jerk.init(0, "mm/s^3");

            xVel.init(0, "mm/s");
            xAccel.init(0, "mm/s^2");
            xJerk.init(0, "mm/s^3");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SPIComms = Communication.Instance;
            try
            {
                bitConnected = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Green.ico", UriKind.Absolute));
                bitDisconnected = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/Grey.ico", UriKind.Absolute));
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

        private void updateSettings(Settings settings)
        {

            ConnTypeCmB.SelectedIndex = settings.ConnTypeIndex;
            ComTypeCmB.SelectedIndex = settings.ComTypeIndex;
            BaudRateCmB.SelectedIndex = settings.BaudRateIndex;
            CommPortCmB.SelectedIndex = settings.ComPortIndex;

            RemoteAddressTB.Text = settings.remoteAddress;
            SlotNumberTB.Text = settings.slotNumber;

            if (SPIComms == null) return;

            if (SPIComms.Connected())
            {
                SPIComms.ReadPersist();

            }

            // Update drill settings
            drillVel.setText(SPIComms.persist.drillVel.ToString());
            drillAccel.setText(SPIComms.persist.drillAccel.ToString());
            drillJerk.setText(SPIComms.persist.drillJerk.ToString());

            // Update z1 axis settings
            z1TravelVel.setText(SPIComms.persist.Z1TravelVel.ToString());
            z1DrillVel.setText(SPIComms.persist.Z1DrillVel.ToString());
            z1Accel.setText(SPIComms.persist.Z1Accel.ToString());
            z1Jerk.setText(SPIComms.persist.Z1Jerk.ToString());

            // Update z2 axis settings
            z2TravelVel.setText(SPIComms.persist.Z2TravelVel.ToString());
            z2ProbeVel.setText(SPIComms.persist.Z2ProbeVel.ToString());
            z2Accel.setText(SPIComms.persist.Z2Accel.ToString());
            z2Jerk.setText(SPIComms.persist.Z2Jerk.ToString());

            // Update X-Axis settings
            xVel.setText(SPIComms.persist.XVel.ToString());
            xAccel.setText(SPIComms.persist.XAccel.ToString());
            xJerk.setText(SPIComms.persist.XJerk.ToString());

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
                        // RemoteAddress.getNumeric() defines the controller's TCP/IP address.
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
                        updateSettings(new Settings());
                    }

                }
                catch (Exception Ex)
                {
                    LogScreen.PushMessage(Ex.StackTrace, MessageType.Error);
                    Console.WriteLine(Ex);			//  Throw exception if this fails
                }
            }
        }

        private void UpdateSettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SPIComms == null) return;

            if(SPIComms.Connected())
            {
                //Drill Axis
                SPIComms.persist.drillVel = drillVel.getNumeric();
                SPIComms.persist.drillAccel = drillAccel.getNumeric();
                SPIComms.persist.drillJerk = drillJerk.getNumeric();

                //Z1 Axis
                SPIComms.persist.Z1TravelVel = z1TravelVel.getNumeric();
                SPIComms.persist.Z1DrillVel = z1DrillVel.getNumeric();
                SPIComms.persist.Z1Accel = z1Accel.getNumeric();
                SPIComms.persist.Z1Jerk = z1Jerk.getNumeric();

                //Z2 Axis
                SPIComms.persist.Z2TravelVel = z2TravelVel.getNumeric();
                SPIComms.persist.Z2ProbeVel = z2ProbeVel.getNumeric();
                SPIComms.persist.Z2Accel = z2Accel.getNumeric();
                SPIComms.persist.Z2Jerk = z2Jerk.getNumeric();

                //X Axis
                SPIComms.persist.XVel = xVel.getNumeric();
                SPIComms.persist.XAccel = xAccel.getNumeric();
                SPIComms.persist.XJerk = xJerk.getNumeric();

                SPIComms.WritePersist();
            }
        }
        

        private void SaveSettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PIE Settings File (*.pie)|*.pie";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if(saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;

                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                TextWriter writer = new StreamWriter(fileName);

                Settings settings = new Settings();
                settings.persist = SPIComms.persist;

                settings.ComTypeIndex = ComTypeCmB.SelectedIndex;
                settings.ConnTypeIndex = ConnTypeCmB.SelectedIndex;
                settings.BaudRateIndex = BaudRateCmB.SelectedIndex;
                settings.ComPortIndex = CommPortCmB.SelectedIndex;
                settings.remoteAddress = RemoteAddressTB.Text;
                settings.slotNumber = SlotNumberTB.Text;                

                serializer.Serialize(writer, settings);

            }
        }

        private void LoadSettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PIE Settings File (*.pie)|*.pie";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Multiselect = false;

            if(openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;

                try
                {

                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    FileStream fs = new FileStream(fileName, FileMode.Open);
                    Settings settings = (Settings)serializer.Deserialize(fs);

                    SPIComms.persist = settings.persist;
                    if(SPIComms.Connected())
                        SPIComms.WritePersist();

                    updateSettings(settings);
                }
                catch(Exception ex)
                {
                    LogScreen.PushMessage(ex.StackTrace, MessageType.Warning);
                }
            }
        }
    }
}
