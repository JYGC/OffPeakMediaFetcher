using System;
using System.IO;
using System.Reflection;

namespace OPMF.Config
{
    public class ConfigHelper
    {
        private static readonly string __configFileName = "config.json";
        private static readonly string __exampleConfigFileName = "config.example.json";

        public static AppConfig GetConfig()
        {
            string execDir = new FileInfo((new Uri(Assembly.GetEntryAssembly().GetName().CodeBase)).AbsolutePath).DirectoryName;
            string configPath = Path.Join(execDir, __configFileName);

            if (!File.Exists(configPath))
            {
                File.Copy(Path.Join(execDir, __exampleConfigFileName), configPath);
            }

            return new AppConfig(File.ReadAllText(configPath).Trim());
        }
    }
}
