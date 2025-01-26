using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace OPMF.SiteAdapter.Youtube
{
    public class RssChannelImporter
    {
        private readonly string[] __apiScope = new string[] { YouTubeService.Scope.YoutubeReadonly };
        private readonly string __channelParts = "snippet";
        private readonly string __xmlUrlChannelIdFlag = "channel_id=";
        private readonly string __channelUrlScaffolding = "https://www.youtube.com/channel/{0}";

        private string __opml;
        private YouTubeService __youtubeService;

        [XmlRoot("opml")]
        public class OpmlTree
        {
            [XmlElement("body")]
            public Body body { get; set; }

            public class Body
            {
                [XmlArray("outline")]
                [XmlArrayItem("outline")]
                public Outline[] outlines { get; set; }

                public class Outline
                {
                    [XmlAttribute("text")]
                    public string Text { get; set; }

                    [XmlAttribute("title")]
                    public string Title { get; set; }

                    [XmlAttribute("type")]
                    public string Type { get; set; }

                    [XmlAttribute("xmlUrl")]
                    public string XmlUrl { get; set; }
                }
            }

        }

        public RssChannelImporter(string opml)
        {
            __opml = opml;

            __youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApplicationName = this.GetType().ToString()
                , HttpClientInitializer = GoogleAuthentication.GetCredential(__apiScope)
            });
        }

        public IEnumerable<Entities.Channel> ImportChannels()
        {
            IEnumerable<Entities.Channel> channels = new Entities.Channel[] { };

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(OpmlTree), new XmlRootAttribute("opml"));
            OpmlTree opmlTree = xmlSerializer.Deserialize(new StringReader(__opml)) as OpmlTree;

            ChannelsResource.ListRequest request = __youtubeService.Channels.List(__channelParts);
            request.MaxResults = 1;
            foreach (OpmlTree.Body.Outline outline in opmlTree.body.outlines)
            {
                string[] xmlUrlParts = outline.XmlUrl.Split(__xmlUrlChannelIdFlag);
                request.Id = xmlUrlParts[1];
                ChannelListResponse response = request.Execute();
                IList<Channel> channelList = response.Items;
                Entities.Channel channel = new Entities.YoutubeChannel {
                    SiteId = xmlUrlParts[1]
                    , Url = string.Format(__channelUrlScaffolding, xmlUrlParts[1])
                };
                if (channelList == null) // channelList becomes null if channel was deleted
                {
                    channel.Name = outline.Title;
                    channel.NotFound = true;
                }
                else
                {
                    channel.Name = channelList[0].Snippet.Title;
                    channel.Description = channelList[0].Snippet.Description;
                    channel.Thumbnail.Url = channelList[0].Snippet.Thumbnails.Default__.Url;
                    channel.Thumbnail.Width = channelList[0].Snippet.Thumbnails.Default__.Width.Value;
                    channel.Thumbnail.Height = channelList[0].Snippet.Thumbnails.Default__.Height.Value;
                }
                channels = channels.Concat(new Entities.Channel[] { channel });
            }

            return channels;
        }
    }
}
