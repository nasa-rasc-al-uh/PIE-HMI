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
using LiveCharts;
using LiveCharts.Wpf;

namespace PIE_HMI
{
    /// <summary>
    /// Interaction logic for PowerGraph.xaml
    /// </summary>
    public partial class PowerGraph : UserControl
    {
        public PowerGraph()
        {
            InitializeComponent();

            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Total",
                    Values = new ChartValues<double>()
                },
                new LineSeries
                {
                    Title = "Frame",
                    Values = new ChartValues<double>()
                },
                new LineSeries
                {
                    Title = "Drill",
                    Values = new ChartValues<double>()
                },
                new LineSeries
                {
                    Title = "Water Ex",
                    Values = new ChartValues<double>()
                },
                new LineSeries
                {
                    Title = "Filtration",
                    Values = new ChartValues<double>()
                }
            };


            SeriesCollection[0].Values.Add(400d);
            SeriesCollection[1].Values.Add(300d);
            SeriesCollection[2].Values.Add(200d);
            SeriesCollection[3].Values.Add(100d);
            SeriesCollection[4].Values.Add(0d);

            SeriesCollection[0].Values.Add(500d);
            SeriesCollection[1].Values.Add(400d);
            SeriesCollection[2].Values.Add(300d);
            SeriesCollection[3].Values.Add(200d);
            SeriesCollection[4].Values.Add(100d);

            SeriesCollection[0].Values.Add(400d);
            SeriesCollection[1].Values.Add(300d);
            SeriesCollection[2].Values.Add(200d);
            SeriesCollection[3].Values.Add(100d);
            SeriesCollection[4].Values.Add(0d);

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
