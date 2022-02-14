using LiteDB;
using OPMF.Entities;

namespace OPMF.Database
{
    public class OPMFLogDbCollection : DatabaseCollection<IOPMFLog>
    {
        private static readonly string __collectionName = "OPMFLog";
        public OPMFLogDbCollection(LiteDatabase db) : base(db, __collectionName) { }
    }
}
