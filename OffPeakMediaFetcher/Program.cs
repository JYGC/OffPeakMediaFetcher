namespace OffPeakMediaFetcher
{
    class Program
    {
        static void Main(string[] args)
        {
            OPMF.Settings.ConfigHelper.EstablishConfig();
            OPMF.Actions.FolderSetup.EstablishFolders();
            OPMF.Actions.SiteDownload.ImportChannels();
            OPMF.Actions.SiteDownload.FetchMetadata();

            //OPMF.Actions.SiteDownload.FetchVideos();

            //OPMF.Actions.DatabaseChanges.Migrate();
        }
    }
}
