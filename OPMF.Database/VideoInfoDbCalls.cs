using System.Collections.Generic;
using System.Linq;

namespace OPMF.Database
{
    public class VideoInfoDbCalls<TItem> : DbCalls<TItem> where TItem : Entities.IVideoInfo, Entities.IId
    {
        public List<TItem> GetNotIgnore()
        {
            return _collection.Find(i => i.Ignore == false).ToList();
        }
    }
}
