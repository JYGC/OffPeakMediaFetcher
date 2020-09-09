namespace OffPeakMediaFetcher
{
    class Program
    {
        static void Main(string[] args)
        {
            OPMF.Settings.ConfigHelper.EstablishConfig();
            OPMF.Actions.FolderSetup.EstablishFolders();
            
            if (args.Length == 1 && args[0] == "videos")
            {
                OPMF.Actions.SiteDownload.FetchVideos();
            }
            else if (args.Length == 1 && args[0] == "metadata")
            {
                OPMF.Actions.SiteDownload.FetchMetadata();
            }
            else if (args.Length == 1 && args[0] == "channel")
            {
                OPMF.Actions.SiteDownload.ImportChannels();
            }
            else
            {
                System.Console.WriteLine("Argument required.");
            }
        }
    }
}
