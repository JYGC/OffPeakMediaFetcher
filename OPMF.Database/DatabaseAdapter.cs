using System;
using System.IO;

using LiteDB;

namespace OPMF.Database
{
    public interface IDatabaseAdapter : IDisposable
    {
        void MigrateData();
    }

    public class DatabaseAdapter<TItem> : IDatabaseAdapter where TItem : Entities.IId
    {
        protected readonly string _keyName = "_id";

        protected LiteDatabase _db;
        protected ILiteCollection<TItem> _collection;
        protected string _dbPath;

        public DatabaseAdapter(string dbName, string collectionName)
        {
            _dbPath = Path.Join(Settings.ReadonlySettings.AppFolderPath, dbName);
            _db = new LiteDatabase(_dbPath);
            _collection = _db.GetCollection<TItem>(collectionName);
        }

        public void MigrateData()
        {
            LiteDatabase newDb = new LiteDatabase(_dbPath + ".new");
            ILiteCollection<TItem> newCollection = newDb.GetCollection<TItem>();
            newCollection.InsertBulk(_collection.FindAll());
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}