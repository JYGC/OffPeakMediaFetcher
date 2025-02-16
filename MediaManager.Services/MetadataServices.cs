using OPMF.Database;
using OPMF.Entities;

namespace MediaManager.Services
{
    public interface IMetadataServices
    {
        void UpdateStatus(IEnumerable<Metadata> metadata);
        void UpdateIsBeingProcessed(IEnumerable<Metadata> metadatas);
        void DownloadNow(Metadata metadata);
    }
    public class MetadataServices(ITaskRunnerServices taskRunnerServices) : IMetadataServices
    {
        private readonly ITaskRunnerServices _taskRunnerServices = taskRunnerServices;

        public void UpdateStatus(IEnumerable<Metadata> metadatas)
        {
            DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                dbAdapter.YoutubeMetadataDbCollection.UpdateStatus(metadatas);
            });
        }

        public void DownloadNow(Metadata metadata)
        {
            DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                dbAdapter.YoutubeMetadataDbCollection.UpdateIsBeingProcessed([metadata]);
            });

            taskRunnerServices.DownloadOneVideo(metadata);
        }

        public void UpdateIsBeingProcessed(IEnumerable<Metadata> metadatas)
        {
            DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                dbAdapter.YoutubeMetadataDbCollection.UpdateIsBeingProcessed(metadatas);
            });
        }
    }
}
