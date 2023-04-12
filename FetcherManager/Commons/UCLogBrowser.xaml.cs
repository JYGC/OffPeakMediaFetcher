using OPMF.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FetcherManager.Commons
{
    /// <summary>
    /// Interaction logic for UCLogBrowser.xaml
    /// </summary>
    public partial class UCLogBrowser : UserControl
    {
        private IEnumerable<IOPMFLog> __logs = new List<IOPMFLog>();

        public Func<int, int, IEnumerable<IOPMFLog>> GetLogs { get; set; }

        public UCLogBrowser()
        {
            InitializeComponent();
        }

        private void __btn_GetLogs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                __skip = 0;
                __logs = GetLogs(__skip, __pageSize);
                dg_Logs.ItemsSource = __logs;
                dg_Logs.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                OPMF.TextLogging.TextLog.GetCurrent().LogEntry(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error");
            }
        }

        #region Paging
        private const int __pageSize = 15;
        private int __skip = 0;

        public bool DisablePaging { get; set; } = false;

        private void __btn_Back_Click(object sender, RoutedEventArgs e)
        {
            __skip = __skip - __pageSize;
            __skip = __skip < 0 ? 0 : __skip;
            __logs = new ObservableCollection<IOPMFLog>(GetLogs(__skip, __pageSize));
            dg_Logs.ItemsSource = __logs;
            dg_Logs.SelectedIndex = 0;
        }

        private void __btn_Forward_Click(object sender, RoutedEventArgs e)
        {
            if (__logs.Count() < __pageSize) return;
            __skip += __pageSize;
            __logs = new ObservableCollection<IOPMFLog>(GetLogs(__skip, __pageSize));
            dg_Logs.ItemsSource = __logs;
            dg_Logs.SelectedIndex = 0;
        }
        #endregion

        private void __dg_Logs_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                if (dg_Logs.SelectedItem is IOPMFLog log)
                {
                    lbl_Message.Text = log.Message;
                    lbl_ExceptionObject.Text = (log is OPMFError) ? ((OPMFError)log).ExceptionObject : "";
                }
            }
            catch (Exception ex)
            {
                OPMF.TextLogging.TextLog.GetCurrent().LogEntry(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
