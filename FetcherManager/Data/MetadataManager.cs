using System;
using System.Collections.Generic;
using System.Linq;

namespace FetcherManager.Data
{
    public class MetadataManager
    {
        public IEnumerable<OPMF.Entities.IMetadataChannel> GetByChannelAndTitleContainingWord(
            string wordInChannelName,
            string wordInMetadataTitle, int skip, int pageSize)
        {
            IEnumerable<OPMF.Entities.IMetadataChannel> metadataChannels = new OPMF.Entities.IMetadataChannel[] { };
            OPMF.Database.DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                Dictionary<string, OPMF.Entities.IChannel> channelsWithSiteId = dbAdapter.YoutubeChannelDbCollection.GetManyByWordInName(
                    wordInChannelName).ToDictionary(c => c.SiteId, c => c);
                IEnumerable<OPMF.Entities.IMetadata> metadatas = dbAdapter.YoutubeMetadataDbCollection.GetManyByChannelSiteIdAndWordInTitle(
                    channelsWithSiteId.Keys, wordInMetadataTitle, skip, pageSize);
                foreach (OPMF.Entities.IMetadata metadata in metadatas)
                {
                    metadataChannels = metadataChannels.Concat(new OPMF.Entities.IMetadataChannel[]
                    {
                        new OPMF.Entities.MetadataChannel
                        {
                            Metadata = new OPMF.Entities.PropertyChangedMetadata(metadata),
                            Channel = channelsWithSiteId[metadata.ChannelSiteId]
                        }
                    });
                }
            });
            return metadataChannels;
        }

        private IEnumerable<OPMF.Entities.IMetadataChannel> __GetMetadataChannels(
            Func<OPMF.Database.IMetadataDbCollection<OPMF.Entities.IMetadata>, IEnumerable<OPMF.Entities.IMetadata>> metadataDbFunc)
        {
            IEnumerable<OPMF.Entities.IMetadataChannel> metadataChannels = new OPMF.Entities.IMetadataChannel[] { };

            OPMF.Database.DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                IEnumerable<OPMF.Entities.IMetadata> metadatas = metadataDbFunc(dbAdapter.YoutubeMetadataDbCollection);
                foreach (OPMF.Entities.IMetadata metadata in metadatas)
                {
                    metadataChannels = metadataChannels.Concat(new OPMF.Entities.IMetadataChannel[]
                    {
                        new OPMF.Entities.MetadataChannel
                        {
                            Metadata = new OPMF.Entities.PropertyChangedMetadata(metadata),
                            Channel = dbAdapter.YoutubeChannelDbCollection.GetBySiteId(metadata.ChannelSiteId)
                        }
                    });
                }
            });

            return metadataChannels;
        }

        public IEnumerable<OPMF.Entities.IMetadataChannel> GetNew(int skip, int pageSize)
        {
            return __GetMetadataChannels((metadataDbAdapter) => metadataDbAdapter.GetNew(skip, pageSize));
        }

        public IEnumerable<OPMF.Entities.IMetadataChannel> GetToDownloadAndWait(int skip, int pageSize)
        {
            return __GetMetadataChannels((metadataDbAdapter) => metadataDbAdapter.GetToDownloadAndWait(skip, pageSize));
        }

        public IEnumerable<OPMF.Entities.IMetadataChannel> GetByTitleContainingWord(string wordInMetadataTitle, int skip, int pageSize)
        {
            return __GetMetadataChannels((metadataDbAdapter) => metadataDbAdapter.GetManyByWordInTitle(wordInMetadataTitle, skip, pageSize));
        }

        public (IEnumerable<OPMF.Entities.IMetadataChannel>, IEnumerable<OPMF.Entities.IMetadataChannel>) SplitFromStatus(IEnumerable<OPMF.Entities.IMetadataChannel> metadataChannels,
                                                                                                                       OPMF.Entities.MetadataStatus metadataStatus,
                                                                                                                       OPMF.Entities.MetadataStatus metadataStatus2)
        {
            IEnumerable<OPMF.Entities.IMetadataChannel> toUpdateMetadataChannels = metadataChannels. Where(i => true);
            IEnumerable<OPMF.Entities.IMetadataChannel> toShowMetadataChannels = metadataChannels.Where(i => i.Metadata.Status == metadataStatus || i.Metadata.Status == metadataStatus2);

            return (toShowMetadataChannels, toUpdateMetadataChannels);
        }

        public (IEnumerable<OPMF.Entities.IMetadataChannel>, IEnumerable<OPMF.Entities.IMetadataChannel>) SplitFromStatus(IEnumerable<OPMF.Entities.IMetadataChannel> metadataChannels,
                                                                                                                       OPMF.Entities.MetadataStatus metadataStatus)
        {
            IEnumerable<OPMF.Entities.IMetadataChannel> toUpdateMetadataChannels = metadataChannels.Where(i => i.Metadata.Status != metadataStatus);
            IEnumerable<OPMF.Entities.IMetadataChannel> toShowMetadataChannels = metadataChannels.Where(i => !toUpdateMetadataChannels.Any(j => j.Metadata.Id == i.Metadata.Id));

            return (toShowMetadataChannels, toUpdateMetadataChannels);
        }

        public void SaveMetadataChanges(IEnumerable<OPMF.Entities.IMetadataChannel> metadataChannels)
        {
            try
            {
                IEnumerable<OPMF.Entities.IMetadata> metadatas = metadataChannels.Select(i => i.Metadata);

                OPMF.Database.DatabaseAdapter.AccessDbAdapter(dbAdapter =>
                {
                    dbAdapter.YoutubeMetadataDbCollection.UpdateStatus(metadatas);
                });
            }
            catch (Exception e)
            {
                OPMF.Logging.Logger.GetCurrent().LogEntry(new OPMF.Entities.OPMFError(e)
                {
                    Variables = new Dictionary<string, object>
                    {
                        { "metadataChannels", metadataChannels }
                    }
                });
                throw e;
            }
        }
    }
}
