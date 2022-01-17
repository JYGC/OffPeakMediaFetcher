using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FetcherManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            OPMF.OSCompat.EnvironmentHelper.EstablishEnvironment();
            OPMF.Settings.ConfigHelper.InitReadonlySettings();
            OPMF.Settings.ConfigHelper.EstablishConfig();

            InitializeComponent();
        }
    }
}
