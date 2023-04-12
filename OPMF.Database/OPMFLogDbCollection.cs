using LiteDB;
using OPMF.Entities;
using System.Collections.Generic;

namespace OPMF.Database
{
    public class OPMFLogDbCollection : DatabaseCollection<IOPMFLog>
    {
        private static readonly string __collectionName = "OPMFLog";
        public OPMFLogDbCollection(LiteDatabase db) : base(db, __collectionName) { }

        public IEnumerable<IOPMFLog> GetWarnings(int __skip, int __pageSize)
        {
            return _Collection.Find(i => i.Type == OPMFLogType.Warning, __skip, __pageSize);
        }

        public IEnumerable<IOPMFLog> GetErrors(int __skip, int __pageSize)
        {
            return _Collection.Find(i => i.Type == OPMFLogType.Error, __skip, __pageSize);
        }

        public IEnumerable<IOPMFLog> GetInfos(int __skip, int __pageSize)
        {
            return _Collection.Find(i => i.Type == OPMFLogType.Info, __skip, __pageSize);
        }
    }
}
