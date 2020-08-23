using System.Collections.Generic;
using System.Linq;

using LiteDB;

namespace OPMF.Database
{
    public class ChannelDbAdapter<TItem> : DatabaseAdapter<TItem> where TItem : Entities.IChannel, Entities.IId
    {
        public ChannelDbAdapter(string dbname) : base(dbname) { }

        public List<TItem> GetNotBacklisted()
        {
            return _collection.Find(i => i.BlackListed == false).ToList();
        }

        public void InsertOrUpdate(List<TItem> items)
        {
            try
            {
                IEnumerable<TItem> toInsert = items.Where(i => _collection.FindOne(Query.EQ(_keyName, i.Id)) == null);
                IEnumerable<TItem> toUpdate = items.Where(i => !toInsert.Any(j => j.Id == i.Id));
                _collection.InsertBulk(toInsert);
                _collection.Update(toUpdate);
                _db.Commit();
            }
            catch
            {
                _db.Rollback();
            }
        }
    }
}
