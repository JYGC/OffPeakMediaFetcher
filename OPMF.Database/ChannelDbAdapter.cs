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
                List<TItem> dbToUpdate = _UpdateFields(items, (item, dbItem) =>
                {
                    dbItem.Name = item.Name;
                    dbItem.Description = item.Description;
                });

                IEnumerable<TItem> toInsert = items.Where(i => !dbToUpdate.Any(j => j.Id == i.Id));
                _Collection.InsertBulk(toInsert);

                _Db.Commit();
            }
            catch (Exception e)
            {
                _Db.Rollback();
                throw e;
            }
        }

        public void UpdateLastCheckedOutAndActivity(List<TItem> items)
        {
            try
            {
                _UpdateFields(items, (item, dbItem) =>
                {
                    dbItem.LastCheckedOut = item.LastCheckedOut;
                    dbItem.LastActivityDate = item.LastActivityDate;
                });

                _Db.Commit();
            }
            catch (Exception e)
            {
                _Db.Rollback();
                throw e;
            }
        }
    }
}
