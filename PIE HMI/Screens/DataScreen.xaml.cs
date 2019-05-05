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

        public DataScreen()
        {
            InitializeComponent();
            rmsTimer = new DispatcherTimer();
            SPIComms = Communication.Instance;
        }

        private void PowerGraph_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (SPIComms == null) return;
            if (SPIComms.Connected())
            {
                double frameRms = SPIComms.global.framePwr;
                powerGraph.SeriesCollection[0].Values.Add(frameRms);
            }

            if(powerGraph.SeriesCollection[0].Values.Count > 100)
            {
                powerGraph.SeriesCollection[0].Values.RemoveAt(0);
            }
        }

        int counter = 0;
        private void DrillGraph_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DrillGraph graph = (DrillGraph)sender;
            graph.SeriesCollection[0].Values.Add(400d + 100 * ++counter);
            graph.SeriesCollection[1].Values.Add(300d + 100 * counter);
            graph.SeriesCollection[2].Values.Add(200d + 100 * counter);
            graph.SeriesCollection[3].Values.Add(100d + 100 * counter);
            graph.SeriesCollection[4].Values.Add(0d + 100 * counter);

            if (graph.SeriesCollection[0].Values.Count > 5)
            {
                graph.SeriesCollection[0].Values.RemoveAt(0);
                graph.SeriesCollection[1].Values.RemoveAt(0);
                graph.SeriesCollection[2].Values.RemoveAt(0);
                graph.SeriesCollection[3].Values.RemoveAt(0);
                graph.SeriesCollection[4].Values.RemoveAt(0);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            rmsTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            rmsTimer.Interval = new TimeSpan(0, 0, 1);
            rmsTimer.Start();
        }
    }
}
