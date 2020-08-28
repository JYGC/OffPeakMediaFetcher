using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;

namespace OPMF.Database
{
    public interface IDatabaseAdapter : IDisposable
    {
        void MigrateData();
    }

    public class DatabaseAdapter<TItem> : IDatabaseAdapter where TItem : Entities.IId
    {
        protected string _KeyName { get; } = "_id";

        private LiteDatabase __db;

        private ILiteCollection<TItem> __collection;

        private string __dbPath;

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

        public DatabaseAdapter(string dbName, string collectionName)
        {
            __dbPath = Path.Join(Settings.ReadonlySettings.AppFolderPath, dbName);
            __db = new LiteDatabase(__dbPath);
            __collection = __db.GetCollection<TItem>(collectionName);
        }

        public void MigrateData()
        {
            LiteDatabase newDb = new LiteDatabase(__dbPath + ".new");
            ILiteCollection<TItem> newCollection = newDb.GetCollection<TItem>();
            newCollection.InsertBulk(__collection.FindAll());
        }

        public void Dispose()
        {
            __db.Dispose();
        }
    }
}