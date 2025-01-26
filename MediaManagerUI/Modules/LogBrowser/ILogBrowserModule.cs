using OPMF.Entities;

namespace MediaManagerUI.Modules.LogBrowser
{
    public interface ILogBrowserModule
    {
        bool IsLoading { get; }
        List<OPMFLog> Results { get; }
        Task GetResultsAsync(DateTime startDateTime, DateTime endDateTime);
    }
}
