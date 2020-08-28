namespace OPMF.Database
{
    public class YoutubeChannelDbAdapter : ChannelDbAdapter<Entities.IChannel>, IChannelDbAdapter<Entities.IChannel>
    {
        private static readonly string __dbFileName = "Youtube.Channels.db";
        private static readonly string __collectionName = "YoutubeChannel";

        public YoutubeChannelDbAdapter() : base(__dbFileName, __collectionName) { }
    }
}
