using OPMF.Database;
using OPMF.Entities;
using OPMF.SiteAdapter;
using OPMF.SiteAdapter.Youtube;
using System.Data.Common;

namespace MediaManager.Services
{
    public interface IChannelMetadataServices
    {
        List<ChannelMetadata> GetByChannelAndTitleContainingWord(
            string wordInChannelName,
            string wordInMetadataTitle, int skip, int pageSize);
        List<ChannelMetadata> GetByTitleContainingWord(string wordInMetadataTitle, int skip, int pageSize);
        List<ChannelMetadata> GetNew(int skip, int pageSize);
        List<ChannelMetadata> GetToDownloadAndWait(int skip, int pageSize);
        List<ChannelMetadata> GetVideoByUrl(string videoUrl, int skip, int pageSize);
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

        public List<ChannelMetadata> GetToDownloadAndWait(int skip, int pageSize)
        {
            return __GetChannelMetadatas((metadataDbAdapter) => metadataDbAdapter.GetToDownloadAndWait(skip, pageSize));
        }

        public List<ChannelMetadata> GetVideoByUrl(string videoUrl, int skip, int pageSize)
        {
            var siteVideoGetter = new YoutubeVideoMetadataGetter();
            var siteId = siteVideoGetter.GetSiteIdFromURL(videoUrl);

            (IMetadata? Metadata, IChannel? Channel) videoWithChannel = (null, null);
            DatabaseAdapter.AccessDbAdapter((dbAdapter) =>
            {
                videoWithChannel.Metadata = dbAdapter.YoutubeMetadataDbCollection.GetBySiteId(siteId);
                if (videoWithChannel.Metadata == null)
                {
                    videoWithChannel = siteVideoGetter.GetVideoByURL(siteId);
                    if (videoWithChannel.Channel == null || videoWithChannel.Metadata == null)
                    {
                        throw new Exception("Failed to get video");
                    }
                    dbAdapter.YoutubeMetadataDbCollection.InsertNew(new List<IMetadata> { videoWithChannel.Metadata });
                    dbAdapter.YoutubeChannelDbCollection.InsertOrUpdate(new List<IChannel> { videoWithChannel.Channel });
                }
                else
                {
                    videoWithChannel.Channel = dbAdapter.YoutubeChannelDbCollection.GetBySiteId(videoWithChannel.Metadata.ChannelSiteId);
                }
            });
            return [ new ChannelMetadata { Channel = videoWithChannel.Channel, Metadata = videoWithChannel.Metadata } ];
        }
    }
}
