using System;
using System.Collections.Generic;
using System.Linq;

using LiteDB;

namespace OPMF.Database
{
    public interface IChannelDbCollection<TItem> : IDatabaseCollection<TItem> where TItem : Entities.Channel
    {
        List<TItem> GetManyBySiteIds(IEnumerable<string> ids);
        IEnumerable<TItem> GetAll();
        IEnumerable<TItem> GetNotBacklisted();
        IEnumerable<TItem> GetManyByWordInName(string wordInChannelName);
        void InsertOrUpdate(IEnumerable<TItem> items);
        void UpdateLastCheckedOutAndActivity(IEnumerable<TItem> items);
        void UpdateBlackListStatus(IEnumerable<TItem> items);
    }

    public class ChannelDbCollection<TItem> : DatabaseCollection<TItem>, IChannelDbCollection<TItem> where TItem : Entities.Channel
    {
        public ChannelDbCollection(LiteDatabase db, string collectionName) : base(db, collectionName) { }

        public List<TItem> GetManyBySiteIds(IEnumerable<string> ids)
        {
            return _Collection.Query().Where(c => ids.Contains(c.SiteId)).ToList();
        }

        public IEnumerable<TItem> GetNotBacklisted()
        {
            return _Collection.Find(i => i.Blacklisted == false).ToList();
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
            catch (Exception)
            {
                _DB.Rollback();
                throw;
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
            catch (Exception)
            {
                _DB.Rollback();
                throw;
            }
        }

        public void UpdateBlackListStatus(IEnumerable<TItem> items)
        {
            try
            {
                _UpdateFields(items, (item, dbItem) => dbItem.Blacklisted = item.Blacklisted);
            }
            catch (Exception)
            {
                _DB.Rollback();
                throw;
            }
        }

        protected IEnumerable<TItem> _UpdateFields(IEnumerable<TItem> items, Action<TItem, TItem> UpdateFields)
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
