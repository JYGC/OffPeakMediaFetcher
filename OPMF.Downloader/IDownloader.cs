using System.Collections.Generic;

namespace OPMF.Downloader
{
    public interface IDownloader<TItem> where TItem : Entities.IStringId
    {
        void Download(List<TItem> items);
    }
}
