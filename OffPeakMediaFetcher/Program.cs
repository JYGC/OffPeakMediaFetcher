using System;

using OPMF.Config;

namespace OffPeakMediaFetcher
{
    class Program
    {
        static void Main(string[] args)
        {
            AppConfig config = ConfigHelper.GetConfig();
            Console.WriteLine(config.AppDataName);
        }
    }
}
