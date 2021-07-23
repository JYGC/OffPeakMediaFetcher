﻿using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OPMF.Database
{
    public interface IDatabaseCollection { }

    public class DatabaseCollection<TItem> : IDatabaseCollection where TItem : Entities.IId
    {
        protected string _KeyName { get; } = "_id";

        private LiteDatabase __db;

        private ILiteCollection<TItem> __collection;

        protected LiteDatabase _Db
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

        public void InsertBulk(IEnumerable<TItem> items)
        {
            _Collection.InsertBulk(items);
        }

        public IEnumerable<TItem> GetAll()
        {
            return _Collection.FindAll();
        }

        protected IEnumerable<TItem> _UpdateFields(IEnumerable<TItem> items, Action<TItem, TItem> UpdateFields)
        {
            IEnumerable<string> itemIds = items.Select(i => i.Id);
            List<TItem> dbToUpdate = _Collection.Find(i => itemIds.Contains(i.Id)).ToList(); // dbToUpdate must be list or it won't update in foreach loop
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