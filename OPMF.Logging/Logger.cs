using OPMF.Entities;
using System.Collections.Generic;

namespace OPMF.Logging
{
    public class Logger
    {
        public void LogError(OPMFError oPMFError)
        {
            Database.DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                dbAdapter.OPMFLogDbCollection.InsertBulk(new List<OPMFError> { oPMFError });
            });
        }

        public void LogInfoOrWarning(IOPMFLog oPMFLog)
        {
            Database.DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                dbAdapter.OPMFLogDbCollection.InsertBulk(new List<IOPMFLog> { oPMFLog });
            });
        }

        private static Logger __currentInstance;

        public static Logger GetCurrent()
        {
            if (__currentInstance == null)
            {
                __currentInstance = new Logger();
            }
            return __currentInstance;
        }
    }
}
