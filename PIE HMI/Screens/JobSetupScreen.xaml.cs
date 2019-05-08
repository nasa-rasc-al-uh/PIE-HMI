using Microsoft.Win32;
using PIE_HMI.Util;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml.Serialization;

namespace PIE_HMI.Screens
{
    /// <summary>
    /// Interaction logic for JobSetupScreen.xaml
    /// </summary>
    public partial class JobSetupScreen : UserControl
    {

        private Communication SPIComms;

        public struct JobSettings
        {
            public double touchRMS;
            public double peckDistance;
            public double retractDistance;

            public double xOffset;
            public double heaterTemp;

            public double currentTarget;
            public double ecTime;
            public int enableSloshing;
            public double sloshAmp;
            public double sloshFreq;
            public double sloshTime;

            public double holeSpacing;
            public int numHoles;
        }

        public JobSetupScreen()
        {
            InitializeComponent();
            SPIComms = Communication.Instance;
            
            touchRMSTb.init(0, "A");
            peckDistance.init(0, "mm");
            retractDistance.init(0, "mm");

            xOffset.init(0, "mm");
            heaterTemp.init(0, "°C");

            currentTarget.init(0, "mA");
            ecTime.init(0, "min");
            sloshAmp.init(0, "mm");
            sloshFreq.init(0, "1/s");
            sloshTime.init(0, "s");

            holeSpacing.init(0, "mm");
            numHoles.init(0);

            numHoles.AllowDecimals = false;

        }

        private void updateBtn_Click(object sender, RoutedEventArgs e)
        {
            SPIComms.global.touchRMS = touchRMSTb.getNumeric();
            SPIComms.global.peckDistance = peckDistance.getNumeric();
            SPIComms.global.retractDistance = retractDistance.getNumeric();

            SPIComms.global.xOffset = xOffset.getNumeric();
            SPIComms.global.heaterSetpoint = heaterTemp.getNumeric();

            SPIComms.global.ecAmpTarget = currentTarget.getNumeric();
            SPIComms.global.ecTime = ecTime.getNumeric();
            SPIComms.global.enableSloshing = (bool)enableSloshing.IsChecked ? 1 : 0;
            SPIComms.global.sloshAmplitude = sloshAmp.getNumeric();
            SPIComms.global.sloshFrequency = sloshFreq.getNumeric();
            SPIComms.global.sloshTime = sloshTime.getNumeric();

            SPIComms.global.holeSpacing = holeSpacing.getNumeric();
            SPIComms.global.numHoles = (int)numHoles.getNumeric();

            if(SPIComms.Connected())
            {
                SPIComms.WriteGlobal();
            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PIE Job File (*.job)|*.job";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;

                XmlSerializer serializer = new XmlSerializer(typeof(JobSettings));
                TextWriter writer = new StreamWriter(fileName);

                JobSettings settings = new JobSettings();
                
                settings.touchRMS = touchRMSTb.getNumeric();
                settings.peckDistance = peckDistance.getNumeric();
                settings.retractDistance = retractDistance.getNumeric();

                settings.xOffset = xOffset.getNumeric();
                settings.heaterTemp = heaterTemp.getNumeric();

                settings.currentTarget = currentTarget.getNumeric();
                settings.ecTime = ecTime.getNumeric();
                settings.enableSloshing = (bool)enableSloshing.IsChecked ? 1 : 0;
                settings.sloshAmp = sloshAmp.getNumeric();
                settings.sloshFreq = sloshFreq.getNumeric();
                settings.sloshTime = sloshTime.getNumeric();

                settings.holeSpacing = holeSpacing.getNumeric();
                settings.numHoles = (int)numHoles.getNumeric();

                serializer.Serialize(writer, settings);

            }
        }

        private void loadBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PIE Job File (*.job)|*.job";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;

                try
                {

                    XmlSerializer serializer = new XmlSerializer(typeof(JobSettings));
                    FileStream fs = new FileStream(fileName, FileMode.Open);
                    JobSettings settings = (JobSettings)serializer.Deserialize(fs);

                    SPIComms.global.touchRMS = settings.touchRMS;
                    SPIComms.global.peckDistance = settings.peckDistance;
                    SPIComms.global.retractDistance = settings.retractDistance;

                    SPIComms.global.xOffset = settings.xOffset;
                    SPIComms.global.heaterSetpoint = settings.heaterTemp;

                    SPIComms.global.ecAmpTarget = settings.currentTarget;
                    SPIComms.global.ecTime = settings.ecTime;
                    SPIComms.global.enableSloshing = settings.enableSloshing;
                    SPIComms.global.sloshAmplitude = settings.sloshAmp;
                    SPIComms.global.sloshFrequency = settings.sloshFreq;
                    SPIComms.global.sloshTime = settings.sloshTime;

                    SPIComms.global.holeSpacing = settings.holeSpacing;
                    SPIComms.global.numHoles = settings.numHoles;


                    if (SPIComms.Connected())
                        SPIComms.WriteGlobal();

                    updateSettings(settings);
                }
                catch (Exception ex)
                {
                    LogScreen.PushMessage(ex.StackTrace, MessageType.Warning);
                }
            }
        }

        private void updateSettings(JobSettings settings)
        {
            touchRMSTb.setText(settings.touchRMS.ToString());
            peckDistance.setText(settings.peckDistance.ToString());
            retractDistance.setText(settings.retractDistance.ToString());

            xOffset.setText(settings.xOffset.ToString());
            heaterTemp.setText(settings.heaterTemp.ToString());

            currentTarget.setText(settings.currentTarget.ToString());
            ecTime.setText(settings.ecTime.ToString());
            enableSloshing.IsChecked = settings.enableSloshing==1?true:false;
            sloshAmp.setText(settings.sloshAmp.ToString());
            sloshFreq.setText(settings.sloshFreq.ToString());
            sloshTime.setText(settings.sloshFreq.ToString());

            holeSpacing.setText(settings.holeSpacing.ToString());
            numHoles.setText(settings.numHoles.ToString());

        }
    }
}
