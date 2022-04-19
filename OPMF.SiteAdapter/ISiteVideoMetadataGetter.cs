namespace OPMF.SiteAdapter
{
    public interface ISiteVideoMetadataGetter
    {
        string GetSiteIdFromURL(string videoURL);

        (Entities.IMetadata, Entities.IChannel) GetVideoByURL(string siteId);
    }
}
