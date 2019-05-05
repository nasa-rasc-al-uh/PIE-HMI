using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
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

namespace PIE_HMI.Graphs
{
    /// <summary>
    /// Interaction logic for DrillGraph.xaml
    /// </summary>
    public partial class DrillGraph : UserControl
    {
        public DrillGraph()
        {
            InitializeComponent();

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Weight on Bit (WOB)",
                    Values = new ChartValues<ObservablePoint>()
                }
            };



            SeriesCollection[0].Values.Add(new ObservablePoint(50d, -10d));

            SeriesCollection[0].Values.Add(new ObservablePoint(60d, -20d));

            SeriesCollection[0].Values.Add(new ObservablePoint(70d, -30d));

            SeriesCollection[0].Values.Add(new ObservablePoint(70d, -40d));

            SeriesCollection[0].Values.Add(new ObservablePoint(70d, -50d));

            SeriesCollection[0].Values.Add(new ObservablePoint(120d, -60d));
            SeriesCollection[0].Values.Add(new ObservablePoint(120d, -70d));
            SeriesCollection[0].Values.Add(new ObservablePoint(120d, -80d));


            SeriesCollection[0].Values.Add(new ObservablePoint(100d, -90d));
            SeriesCollection[0].Values.Add(new ObservablePoint(100d, -99d));

            

            //Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May" };
            //YFormatter = value => value.ToString("C");

            //modifying the series collection will animate and update the chart
            /*seriesCollection.Add(new LineSeries
            {
                Title = "Series 4",
                Values = new ChartValues<double> { 5, 3, 2, 4 },
                LineSmoothness = 0, //0: straight lines, 1: really smooth lines
                PointGeometry = Geometry.Parse("m 25 70.36218 20 -28 -20 22 -8 -6 z"),
                PointGeometrySize = 50,
                PointForeground = Brushes.Gray
            });*/

            DataContext = this;
        }

        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> YFormatter { get; set; }
    }
}
