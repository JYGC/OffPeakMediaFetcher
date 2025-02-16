using OPMF.Database;
using OPMF.Entities;

namespace MediaManager.Services2
{
    public interface ILogServices
    {
        List<OPMFLog> GetWarnings(DateTime startDateTime, DateTime endDateTime);
        List<OPMFLog> GetErrors(DateTime startDateTime, DateTime endDateTime);
    }
    public class LogServices : ILogServices
    {
        public List<OPMFLog> GetWarnings(DateTime startDateTime, DateTime endDateTime)
        {
            List<OPMFLog> logs = [];
            DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                logs = dbAdapter.OPMFLogDbCollection.GetWarnings(startDateTime, endDateTime).ToList();
            });
            return logs;
        }

        public List<OPMFLog> GetErrors(DateTime startDateTime, DateTime endDateTime)
        {
            List<OPMFLog> logs = [];
            DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                logs = dbAdapter.OPMFLogDbCollection.GetErrors(startDateTime, endDateTime).ToList();
            });
            return logs;
        }
    }
}
