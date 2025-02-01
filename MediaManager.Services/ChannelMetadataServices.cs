using OPMF.Entities;

namespace MediaManager.Services
{
    public interface IChannelMetadataServices
    {
        List<ChannelMetadata> GetByChannelAndTitleContainingWord(
            string wordInChannelName,
            string wordInMetadataTitle, int skip, int pageSize);
        List<ChannelMetadata> GetByTitleContainingWord(string wordInMetadataTitle, int skip, int pageSize);
        List<ChannelMetadata> GetNew(int skip, int pageSize);
    }

    public class ChannelMetadataServices : IChannelMetadataServices
    {
        public List<ChannelMetadata> GetByChannelAndTitleContainingWord(
            string wordInChannelName,
            string wordInMetadataTitle, int skip, int pageSize)
        {
            List<ChannelMetadata> metadataChannels = new List<ChannelMetadata>() { };
            OPMF.Database.DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                Dictionary<string, IChannel> channelsWithSiteId = dbAdapter.YoutubeChannelDbCollection.GetManyByWordInName(
                    wordInChannelName).ToDictionary(c => c.SiteId, c => c);

                IEnumerable<IMetadata> metadatas = dbAdapter.YoutubeMetadataDbCollection.GetManyByChannelSiteIdAndWordInTitle(
                    channelsWithSiteId.Keys, wordInMetadataTitle, skip, pageSize);

                foreach (IMetadata metadata in metadatas)
                {
                    metadataChannels.AddRange(new ChannelMetadata[]
                    {
                        new ChannelMetadata
                        {
                            Metadata = metadata,
                            Channel = channelsWithSiteId[metadata.ChannelSiteId]
                        }
                    });
                }
            });
            return metadataChannels;
        }

        public List<ChannelMetadata> GetByTitleContainingWord(string wordInMetadataTitle, int skip, int pageSize)
        {
            return __GetChannelMetadatas((metadataDbAdapter) => metadataDbAdapter.GetManyByWordInTitle(wordInMetadataTitle, skip, pageSize));
        }

        private List<ChannelMetadata> __GetChannelMetadatas(
            Func<OPMF.Database.IMetadataDbCollection<IMetadata>, IEnumerable<IMetadata>> metadataDbFunc)
        {
            List<ChannelMetadata> metadataChannels = new List<ChannelMetadata>() { };

            OPMF.Database.DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                IEnumerable<IMetadata> metadatas = metadataDbFunc(dbAdapter.YoutubeMetadataDbCollection);

                List<string> channelSiteIds = metadatas.Select(s => s.ChannelSiteId).Distinct().ToList();
                Dictionary<string, IChannel> channelsWithSiteIds = dbAdapter.YoutubeChannelDbCollection
                    .GetManyBySiteIds(channelSiteIds).ToDictionary(c => c.SiteId, c => c);

                foreach (IMetadata metadata in metadatas)
                {
                    metadataChannels.Add(new ChannelMetadata
                    {
                        Metadata = metadata,
                        Channel = channelsWithSiteIds[metadata.ChannelSiteId]
                    });
                }
            });

            return metadataChannels;
        }

        public List<ChannelMetadata> GetNew(int skip, int pageSize)
        {
            return __GetChannelMetadatas((metadataDbAdapter) => metadataDbAdapter.GetNew(skip, pageSize));
        }
    }
}
