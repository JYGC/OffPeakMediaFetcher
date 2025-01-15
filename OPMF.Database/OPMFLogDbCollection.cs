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
            return _Collection.Query().Where(i => i.Type == OPMFLogType.Warning).OrderByDescending(i => i.DateCreated).Skip(__skip).Limit(__pageSize).ToList();
        }

        public IEnumerable<IOPMFLog> GetErrors(int __skip, int __pageSize)
        {
            return _Collection.Query().Where(i => i.Type == OPMFLogType.Error).OrderByDescending(i => i.DateCreated).Skip(__skip).Limit(__pageSize).ToList();
        }

        public IEnumerable<IOPMFLog> GetInfos(int __skip, int __pageSize)
        {
            return _Collection.Query().Where(i => i.Type == OPMFLogType.Info).OrderByDescending(i => i.DateCreated).Skip(__skip).Limit(__pageSize).ToList();
        }
    }
}
