using System;
using System.Collections.Generic;
using System.Linq;

using LiteDB;

namespace OPMF.Database
{
    public interface IChannelDbCollection<TItem> : IDatabaseCollection where TItem : Entities.IChannel
    {
        IEnumerable<TItem> GetAll();
        IEnumerable<TItem> GetNotBacklisted();
        TItem GetBySiteId(string id);
        IEnumerable<TItem> GetManyByWordInName(string wordInChannelName);
        void InsertOrUpdate(IEnumerable<TItem> items);
        void UpdateLastCheckedOutAndActivity(IEnumerable<TItem> items);
        void UpdateBlackListStatus(IEnumerable<TItem> items);
    }

    public class ChannelDbCollection<TItem> : DatabaseCollection<TItem>, IChannelDbCollection<TItem> where TItem : Entities.IChannel
    {
        public ChannelDbCollection(LiteDatabase db, string collectionName) : base(db, collectionName) { }

        public IEnumerable<TItem> GetNotBacklisted()
        {
            return _Collection.Find(i => i.BlackListed == false).ToList();
        }

        public TItem GetBySiteId(string id)
        {
            return _Collection.FindOne(Query.Contains("SiteId", id));
        }

        public IEnumerable<TItem> GetManyByWordInName(string wordInChannelName)
        {
            return _Collection.Find(Query.Contains("Name", wordInChannelName));
        }

        public void InsertOrUpdate(IEnumerable<TItem> items)
        {
            try
            {
                IEnumerable<TItem> dbToUpdate = _UpdateFields(items, (item, dbItem) =>
                {
                    dbItem.Name = item.Name;
                    dbItem.Url = item.Url;
                    dbItem.Thumbnail.Url = item.Thumbnail.Url;
                    dbItem.Thumbnail.Width = item.Thumbnail.Width;
                    dbItem.Thumbnail.Height = item.Thumbnail.Height;
                    if (item.Description != null)
                    {
                        dbItem.Description = item.Description;
                    }
                });

                IEnumerable<TItem> toInsert = items.Where(i => !dbToUpdate.Any(j => j.Id == i.Id));
                _Collection.InsertBulk(toInsert);

                _DB.Commit();
            }
            catch (Exception e)
            {
                _DB.Rollback();
                throw e;
            }
        }

        public void UpdateLastCheckedOutAndActivity(IEnumerable<TItem> items)
        {
            try
            {
                _UpdateFields(items, (item, dbItem) =>
                {
                    dbItem.LastCheckedOut = item.LastCheckedOut;
                    dbItem.LastActivityDate = item.LastActivityDate;
                });

                _DB.Commit();
            }
            catch (Exception e)
            {
                _DB.Rollback();
                throw e;
            }
        }

        public void UpdateBlackListStatus(IEnumerable<TItem> items)
        {
            try
            {
                _UpdateFields(items, (item, dbItem) => dbItem.BlackListed = item.BlackListed);
            }
            catch (Exception e)
            {
                _DB.Rollback();
                throw e;
            }
        }
    }
}
