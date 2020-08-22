using System.Collections.Generic;
using System.Linq;

using LiteDB;

namespace OPMF.Database
{
    public class VideoInfoDbCalls<TItem> : DbCalls<TItem> where TItem : Entities.IVideoInfo, Entities.IId
    {
        public List<TItem> GetNotIgnore()
        {
            return _collection.Find(i => i.Ignore == false).ToList();
        }

        public void InsertOrIgnore(List<TItem> items)
        {
            try
            {
                IEnumerable<TItem> toInsert = items.Where(i => _collection.FindOne(Query.EQ(_keyName, i.Id)) == null);
                _collection.InsertBulk(toInsert);
                _db.Commit();
            }
            catch
            {
                _db.Rollback();
            }
        }
    }
}
