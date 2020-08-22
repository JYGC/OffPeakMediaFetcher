using System.Collections.Generic;
using System.Linq;

namespace OPMF.Database
{
    public class ChannelDbCalls<TItem> : DbCalls<TItem> where TItem : Entities.IChannel, Entities.IId
    {
        public List<TItem> GetNotBacklisted()
        {
            return _collection.Find(i => i.BlackListed == false).ToList();
        }
    }
}
