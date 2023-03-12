using OPMF.Entities;
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

namespace FetcherManager.Commons
{
    /// <summary>
    /// Interaction logic for UCLogBrowser.xaml
    /// </summary>
    public partial class UCLogBrowser : UserControl
    {
        private IEnumerable<OPMF.Entities.IOPMFLog> __logs = new List<OPMF.Entities.IOPMFLog>();

        public Func<IEnumerable<OPMF.Entities.IOPMFLog>> GetLogs { get; set; }

        public UCLogBrowser()
        {
            InitializeComponent();
        }

        private void __btn_GetLogs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                __logs = GetLogs();

                LoadingDialog loadingDialog = new LoadingDialog($"Rendering {__logs.Count()} log entries...", () =>
                {
                    dg_Logs.ItemsSource = __logs;
                    dg_Logs.SelectedIndex = 0;
                });
                loadingDialog.Show();
            }
            catch (Exception ex)
            {
                OPMF.TextLogging.TextLog.GetCurrent().LogEntry(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void __dg_Logs_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            //
        }
    }
}
