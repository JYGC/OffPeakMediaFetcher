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
            return _Collection.Find(i => i.BlackListed == false).ToList();
        }

        public void InsertOrUpdate(List<TItem> items)
        {
            try
            {
                IEnumerable<string> itemIds = items.Select(i => i.Id);
                List<TItem> dbToUpdate = _Collection.Find(i => itemIds.Contains(i.Id)).ToList();
                foreach(TItem dbItem in dbToUpdate)
                {
                    TItem item = items.First(i => i.Id == dbItem.Id);
                    dbItem.Name = item.Name;
                    dbItem.Description = item.Description;
                }
                _Collection.Update(dbToUpdate);

                IEnumerable<TItem> toInsert = items.Where(i => !dbToUpdate.Any(j => j.Id == i.Id));
                _Collection.InsertBulk(toInsert);

                _Db.Commit();
            }
            catch (Exception e)
            {
                _Db.Rollback();
            }
        }

        public void UpdateLastCheckedOutAndActivity(List<TItem> items)
        {
            try
            {
                IEnumerable<string> itemIds = items.Select(i => i.Id);
                List<TItem> dbToUpdate = _Collection.Find(i => itemIds.Contains(i.Id)).ToList();
                foreach (TItem dbItem in dbToUpdate)
                {
                    TItem item = items.First(i => i.Id == dbItem.Id);
                    dbItem.LastCheckedOut = item.LastCheckedOut;
                    dbItem.LastActivityDate = item.LastActivityDate;
                }
                _Collection.Update(dbToUpdate);

                _Db.Commit();
            }
            catch (Exception)
            {
                _Db.Rollback();
            }
        }
    }
}
