using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LiteDB;

namespace OPMF.Database
{
    public interface IDbCalls
    {
        LiteDatabase DB { set; }
    }

    public class DbCalls<TValue> : IDbCalls where TValue : Entities.IId
    {
        protected readonly string _keyName = "_id";

        protected LiteDatabase _db;
        protected ILiteCollection<TValue> _collection;

        public LiteDatabase DB
        {
            set
            {
                _db = value;
                _collection = _db.GetCollection<TValue>();
            }
        }

        public void InsertOrUpdate(List<TValue> items)
        {
            try
            {
                IEnumerable<TValue> toInsert = items.Where(i => _collection.FindOne(Query.EQ(_keyName, i.Id)) == null);
                IEnumerable<TValue> toUpdate = items.Where(i => !toInsert.Any(j => j.Id == i.Id));
                _collection.InsertBulk(toInsert);
                _collection.Update(toUpdate);
                _db.Commit();
            }
            catch
            {
                _db.Rollback();
            }
        }
    }
}
