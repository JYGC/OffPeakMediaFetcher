using OPMF.Database;
using OPMF.Entities;

namespace MediaManager.Services
{
    public interface IMetadataServices
    {
        void UpdateStatus(IEnumerable<IMetadata> metadata);
        void UpdateIsBeingProcessed(IEnumerable<IMetadata> metadatas);
        void DownloadNow(IMetadata metadata);
    }
    public class MetadataServices(ITaskRunnerServices taskRunnerServices) : IMetadataServices
    {
        private readonly ITaskRunnerServices _taskRunnerServices = taskRunnerServices;

        public void UpdateStatus(IEnumerable<IMetadata> metadatas)
        {
            DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                dbAdapter.YoutubeMetadataDbCollection.UpdateStatus(metadatas);
            });
        }

        public void DownloadNow(IMetadata metadata)
        {
            DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                dbAdapter.YoutubeMetadataDbCollection.UpdateIsBeingProcessed([metadata]);
            });

            taskRunnerServices.DownloadOneVideo(metadata);
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
