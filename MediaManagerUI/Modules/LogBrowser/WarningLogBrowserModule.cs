using MediaManager.Services;

namespace MediaManagerUI.Modules.LogBrowser
{
    public class WarningLogBrowserModule(ILogServices _logServices) : LogBrowserModuleBase, ILogBrowserModule
    {
        public async Task GetResultsAsync(DateTime startDateTime, DateTime endDateTime)
        {
            Results = [];

            IsLoading = true;
            await Task.Run(() =>
            {
                Results = _logServices.GetWarnings(startDateTime, endDateTime);
            });

            IsLoading = false;
        }
    }
}
