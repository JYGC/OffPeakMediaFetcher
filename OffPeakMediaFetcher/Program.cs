namespace OffPeakMediaFetcher
{
    class Program
    {
        static void Main(string[] args)
        {
            OPMF.Settings.ConfigHelper.EstablishConfig();
            OPMF.Actions.DirectorySetup.EstablishAppDirectory();
            OPMF.Actions.SiteDownload.ImportChannels();
            OPMF.Actions.SiteDownload.FetchVideoInfos();

            //OPMF.Actions.DatabaseChanges.Migrate();
        }
    }
}
