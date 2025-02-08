using OPMF.Database;
using OPMF.Entities;
using System.Diagnostics;

namespace MediaManager.Services
{
    public interface IMetadataServices
    {
        void UpdateStatus(IEnumerable<IMetadata> metadata);
        void UpdateIsBeingProcessed(IEnumerable<IMetadata> metadatas);
        void DownloadNow(IMetadata metadata);
    }
    public class MetadataServices : IMetadataServices
    {
        public void UpdateStatus(IEnumerable<IMetadata> metadatas)
        {
            DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                dbAdapter.YoutubeMetadataDbCollection.UpdateStatus(metadatas);
            });
        }

        public void DownloadNow(IMetadata metadata)
        {
            const string __offPeakMediaFetcherEXE = @"OffPeakMediaFetcher.exe";
            const string __offPeakMediaFetcherArgsScaffold = "videos {0}";

            DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                dbAdapter.YoutubeMetadataDbCollection.UpdateIsBeingProcessed([metadata]);
            });

            Process process = new Process();
            process.StartInfo.FileName = __offPeakMediaFetcherEXE;
            process.StartInfo.Arguments = string.Format(__offPeakMediaFetcherArgsScaffold, metadata.SiteId);
            process.Start();
        }

        public void UpdateIsBeingProcessed(IEnumerable<IMetadata> metadatas)
        {
            DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                dbAdapter.YoutubeMetadataDbCollection.UpdateIsBeingProcessed(metadatas);
            });
        }
    }
}
