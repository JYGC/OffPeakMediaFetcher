using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace OPMF.Config
{
    public class AppConfig
    {
        private string __appDataDir;

        public AppConfig(string configJson)
        {
            Dictionary<string, string> configDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(configJson);
            __appDataDir = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), configDict["AppDataDirectory"]);
        }

        public string AppDataDirectory
        {
            get
            {
                return __appDataDir;
            }
        }
    }
}
