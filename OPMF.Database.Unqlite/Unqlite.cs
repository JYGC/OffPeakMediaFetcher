using System.Collections.Generic;
using System.Linq;

using UnQLiteNet;
using Newtonsoft.Json;

namespace OPMF.Database.Unqlite
{
    public class Unqlite<TValue> : IDatabase<TValue>
    {
        private UnQLite __database;

        public Unqlite(string dbpath)
        {
            __database = new UnQLite(dbpath, UnQLiteOpenModel.Create | UnQLiteOpenModel.ReadWrite);
        }

        ~Unqlite()
        {
            __database.Close();
        }

        public void Save(Dictionary<string, TValue> items)
        {
            foreach(KeyValuePair<string, TValue> item in items)
            {
                __database.Save(item.Key, JsonConvert.SerializeObject(item.Value));
            }
        }

        public string Get(string key)
        {
            return __database.Get(key);
        }
        
        public Dictionary<string, TValue> GetAll()
        {
            return __database.GetAll().ToDictionary(item => item.Item1, item => JsonConvert.DeserializeObject<TValue>(item.Item2));
        }
        
        public void Remove(string key)
        {
            __database.Remove(key);
        }
    }
}
