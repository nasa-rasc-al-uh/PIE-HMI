using LiveCharts.Defaults;
using Microsoft.Win32;
using PIE_HMI.Graphs;
using PIE_HMI.Util;
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
using System.Windows.Threading;

namespace PIE_HMI.Screens
{
    /// <summary>
    /// Interaction logic for DataScreen.xaml
    /// </summary>
    public partial class DataScreen : UserControl
    {
        private DispatcherTimer rmsTimer;
        private Communication SPIComms;
        private CSVWriter writer;
        private bool collecting;

        public DataScreen()
        {
            InitializeComponent();
            rmsTimer = new DispatcherTimer();
            SPIComms = Communication.Instance;
            collecting = false;
        }

        private void PowerGraph_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (SPIComms == null) return;
            if (SPIComms.Connected())
            {
                SPIComms.ReadGlobal();
                double totalPwr = SPIComms.global.totalPwr;
                double framePwr = SPIComms.global.framePwr;
                double drillPwr = SPIComms.global.drillPwr;
                powerGraph.SeriesCollection[0].Values.Add(totalPwr);
                powerGraph.SeriesCollection[1].Values.Add(framePwr);
                powerGraph.SeriesCollection[2].Values.Add(drillPwr);

                if (SPIComms.global.machineState == SPIComms.global.drillState && SPIComms.global.drillState > 50)
                {
                    double ROP = SPIComms.global.ROP;
                    double depth = SPIComms.global.depth;

                    drillGraph.SeriesCollection[0].Values.Add(new ObservablePoint(ROP, depth));
                    if (collecting)
                        writer.writeDataPoint(depth, ROP);
                }
                else
                {
                    drillGraph.SeriesCollection[0].Values.Clear();
                }
            }

            if(powerGraph.SeriesCollection[0].Values.Count > 25)
                powerGraph.SeriesCollection[0].Values.RemoveAt(0);

            if (powerGraph.SeriesCollection[1].Values.Count > 25)
                powerGraph.SeriesCollection[1].Values.RemoveAt(0);

            if (powerGraph.SeriesCollection[2].Values.Count > 25)
                powerGraph.SeriesCollection[2].Values.RemoveAt(0);

            if (powerGraph.SeriesCollection[3].Values.Count > 25)
                powerGraph.SeriesCollection[3].Values.RemoveAt(0);

            if (powerGraph.SeriesCollection[4].Values.Count > 25)
                powerGraph.SeriesCollection[4].Values.RemoveAt(0);

            if (drillGraph.SeriesCollection[0].Values.Count > 100)
                drillGraph.SeriesCollection[0].Values.RemoveAt(0);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            rmsTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            rmsTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            rmsTimer.Start();
        }

        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV File (*.csv)|*.csv";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (saveFileDialog.ShowDialog() == true)
            {
                digitalCoreCSV.Content = saveFileDialog.FileName;
                writer = new CSVWriter(digitalCoreCSV.Content.ToString());
            }
        }
        
        private void digitalCoreExport_Click(object sender, RoutedEventArgs e)
        {
            if (!collecting)
            {
                if (writer == null)
                    writer = new CSVWriter(digitalCoreCSV.Content.ToString());
                collecting = true;
                digitalCoreExport.Content = "Exporting...";
                writer.writeHeader("Depth (mm)", "ROP (mm/s)");
            }
            else
            {
                collecting = false;
                writer.closeFIle();
                digitalCoreExport.Content = "Export";
            }

        }
    }
}
