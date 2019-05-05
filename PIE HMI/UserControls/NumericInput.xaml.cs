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

namespace PIE_HMI.UserControls
{
    /// <summary>
    /// Interaction logic for NumericInput.xaml
    /// </summary>
    public partial class NumericInput : TextBox
    {
        public NumericInput()
        {
            InitializeComponent();
        }

        public string units = "";

        public void init(double value)
        {
            units = " " + units;
            this.Text = value + units;
        }

        public double getNumeric()
        {
            string value = this.Text.Remove(this.Text.Length - units.Length);

            return Double.Parse(value);
        }

        public void setText(string text)
        {
            this.Text = text + units;
        }

        private static readonly Regex _numerics = new Regex("[^0-9.]+");
        private bool isNumeric(string text)
        {
            return !_numerics.IsMatch(text) && (this.Text + text).Count(x => x == '.') <= 1;
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

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            int len = units.Length;
            string text = ((TextBox)sender).Text;
            
            if(units.Length > 0)
                ((TextBox)sender).Text = text.Remove(text.Length - len);
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(units.Length > 0)
                ((TextBox)sender).Text += units;
        }
    }
}
