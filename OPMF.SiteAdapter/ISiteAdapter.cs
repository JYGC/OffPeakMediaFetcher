using System.Collections.Generic;



namespace OPMF.SiteAdapter
{
    public interface ISiteAdapter<TChannel, TVideoInfo> where TChannel : Entities.IChannel where TVideoInfo : Entities.IVideoInfo
    {
        List<TVideoInfo> FetchVideoInfos(ref List<TChannel> channels);
        List<TChannel> ImportChannels();
    }
}
