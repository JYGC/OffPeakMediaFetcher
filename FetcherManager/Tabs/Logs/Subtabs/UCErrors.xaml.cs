using OPMF.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace FetcherManager.Tabs.Logs.Subtabs
{
    /// <summary>
    /// Interaction logic for UCErrors.xaml
    /// </summary>
    public partial class UCErrors : UserControl
    {
        public UCErrors()
        {
            InitializeComponent();
            __PrepareChildUserControls();
        }

        private void __PrepareChildUserControls()
        {
            uc_LogBrowser.btn_GetLogs.Content = "Get Errors";

            uc_LogBrowser.GetLogs = () =>
            {
                IEnumerable<IOPMFLog> logs = new IOPMFLog[] { };

                OPMF.Database.DatabaseAdapter.AccessDbAdapter(dbAdapter =>
                {
                    logs = dbAdapter.OPMFLogDbCollection.GetErrors().Select(l => __ConvertLogToOPMFError(l)).OrderByDescending(l => l.DateCreated);
                });

                return logs;
            };
        }

        private OPMFError __ConvertLogToOPMFError(IOPMFLog oPMFLog)
        {
            return new OPMFError
            {
                Id = oPMFLog.Id,
                Message = oPMFLog.Message,
                Type = oPMFLog.Type,
                Variables = oPMFLog.Variables,
                DateCreated = oPMFLog.DateCreated,
                ExceptionObject = ((OPMFError)oPMFLog).ExceptionObject
            };
        }
    }
}
