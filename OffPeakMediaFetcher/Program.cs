using OPMF.SiteAdapter;
using OPMF.SiteAdapter.Youtube;

namespace OffPeakMediaFetcher
{
    class Program
    {
        static void Main(string[] args)
        {
            OPMF.Startup.DirectorySetup.CheckAppDirectory();

            ISiteAdapter youtubeAdapter = new YoutubeAdapter();
            youtubeAdapter.ImportChannels();
            youtubeAdapter.FetchVideoInfos();
        }
    }
}
