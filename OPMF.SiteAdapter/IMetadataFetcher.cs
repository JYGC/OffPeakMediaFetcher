using System.Collections.Generic;



namespace OPMF.SiteAdapter
{
    public interface IMetadataFetcher<TChannel, TVideoInfo> where TChannel : Entities.IChannel where TVideoInfo : Entities.IMetadata
    {
        List<TVideoInfo> FetchMetadata(ref List<TChannel> channels);
    }
}
