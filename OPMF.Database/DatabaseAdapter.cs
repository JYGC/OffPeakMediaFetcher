using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using LiteDB;

namespace OPMF.Database
{
    public interface IDatabaseAdapter { }

    public class DatabaseAdapter<TItem> : IDatabaseAdapter where TItem : Entities.IId
    {
        protected readonly string _keyName = "_id";

        protected LiteDatabase _db;
        protected ILiteCollection<TItem> _collection;

        public DatabaseAdapter(string dbname)
        {
            _db = new LiteDatabase(Path.Join(Config.ConfigHelper.GetConfig().AppDataDirectory, dbname));
            _collection = _db.GetCollection<TItem>();
        }
    }
}
