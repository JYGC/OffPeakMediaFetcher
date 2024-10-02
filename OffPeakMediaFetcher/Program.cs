namespace OffPeakMediaFetcher
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                OPMF.Settings.ConfigHelper.InitReadonlySettings();
                OPMF.Filesystem.FolderSetup.EstablishFolders();
                OPMF.Settings.ConfigHelper.EstablishConfig();

                if (args.Length >= 1)
                {
                    switch (args[0])
                    {
                        case "videos":
                            VideoFetcher videoFetcher = new VideoFetcher();
                            if (args.Length == 2)
                            {
                                videoFetcher.Run(args[1]);
                                break;
                            }
                            videoFetcher.Run();
                            break;
                        case "metadata":
                            MetadataFetcher metadataFetcher = new MetadataFetcher();
                            metadataFetcher.Run();
                            break;
                        default:
                            throw new System.Exception("Invalid argument.");
                    }
                }
                else
                {
                    throw new System.Exception("Appropriate arguments required.");
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
