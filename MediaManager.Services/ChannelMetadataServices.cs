using System.Collections.Generic;

namespace MediaManager.Services
{
    public interface IChannelMetadataServices
    {
        List<OPMF.Entities.ChannelMetadata> GetByChannelAndTitleContainingWord(
            string wordInChannelName,
            string wordInMetadataTitle, int skip, int pageSize);
        List<OPMF.Entities.ChannelMetadata> GetByTitleContainingWord(string wordInMetadataTitle, int skip, int pageSize);
        List<OPMF.Entities.ChannelMetadata> GetNew(int skip, int pageSize);
    }

    public class ChannelMetadataServices : IChannelMetadataServices
    {
        public List<OPMF.Entities.ChannelMetadata> GetByChannelAndTitleContainingWord(
            string wordInChannelName,
            string wordInMetadataTitle, int skip, int pageSize)
        {
            List<OPMF.Entities.ChannelMetadata> metadataChannels = new List<OPMF.Entities.ChannelMetadata>() { };
            OPMF.Database.DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                Dictionary<string, OPMF.Entities.IChannel> channelsWithSiteId = dbAdapter.YoutubeChannelDbCollection.GetManyByWordInName(
                    wordInChannelName).ToDictionary(c => c.SiteId, c => c);

                IEnumerable<OPMF.Entities.IMetadata> metadatas = dbAdapter.YoutubeMetadataDbCollection.GetManyByChannelSiteIdAndWordInTitle(
                    channelsWithSiteId.Keys, wordInMetadataTitle, skip, pageSize);

                foreach (OPMF.Entities.IMetadata metadata in metadatas)
                {
                    metadataChannels.AddRange(new OPMF.Entities.ChannelMetadata[]
                    {
                        new OPMF.Entities.ChannelMetadata
                        {
                            Metadata = metadata,
                            Channel = channelsWithSiteId[metadata.ChannelSiteId]
                        }
                    });
                }
            });
            return metadataChannels;
        }

        public List<OPMF.Entities.ChannelMetadata> GetByTitleContainingWord(string wordInMetadataTitle, int skip, int pageSize)
        {
            return __GetChannelMetadatas((metadataDbAdapter) => metadataDbAdapter.GetManyByWordInTitle(wordInMetadataTitle, skip, pageSize));
        }

        private List<OPMF.Entities.ChannelMetadata> __GetChannelMetadatas(
            Func<OPMF.Database.IMetadataDbCollection<OPMF.Entities.IMetadata>, IEnumerable<OPMF.Entities.IMetadata>> metadataDbFunc)
        {
            List<OPMF.Entities.ChannelMetadata> metadataChannels = new List<OPMF.Entities.ChannelMetadata>() { };

            OPMF.Database.DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                IEnumerable<OPMF.Entities.IMetadata> metadatas = metadataDbFunc(dbAdapter.YoutubeMetadataDbCollection);

                List<string> channelSiteIds = metadatas.Select(s => s.ChannelSiteId).Distinct().ToList();
                Dictionary<string, OPMF.Entities.IChannel> channelsWithSiteIds = dbAdapter.YoutubeChannelDbCollection
                    .GetManyBySiteIds(channelSiteIds).ToDictionary(c => c.SiteId, c => c);

                foreach (OPMF.Entities.IMetadata metadata in metadatas)
                {
                    metadataChannels.Add(new OPMF.Entities.ChannelMetadata
                    {
                        Metadata = metadata,
                        Channel = channelsWithSiteIds[metadata.ChannelSiteId]
                    });
                }
            });

            return metadataChannels;
        }

        public List<OPMF.Entities.ChannelMetadata> GetNew(int skip, int pageSize)
        {
            return __GetChannelMetadatas((metadataDbAdapter) => metadataDbAdapter.GetNew(skip, pageSize));
        }
    }
}
