namespace OPMF.SiteAdapter.Youtube
{
    class YoutubeVideoInfoDbAdapter : Database.VideoInfoDbAdapter<YoutubeVideoInfo>
    {
        public YoutubeVideoInfoDbAdapter(string dbname) : base(dbname) { }
    }
}
