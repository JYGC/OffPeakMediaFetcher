using System.Collections.Generic;

namespace OPMF.Database
{
    public interface IDatabase<TValue>
    {
        void Save(Dictionary<string, TValue> items);
        string Get(string key);
        Dictionary<string, TValue> GetAll();
        void Remove(string key);
    }
}
