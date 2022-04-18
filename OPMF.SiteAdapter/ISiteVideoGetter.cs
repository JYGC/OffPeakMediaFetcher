namespace OPMF.SiteAdapter
{
    public interface ISiteVideoGetter
    {
        string GetSiteIdFromURL(string videoURL);

        (Entities.IMetadata, Entities.IChannel) GetVideoByURL(string siteId);
    }
}
