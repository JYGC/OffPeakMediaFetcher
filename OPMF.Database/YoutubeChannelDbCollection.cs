using LiteDB;

namespace OPMF.Database
{
    public class YoutubeChannelDbCollection : ChannelDbCollection<Entities.Channel>, IChannelDbCollection<Entities.Channel>
    {
        private static readonly string __collectionName = "YoutubeChannel";

        public YoutubeChannelDbCollection(LiteDatabase db) : base(db, __collectionName) { }
    }
}
