using System.IO;

namespace OPMF.Startup
{
    public static class DirectorySetup
    {
        public static void CheckAppDirectory()
        {
            Config.AppConfig config = Config.ConfigHelper.GetConfig();
            if (!Directory.Exists(config.AppDataDirectory))
            {
                Directory.CreateDirectory(config.AppDataDirectory);
            }
        }
    }
}
