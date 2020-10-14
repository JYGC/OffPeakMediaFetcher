using System.Collections.Generic;
using Xunit;

namespace OPMF.SiteAdapter.Youtube.Tests
{
    public class TestRssChannelImporter
    {
        private string __fiveChannelsXml = @"<opml version=""1.1"">
<body>
<outline text=""Patrick"" title=""Patrick"" type=""rss"" xmlUrl=""https://www.youtube.com/feeds/videos.xml?channel_id=UC-EREEErQQqgYNyNB4YGQnQ"" />
<outline text=""Violineest"" title=""Violineest"" type=""rss"" xmlUrl=""https://www.youtube.com/feeds/videos.xml?channel_id=UC-E_nLi_776jj2gvKVgs0EQ"" />
<outline text=""Mimzy Vidz"" title=""Mimzy Vidz"" type=""rss"" xmlUrl=""https://www.youtube.com/feeds/videos.xml?channel_id=UC-Y7_v54xPxZWKDeugfID4w"" />
<outline text=""Kimera"" title=""Kimera"" type=""rss"" xmlUrl=""https://www.youtube.com/feeds/videos.xml?channel_id=UC-etSr9FDR-7XphnHr-hYqA"" />
<outline text=""Ann Lieven"" title=""Ann Lieven"" type=""rss"" xmlUrl=""https://www.youtube.com/feeds/videos.xml?channel_id=UC-jjYbFILVe9B6ab-RGOxpw"" />
</body>
</opml>";
        private IEnumerable<Entities.IChannel> __fiveChannels = new Entities.IChannel[]
        {
            new Entities.YoutubeChannel
            {
                Id = "UC-EREEErQQqgYNyNB4YGQnQ"
                , Name = "Patrick"
            }
            , new Entities.YoutubeChannel
            {
                Id = "UC-E_nLi_776jj2gvKVgs0EQ"
                , Name = "Violineest"
            }
            , new Entities.YoutubeChannel
            {
                Id = "UC-Y7_v54xPxZWKDeugfID4w"
                , Name = "Mimzy Vidz"
            }
            , new Entities.YoutubeChannel
            {
                Id = "UC-etSr9FDR-7XphnHr-hYqA"
                , Name = "Kimera"
            }
            , new Entities.YoutubeChannel
            {
                Id = "UC-jjYbFILVe9B6ab-RGOxpw"
                , Name = "Ann Lieven"
            }
        };

        [Fact]
        public void ValidXmlFiveChannels()
        {
            RssChannelImporter rssChannelImporter = new RssChannelImporter(__fiveChannelsXml);
            IEnumerable<Entities.IChannel> channels = rssChannelImporter.ImportChannels();
            Assert.Equal(__fiveChannels, channels);
        }

        [Fact]
        public void Add()
        {
            Assert.Equal(3, 2 + 1);
        }

        [Fact]
        public void Wrong()
        {
            Assert.True(false);
        }
    }
}
