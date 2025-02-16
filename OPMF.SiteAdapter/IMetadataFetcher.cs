using System.Collections.Generic;



namespace OPMF.SiteAdapter
{
    public interface IMetadataFetcher<TChannel, TVideoInfo> where TChannel : Entities.Channel where TVideoInfo : Entities.Metadata
    {
        List<TVideoInfo> FetchMetadata(ref List<TChannel> channels);
    }
}
