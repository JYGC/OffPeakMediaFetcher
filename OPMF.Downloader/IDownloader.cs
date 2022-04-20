using System.Collections.Generic;

namespace OPMF.Downloader
{
    public interface IDownloader<TItem> where TItem : Entities.IStringId
    {
        List<Entities.IMetadata> DownloadQueue { get; set; }
        void StartDownloadingQueue();
    }
}
