using System;
using System.Collections.Generic;
using System.Linq;

using LiteDB;

namespace OPMF.Database
{
    public interface IMetadataDbAdapter<TItem> : IDatabaseAdapter where TItem : Entities.IMetadata
    {
        List<TItem> GetLookedAtNotIgnoreNotDownloaded();
        void InsertOrIgnore(List<TItem> items);
        void UpdateDownloaded(List<TItem> items);
    }

    public class MetadataDbAdapter<TItem> : DatabaseAdapter<TItem>, IMetadataDbAdapter<TItem> where TItem : Entities.IMetadata
    {
        public MetadataDbAdapter(string dbName, string collectionName) : base(dbName, collectionName) { }

        public List<TItem> GetLookedAtNotIgnoreNotDownloaded()
        {
            return _Collection.Find(i => i.LookedAt == true && i.Ignore == false && i.Downloaded == false).ToList();
        }

        public void InsertOrIgnore(List<TItem> items)
        {
            try
            {
                IEnumerable<TItem> toInsert = items.Where(i => _Collection.FindOne(Query.EQ(_KeyName, i.Id)) == null);
                _Collection.InsertBulk(toInsert);
                _Db.Commit();
            }
            catch(Exception e)
            {
                _Db.Rollback();
                throw e;
            }
        }

        public void UpdateDownloaded(List<TItem> items)
        {
            try
            {
                _UpdateFields(items, (item, dbItem) =>
                {
                    dbItem.Downloaded = item.Downloaded;
                });

                _Db.Commit();
            }
            catch(Exception e)
            {
                _Db.Rollback();
                throw e;
            }
        }
    }
}
