using System.Collections.Generic;
using System.Linq;

using LiteDB;

namespace OPMF.Database
{
    public interface IVideoInfoDbAdapter<TItem> : IDatabaseAdapter where TItem : Entities.IVideoInfo
    {
        List<TItem> GetNotIgnore();
        void InsertOrIgnore(List<TItem> items);
    }

    public class VideoInfoDbAdapter<TItem> : DatabaseAdapter<TItem>, IVideoInfoDbAdapter<TItem> where TItem : Entities.IVideoInfo
    {
        public VideoInfoDbAdapter(string dbName, string collectionName) : base(dbName, collectionName) { }

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
