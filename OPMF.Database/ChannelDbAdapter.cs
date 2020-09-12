using System;
using System.Collections.Generic;
using System.Linq;

using LiteDB;

namespace OPMF.Database
{
    public interface IChannelDbAdapter<TItem> : IDatabaseAdapter where TItem : Entities.IChannel
    {
        IEnumerable<TItem> GetNotBacklisted();
        TItem GetBySiteId(string id);
        TItem GetById(string id);
        void InsertOrUpdate(IEnumerable<TItem> items);
        void UpdateLastCheckedOutAndActivity(IEnumerable<TItem> items);
    }

    public class ChannelDbAdapter<TItem> : DatabaseAdapter<TItem>, IChannelDbAdapter<TItem> where TItem : Entities.IChannel
    {
        public ChannelDbAdapter(string dbName, string collectionName) : base(dbName, collectionName) { }

        public IEnumerable<TItem> GetNotBacklisted()
        {
            return _Collection.Find(i => i.BlackListed == false).ToList();
        }

        public TItem GetBySiteId(string id)
        {
            return GetById(id);
        }

        public TItem GetById(string id)
        {
            return _Collection.FindById(id);
        }

        public void InsertOrUpdate(IEnumerable<TItem> items)
        {
            try
            {
                IEnumerable<TItem> dbToUpdate = _UpdateFields(items, (item, dbItem) =>
                {
                    dbItem.Name = item.Name;
                    if (item.Description != null)
                    {
                        dbItem.Description = item.Description;
                    }
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

        public void UpdateLastCheckedOutAndActivity(IEnumerable<TItem> items)
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
