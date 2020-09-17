using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OPMF.Actions
{
    public static class MetadataManagement
    {
        public static IEnumerable<Entities.IMetadataChannel> GetMetadataChannels()
        {
            IEnumerable<Entities.IMetadataChannel> metadataChannels = new Entities.IMetadataChannel[] { };

            using (Database.IMetadataDbAdapter<Entities.IMetadata> metadataDbAdapter = new Database.YoutubeMetadataDbAdapter())
            using (Database.IChannelDbAdapter<Entities.IChannel> channelDbAdapter = new Database.YoutubeChannelDbAdapter())
            {
                IEnumerable<Entities.IMetadata> metadatas = metadataDbAdapter.GetNew();
                foreach (Entities.IMetadata metadata in metadatas)
                {
                    metadataChannels = metadataChannels.Concat(new Entities.IMetadataChannel[]
                    {
                        new Entities.MetadataChannel
                        {
                            Metadata = new Entities.PropertyChangedMetadata(metadata)
                            , Channel = channelDbAdapter.GetBySiteId(metadata.ChannelSiteId)
                        }
                    });
                }
            }

            return metadataChannels;
        }

        public static (IEnumerable<Entities.IMetadataChannel>, IEnumerable<Entities.IMetadataChannel>) SplitFromNew(IEnumerable<Entities.IMetadataChannel> metadataChannels)
        {
            IEnumerable<Entities.IMetadataChannel> notNewMetadataChannels = metadataChannels.Where(i => i.Metadata.Status != Entities.MetadataStatus.New);
            IEnumerable<Entities.IMetadataChannel> newMetadataChannels = metadataChannels.Where(i => !notNewMetadataChannels.Any(j => j.Metadata.Id == i.Metadata.Id));

            return (newMetadataChannels, notNewMetadataChannels);
        }

        public static void SaveMetadataChanges(IEnumerable<Entities.IMetadataChannel> metadataChannels)
        {
            IEnumerable<Entities.IMetadata> metadatas = metadataChannels.Select(i => i.Metadata);

            using (Database.IMetadataDbAdapter<Entities.IMetadata> metadataDbAdapter = new Database.YoutubeMetadataDbAdapter())
            {
                metadataDbAdapter.UpdateStatus(metadatas);
            }
        }
    }
}
