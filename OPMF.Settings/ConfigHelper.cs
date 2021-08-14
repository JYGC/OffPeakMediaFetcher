using System.IO;

using Newtonsoft.Json;

namespace OPMF.Settings
{
    /// <summary>
    /// Exposes methods that assessing config settings.
    /// </summary>
    public static class ConfigHelper
    {
        private static AppConfig __config;

        public static IReadonlySettings ReadonlySettings { get; set; }

        public static AppConfig Config
        {
            get
            {
                return __config;
            }
        }

        /// <summary>
        /// Gets configurations variables from config file. Copies config file from example config file to the correct location if it does not exists.
        /// </summary>
        /// <returns>Config file variables in AppConfig class</returns>
        public static void EstablishConfig()
        {
            // get config settings from config.json if it exists and if not, create it and use default values
            if (File.Exists(ReadonlySettings.GetConfigFilePath()))
            {
                __config = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(ReadonlySettings.GetConfigFilePath()).Trim());
            }
            else
            {
                __config = new AppConfig();
                File.WriteAllText(ReadonlySettings.GetConfigFilePath(), JsonConvert.SerializeObject(__config, Formatting.Indented));
            }
        }
    }
}
