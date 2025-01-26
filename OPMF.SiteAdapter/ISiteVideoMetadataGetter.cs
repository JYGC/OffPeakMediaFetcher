namespace OPMF.SiteAdapter
{
    public interface ISiteVideoMetadataGetter
    {
        string GetSiteIdFromURL(string videoURL);

        (Entities.Metadata, Entities.Channel) GetVideoByURL(string siteId);
    }
}
