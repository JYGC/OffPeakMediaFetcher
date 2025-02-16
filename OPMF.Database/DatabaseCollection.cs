using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OPMF.Database
{
    public interface IDatabaseCollection<TItem>
    {
        TItem GetBySiteId(string id);
    }

    public class DatabaseCollection<TItem> : IDatabaseCollection<TItem>
    {
        protected string _KeyName { get; } = "_id";

        private LiteDatabase __db;

        private ILiteCollection<TItem> __collection;

        protected LiteDatabase _DB
        {
            get
            {
                return __db;
            }
        }

        protected ILiteCollection<TItem> _Collection
        {
            get
            {
                return __collection;
            }
        }

        public DatabaseCollection(LiteDatabase db, string collectionName)
        {
            __db = db;
            __collection = __db.GetCollection<TItem>(collectionName);
        }

        public TItem GetBySiteId(string id)
        {
            return _Collection.FindOne(Query.Contains("SiteId", id));
        }

        public void InsertBulk(IEnumerable<TItem> items)
        {
            _Collection.InsertBulk(items);
        }

        public IEnumerable<TItem> GetAll()
        {
            return _Collection.FindAll();
        }
    }
}