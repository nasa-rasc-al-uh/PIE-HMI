﻿using PIE_HMI.Util;
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
            SPIComms = new Communication();
            SPIComms.Init();
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

        private void StartAuton_Click(object sender, RoutedEventArgs e)
        {
            if (SPIComms.Connected())
            {
                SPIComms.WriteVariable((object)1, "advanceState");
            }
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            if(SPIComms.Connected())
            {
                SPIComms.WriteVariable((object)0, "running");
            }
        }

        private void jogXFwd_Click(object sender, RoutedEventArgs e)
        {
            double jogX = Double.Parse(xAxisStep.Text);

            if(SPIComms.Connected())
            {
                SPIComms.WriteVariable((object)jogX, "jogX");
            }
        }

        private void jogXRev_Click(object sender, RoutedEventArgs e)
        {
            double jogX = -Double.Parse(xAxisStep.Text);

            if (SPIComms.Connected())
            {
                SPIComms.WriteVariable((object)jogX, "jogX");
            }
        }

        private void jogZ1Fwd_Click(object sender, RoutedEventArgs e)
        {
            double jogZ1 = Double.Parse(z1AxisStep.Text);

            if (SPIComms.Connected())
            {
                SPIComms.WriteVariable((object)jogZ1, "jogZ1");
            }
        }

        private void jogZ1Rev_Click(object sender, RoutedEventArgs e)
        {
            double jogZ1 = -Double.Parse(z1AxisStep.Text);

            if (SPIComms.Connected())
            {
                SPIComms.WriteVariable((object)jogZ1, "jogZ1");
            }
        }

        private void jogZ2Fwd_Click(object sender, RoutedEventArgs e)
        {
            double jogZ2 = Double.Parse(z2AxisStep.Text);

            if (SPIComms.Connected())
            {
                SPIComms.WriteVariable((object)jogZ2, "jogZ2");
            }
        }

        private void jogZ2Rev_Click(object sender, RoutedEventArgs e)
        {
            double jogZ2 = -Double.Parse(z2AxisStep.Text);

            if (SPIComms.Connected())
            {
                SPIComms.WriteVariable((object)jogZ2, "jogZ2");
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
                    drill = direction*Double.Parse(drillValue.Text);
                    drillJogStart.Content = "Off";
                }

                SPIComms.WriteVariable((object)drill, "jogDrill");
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
                    drill = direction * Double.Parse(drillValue.Text);
                }

                SPIComms.WriteVariable((object)drill, "jogDrill");
            }
        }
    }

}