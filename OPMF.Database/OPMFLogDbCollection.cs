using LiteDB;
using OPMF.Entities;
using System.Collections.Generic;

namespace OPMF.Database
{
    public class OPMFLogDbCollection : DatabaseCollection<IOPMFLog>
    {
        private static readonly string __collectionName = "OPMFLog";
        public OPMFLogDbCollection(LiteDatabase db) : base(db, __collectionName) { }

        public IEnumerable<IOPMFLog> GetWarnings()
        {
            return _Collection.Find(i => i.Type == OPMFLogType.Warning);
        }

        public IEnumerable<IOPMFLog> GetErrors()
        {
            return _Collection.Find(i => i.Type == OPMFLogType.Error);
        }

        public IEnumerable<IOPMFLog> GetInfos()
        {
            return _Collection.Find(i => i.Type == OPMFLogType.Info);
        }
    }
}
