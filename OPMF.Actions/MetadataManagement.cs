using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OPMF.Actions
{
    public static class MetadataManagement
    {
        private static IEnumerable<Entities.IMetadataChannel> _GetMetadataChannels(
            Func<Database.IMetadataDbCollection<Entities.IMetadata>, IEnumerable<Entities.IMetadata>> metadataDbFunc)
        {
            IEnumerable<Entities.IMetadataChannel> metadataChannels = new Entities.IMetadataChannel[] { };

            Database.DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                IEnumerable<Entities.IMetadata> metadatas = metadataDbFunc(dbAdapter.YoutubeMetadataDbCollection);
                foreach (Entities.IMetadata metadata in metadatas)
                {
                    metadataChannels = metadataChannels.Concat(new Entities.IMetadataChannel[]
                    {
                        new Entities.MetadataChannel
                        {
                            Metadata = new Entities.PropertyChangedMetadata(metadata)
                            , Channel = dbAdapter.YoutubeChannelDbCollection.GetBySiteId(metadata.ChannelSiteId)
                        }
                    });
                }
            });

            return metadataChannels;
        }

        public static IEnumerable<Entities.IMetadataChannel> GetNew()
        {
            return _GetMetadataChannels((metadataDbAdapter) => metadataDbAdapter.GetNew());
        }

        public static IEnumerable<Entities.IMetadataChannel> GetToDownloadAndWait()
        {
            return _GetMetadataChannels((metadataDbAdapter) => metadataDbAdapter.GetToDownloadAndWait());
        }

        public static IEnumerable<Entities.IMetadataChannel> GetDownloaded()
        {
            return _GetMetadataChannels((metadataDbAdapter) => metadataDbAdapter.GetDownloaded());
        }

        public static IEnumerable<Entities.IMetadataChannel> GetIgnored()
        {
            return _GetMetadataChannels((metadataDbAdapter) => metadataDbAdapter.GetIgnored());
        }

        public static (IEnumerable<Entities.IMetadataChannel>, IEnumerable<Entities.IMetadataChannel>) SplitFromStatus(IEnumerable<Entities.IMetadataChannel> metadataChannels,
                                                                                                                       Entities.MetadataStatus metadataStatus,
                                                                                                                       Entities.MetadataStatus metadataStatus2)
        {
            IEnumerable<Entities.IMetadataChannel> toUpdateMetadataChannels = metadataChannels. Where(i => true);
            IEnumerable<Entities.IMetadataChannel> toShowMetadataChannels = metadataChannels.Where(i => i.Metadata.Status == metadataStatus || i.Metadata.Status == metadataStatus2);

            return (toShowMetadataChannels, toUpdateMetadataChannels);
        }

        public static (IEnumerable<Entities.IMetadataChannel>, IEnumerable<Entities.IMetadataChannel>) SplitFromStatus(IEnumerable<Entities.IMetadataChannel> metadataChannels,
                                                                                                                       Entities.MetadataStatus metadataStatus)
        {
            IEnumerable<Entities.IMetadataChannel> toUpdateMetadataChannels = metadataChannels.Where(i => i.Metadata.Status != metadataStatus);
            IEnumerable<Entities.IMetadataChannel> toShowMetadataChannels = metadataChannels.Where(i => !toUpdateMetadataChannels.Any(j => j.Metadata.Id == i.Metadata.Id));

            return (toShowMetadataChannels, toUpdateMetadataChannels);
        }

        public static void SaveMetadataChanges(IEnumerable<Entities.IMetadataChannel> metadataChannels)
        {
            IEnumerable<Entities.IMetadata> metadatas = metadataChannels.Select(i => i.Metadata);

            Database.DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                dbAdapter.YoutubeMetadataDbCollection.UpdateStatus(metadatas);
            });
        }
    }
}
