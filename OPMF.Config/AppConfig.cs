using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace OPMF.Config
{
    public class AppConfig
    {
        private string __appDataName;

        public AppConfig(string configJson)
        {
            Dictionary<string, string> configDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(configJson);
            __appDataName = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), configDict["AppDataName"]);
        }

        public string AppDataName
        {
            get
            {
                return __appDataName;
            }
        }
    }
}
