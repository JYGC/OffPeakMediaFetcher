namespace OffPeakMediaFetcher
{
    class Program
    {
        static void Main(string[] args)
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
                    case "testerrorlog":
                        System.Collections.Generic.List<System.Exception> stuff = new System.Collections.Generic.List<System.Exception>
                        {
                            new System.Exception("Exp 1"),
                            new System.Exception("Exp 2"),
                            new System.Exception("Exp 3")
                        };
                        OPMF.Logging.Logger.GetCurrent().LogInfoOrWarning(new OPMF.Entities.OPMFLog
                        {
                            Message = "Warning message",
                            Type = OPMF.Entities.OPMFLogType.Info,
                            Variables = new System.Collections.Generic.Dictionary<string, object>
                            {
                                { "metadata.Title", "The Time" },
                                { "metadata.Url", stuff }
                            } // continue with: LiteDB.LiteException: Invalid BSON data type 'Null' on field '_id'.
                        });
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
