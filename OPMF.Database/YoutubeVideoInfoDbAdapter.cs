namespace OPMF.Database
{
    public class YoutubeVideoInfoDbAdapter : VideoInfoDbAdapter<Entities.IVideoInfo>, IVideoInfoDbAdapter<Entities.IVideoInfo>
    {
        private static readonly string __dbFileName = "Youtube.VideoInfos.db";
        private static readonly string __collectionName = "YoutubeVideoInfo";

        public YoutubeVideoInfoDbAdapter() : base(__dbFileName, __collectionName) { }
    }
}
