using System;
using System.Collections.Generic;
using System.Linq;

using LiteDB;
using Newtonsoft.Json;

namespace OPMF.Database.Unqlite
{
    public class Unqlite<TValue> : IDatabase<TValue>
    {
        private LiteDatabase __database;

        public Unqlite(string dbpath)
        {
            __database = new LiteDatabase(dbpath);
        }
        public void Insert(Dictionary<string, TValue> items)
        {
            ILiteCollection<TValue> collection = __database.GetCollection<TValue>();
            collection.InsertBulk(items.Select(x => x.Value));
        }

        public string Get(string key)
        {
            ILiteCollection<TValue> collection = __database.GetCollection<TValue>();
            collection.Find(x => (string)typeof(TValue).GetProperty("SiteID").GetValue(x) == key);
            return null;
        }
        
        public List<TValue> GetAll()
        {
            return __database.GetCollection<TValue>().FindAll().ToList();
        }
        
        public void Remove(string key)
        {
            //__database.Remove(key);
        }
    }
}
