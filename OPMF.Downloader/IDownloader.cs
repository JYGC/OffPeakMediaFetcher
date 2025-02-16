using System.Collections.Generic;

namespace OPMF.Downloader
{
    public interface IDownloader
    {
        List<Entities.Metadata> DownloadQueue { get; set; }
        void StartDownloadingQueue();
    }
}
