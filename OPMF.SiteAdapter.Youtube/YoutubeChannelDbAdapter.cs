namespace OPMF.SiteAdapter.Youtube
{
    class YoutubeChannelDbAdapter : Database.ChannelDbAdapter<YoutubeChannel>
    {
        public YoutubeChannelDbAdapter(string dbname) : base(dbname) { }
    }
}
