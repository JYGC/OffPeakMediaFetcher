namespace OffPeakMediaFetcher
{
    class Program
    {
        static void Main(string[] args)
        {
            OPMF.OSCompat.EnvironmentHelper.EstablishEnvironment();
            OPMF.Settings.ConfigHelper.EstablishConfig();
            OPMF.Actions.FolderSetup.EstablishFolders();

            if (args.Length == 1)
            {
                if (args[0] == "videos")
                {
                    OPMF.Actions.SiteDownload.FetchVideos();
                }
                else if (args[0] == "metadata")
                {
                    OPMF.Actions.SiteDownload.FetchMetadata();
                }
                else if (args[0] == "updatechannels")
                {
                    //OPMF.Actions.SiteDownload.ImportChannels();
                }
                else if (args[0] == "test")
                {
                    //OPMF.Tests.Tests
                }
                else
                {
                    System.Console.WriteLine("Invalid argument.");
                }
            }
            else
            {
                System.Console.WriteLine("Argument required.");
            }
        }
    }
}
