namespace OffPeakMediaFetcher
{
    class Program
    {
        static void Main(string[] args)
        {
            OPMF.OSCompat.EnvironmentHelper.EstablishEnvironment();
            OPMF.Settings.ConfigHelper.ReadonlySettings = new OPMF.Settings.ReadonlySettings();
            OPMF.Actions.FolderSetup.EstablishFolders();
            OPMF.Settings.ConfigHelper.EstablishConfig();

            if (args.Length == 1)
            {
                switch (args[0])
                {
                    case "videos":
                        OPMF.Actions.SiteDownload.FetchVideos();
                        break;
                    case "metadata":
                        OPMF.Actions.SiteDownload.FetchMetadata();
                        break;
                    default:
                        System.Console.WriteLine("Invalid argument.");
                        break;
                }
            }
            else
            {
                System.Console.WriteLine("Single argument required.");
            }
        }
    }
}
