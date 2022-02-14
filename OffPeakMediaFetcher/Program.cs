namespace OffPeakMediaFetcher
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                OPMF.OSCompat.EnvironmentHelper.EstablishEnvironment();
                OPMF.Settings.ConfigHelper.InitReadonlySettings();
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
                            throw new System.Exception("Invalid argument.");
                            // break; - code will never reach this
                    }
                }
                else
                {
                    throw new System.Exception("Single argument required.");
                }
            }
            catch (System.Exception e)
            {
                OPMF.Logging.Logger.GetCurrent().LogEntry(new OPMF.Entities.OPMFError(e));
                System.Console.WriteLine(e.Message);
            }
        }
    }
}
