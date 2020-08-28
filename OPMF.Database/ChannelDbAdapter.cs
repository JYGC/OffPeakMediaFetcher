using System;
using System.Collections.Generic;
using System.Linq;

using LiteDB;

namespace OPMF.Database
{
    public interface IChannelDbAdapter<TItem> : IDatabaseAdapter where TItem : Entities.IChannel
    {
        List<TItem> GetNotBacklisted();
        void InsertOrUpdate(List<TItem> items);
        void UpdateLastCheckedOutAndActivity(List<TItem> items);
    }

    public class ChannelDbAdapter<TItem> : DatabaseAdapter<TItem>, IChannelDbAdapter<TItem> where TItem : Entities.IChannel
    {
        public ChannelDbAdapter(string dbName, string collectionName) : base(dbName, collectionName) { }

        public List<TItem> GetNotBacklisted()
        {
            return _collection.Find(i => i.BlackListed == false).ToList();
        }

        public void InsertOrUpdate(List<TItem> items)
        {
            try
            {
                IEnumerable<string> itemIds = items.Select(i => i.Id);
                IEnumerable<TItem> dbToUpdate = _collection.Find(i => itemIds.Contains(i.Id));
                IEnumerable<TItem> toInsert = items.Where(i => !dbToUpdate.Any(j => j.Id == i.Id));
                _collection.InsertBulk(toInsert);

                foreach(TItem dbItem in dbToUpdate)
                {
                    TItem item = items.First(i => i.Id == dbItem.Id);
                    dbItem.Name = item.Name;
                    dbItem.Description = item.Description;
                }
                _collection.Update(dbToUpdate);
                _db.Commit();
            }
            catch (Exception e)
            {
                _db.Rollback();
            }
        }

        public void UpdateLastCheckedOutAndActivity(List<TItem> items)
        {
            try
            {
                IEnumerable<string> itemIds = items.Select(i => i.Id);
                List<TItem> dbToUpdate = _collection.Find(i => itemIds.Contains(i.Id)).ToList();
                foreach (TItem dbItem in dbToUpdate)
                {
                    TItem item = items.First(i => i.Id == dbItem.Id);
                    dbItem.LastCheckedOut = item.LastCheckedOut;
                    dbItem.LastActivityDate = item.LastActivityDate;
                }
                _collection.Update(dbToUpdate);
                _db.Commit();
            }
            catch (Exception)
            {
                _db.Rollback();
            }
        }
    }
}
