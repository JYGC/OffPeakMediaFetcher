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

namespace FetcherManager.Tabs.Logs.Subtabs
{
    /// <summary>
    /// Interaction logic for UCWarnings.xaml
    /// </summary>
    public partial class UCWarnings : UserControl
    {
        public UCWarnings()
        {
            InitializeComponent();
            __PrepareChildUserControls();
        }

        private void __PrepareChildUserControls()
        {
            uc_LogBrowser.btn_GetLogs.Content = "Get Warnings";

            uc_LogBrowser.GetLogs = () =>
            {
                IEnumerable<OPMF.Entities.IOPMFLog> logs = new OPMF.Entities.IOPMFLog[] { };

                OPMF.Database.DatabaseAdapter.AccessDbAdapter(dbAdapter =>
                {
                    logs = dbAdapter.OPMFLogDbCollection.GetWarnings();
                });

                return logs;
            };
        }
    }
}
