using LiteDB;

namespace OPMF.Database
{
    public class YoutubeChannelDbCollection : ChannelDbCollection<Entities.IChannel>, IChannelDbCollection<Entities.IChannel>
    {
        private static readonly string __collectionName = "YoutubeChannel";

        public YoutubeChannelDbCollection(LiteDatabase db) : base(db, __collectionName) { }
    }
}
