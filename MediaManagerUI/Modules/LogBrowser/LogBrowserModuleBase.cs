using MediaManager.Services2;
using OPMF.Entities;

namespace MediaManagerUI.Modules.LogBrowser
{
    public class LogBrowserModuleBase
    {
        public bool IsLoading { get; protected set; } = false;
        public List<OPMFLog> Results { get; protected set; } = [];
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
    }
}
