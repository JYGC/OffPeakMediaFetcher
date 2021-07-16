using System;
using System.Collections.Generic;
using System.Linq;

using LiteDB;
using OPMF.Entities;

namespace OPMF.Database
{
    public interface IMetadataDbAdapter<TItem> : IDatabaseAdapter where TItem : Entities.IMetadata
    {
        IEnumerable<TItem> GetToDownload();
        IEnumerable<TItem> GetNew();
        IEnumerable<TItem> GetDownloaded();
        IEnumerable<TItem> GetToDownloadAndWait();
        IEnumerable<TItem> GetIgnored();
        void InsertNew(IEnumerable<TItem> items);
        void UpdateStatus(IEnumerable<TItem> items);
    }

    public class MetadataDbAdapter<TItem> : DatabaseAdapter<TItem>, IMetadataDbAdapter<TItem> where TItem : IMetadata
    {
        public MetadataDbAdapter(string dbName, string collectionName) : base(dbName, collectionName) { }

        public IEnumerable<TItem> GetToDownload()
        {
            return _Collection.Find(i => i.Status == MetadataStatus.ToDownload);
        }

        public IEnumerable<TItem> GetToDownloadAndWait()
        {
            return _Collection.Find(i => i.Status == MetadataStatus.ToDownload || i.Status == MetadataStatus.Wait);
        }

        public IEnumerable<TItem> GetNew()
        {
            return _Collection.Find(i => i.Status == MetadataStatus.New);
        }

        public IEnumerable<TItem> GetDownloaded()
        {
            return _Collection.Find(i => i.Status == MetadataStatus.Downloaded);
        }

        public IEnumerable<TItem> GetIgnored()
        {
            return _Collection.Find(i => i.Status == MetadataStatus.Ignore);
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
