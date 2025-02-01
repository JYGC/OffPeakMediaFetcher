using OPMF.Database;
using OPMF.Entities;

namespace MediaManager.Services
{
    public interface IMetadataServices
    {
        void UpdateStatus(IEnumerable<IMetadata> metadata);
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
    }
}
