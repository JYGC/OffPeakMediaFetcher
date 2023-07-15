using System;
using System.Collections.Generic;
using System.Linq;

using LiteDB;
using OPMF.Entities;

namespace OPMF.Database
{
    public interface IMetadataDbCollection<TItem> : IDatabaseCollection<TItem> where TItem : IMetadata
    {
        IEnumerable<TItem> GetToDownload();
        IEnumerable<TItem> GetNew(int skip, int pageSize);
        IEnumerable<TItem> GetToDownloadAndWait(int skip, int pageSize);
        IEnumerable<TItem> GetManyByWordInTitle(string wordInMetadataTitle, int skip, int pageSize);
        IEnumerable<TItem> GetManyByChannelSiteIdAndWordInTitle(IEnumerable<string> channelSiteIds, string wordInMetadataTitle, int skip, int pageSize);
        void InsertNew(IEnumerable<TItem> items);
        void UpdateStatus(IEnumerable<TItem> items);
        void UpdateIsBeingProcessed(IEnumerable<TItem> items, bool? isProcessedValue = null);
    }

    public class MetadataDbCollection<TItem> : DatabaseCollection<TItem>, IMetadataDbCollection<TItem> where TItem : IMetadata
    {
        public MetadataDbCollection(LiteDatabase db, string collectionName) : base(db, collectionName) { }

        public IEnumerable<TItem> GetToDownload()
        {
            return _Collection.Find(i => i.Status == MetadataStatus.ToDownload);
        }

        public IEnumerable<TItem> GetToDownloadAndWait(int skip, int pageSize)
        {
            return _Collection.Find(i => i.Status == MetadataStatus.ToDownload || i.Status == MetadataStatus.Wait, skip, pageSize);
        }

        public IEnumerable<TItem> GetNew(int skip, int pageSize)
        {
            return _Collection.Find(i => i.Status == MetadataStatus.New, skip, pageSize);
        }

        public IEnumerable<TItem> GetManyByWordInTitle(string wordInMetadataTitle, int skip, int pageSize)
        {
            return _Collection.Find(Query.Contains("Title", wordInMetadataTitle), skip, pageSize);
        }

        public IEnumerable<TItem> GetManyByChannelSiteIdAndWordInTitle(IEnumerable<string> channelSiteIds, string wordInMetadataTitle, int skip, int pageSize)
        {
            BsonArray siteIdsBsonArray = new BsonArray();
            foreach (string channelSiteId in channelSiteIds)
            {
                siteIdsBsonArray.Add(channelSiteId);
            }
            BsonExpression whereCause = Query.In("ChannelSiteId", siteIdsBsonArray);
            if (!string.IsNullOrWhiteSpace(wordInMetadataTitle))
            {
                whereCause = Query.And(whereCause, Query.Contains("Title", wordInMetadataTitle));
            }
            return _Collection.Find(whereCause, skip, pageSize);
        }

        public void InsertNew(IEnumerable<TItem> items)
        {
            try
            {
                IEnumerable<TItem> toInsert = items.Where(i => _Collection.FindOne(Query.EQ(_KeyName, i.Id)) == null);
                _Collection.InsertBulk(toInsert);
                _DB.Commit();
            }
            catch (Exception)
            {
                _DB.Rollback();
                throw;
            }
        }

        public void UpdateStatus(IEnumerable<TItem> items)
        {
            try
            {
                _UpdateFields(items, (item, dbItem) => dbItem.Status = item.Status);
                _DB.Commit();
            }
            catch (Exception)
            {
                _DB.Rollback();
                throw;
            }
        }

        public void UpdateIsBeingProcessed(IEnumerable<TItem> items, bool? isProcessedValue = null)
        {
            try
            {
                _UpdateFields(items, (item, dbItem) =>
                {
                    if (isProcessedValue.HasValue) // Need to update items as well
                    {
                        item.IsBeingDownloaded = isProcessedValue.Value;
                    }
                    dbItem.IsBeingDownloaded = item.IsBeingDownloaded;
                });
                _DB.Commit();
            }
            catch (Exception)
            {
                _DB.Rollback();
                throw;
            }
        }

        protected new IEnumerable<TItem> _UpdateFields(IEnumerable<TItem> items, Action<TItem, TItem> UpdateFields)
        {
            IEnumerable<string> itemIds = items.Select(i => i.Id);
            List<TItem> dbToUpdate = _Collection.Find(i => itemIds.Contains(i.SiteId)).ToList(); // dbToUpdate must be list or it won't update in foreach loop
            foreach (TItem dbItem in dbToUpdate)
            {
                TItem item = items.First(i => i.Id == dbItem.Id);
                UpdateFields(item, dbItem);
            }
            _Collection.Update(dbToUpdate);

            return dbToUpdate;
        }
    }
}
