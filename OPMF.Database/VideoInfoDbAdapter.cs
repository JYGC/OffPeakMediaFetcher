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
            return _Collection.Find(i => i.Ignore == false).ToList();
        }

        public void InsertOrIgnore(List<TItem> items)
        {
            try
            {
                IEnumerable<TItem> toInsert = items.Where(i => _Collection.FindOne(Query.EQ(_KeyName, i.Id)) == null);
                _Collection.InsertBulk(toInsert);
                _Db.Commit();
            }
            catch
            {
                _Db.Rollback();
            }
        }
    }
}
