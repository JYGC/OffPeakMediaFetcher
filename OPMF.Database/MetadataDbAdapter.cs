using System;
using System.Collections.Generic;
using System.Linq;

using LiteDB;

namespace OPMF.Database
{
    public interface IMetadataDbAdapter<TItem> : IDatabaseAdapter where TItem : Entities.IMetadata
    {
        IEnumerable<TItem> GetReallyForDownload();
        IEnumerable<TItem> GetNew();
        void InsertNew(IEnumerable<TItem> items);
        void UpdateStatus(IEnumerable<TItem> items);
    }

    public class MetadataDbAdapter<TItem> : DatabaseAdapter<TItem>, IMetadataDbAdapter<TItem> where TItem : Entities.IMetadata
    {
        public MetadataDbAdapter(string dbName, string collectionName) : base(dbName, collectionName) { }

        public IEnumerable<TItem> GetReallyForDownload()
        {
            return _Collection.Find(i => i.Status == Entities.MetadataStatus.ToDownload).ToList();
        }

        public IEnumerable<TItem> GetNew()
        {
            return _Collection.Find(i => i.Status == Entities.MetadataStatus.New).ToList();
        }

        public void InsertNew(IEnumerable<TItem> items)
        {
            try
            {
                IEnumerable<TItem> toInsert = items.Where(i => _Collection.FindOne(Query.EQ(_KeyName, i.Id)) == null);
                _Collection.InsertBulk(toInsert);
                _Db.Commit();
            }
            catch (Exception e)
            {
                _Db.Rollback();
                throw e;
            }
        }

        public void UpdateStatus(IEnumerable<TItem> items)
        {
            try
            {
                _UpdateFields(items, (item, dbItem) => dbItem.Status = item.Status);

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
