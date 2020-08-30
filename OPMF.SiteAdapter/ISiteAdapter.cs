using System.Collections.Generic;



namespace OPMF.SiteAdapter
{
    public interface ISiteAdapter<TChannel, TVideoInfo> where TChannel : Entities.IChannel where TVideoInfo : Entities.IMetadata
    {
        List<TVideoInfo> FetchMetadata(ref List<TChannel> channels);
        List<TChannel> ImportChannels();
    }
}
