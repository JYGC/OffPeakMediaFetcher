using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;

namespace OPMF.SiteAdapter.Youtube
{
    public class YoutubeChannelFinder : ISiteChannelFinder
    {
        private const string __channelParts = "snippet";

        private readonly string[] __apiScope = new string[] { YouTubeService.Scope.YoutubeReadonly };
        private YouTubeService __youtubeService;

        public YoutubeChannelFinder()
        {
            UserCredential credential = GoogleAuthentication.GetCredential(__apiScope);
            __youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApplicationName = this.GetType().ToString()
                , HttpClientInitializer = credential
            });
        }

        public List<Entities.IChannel> FindChannelById(string[] channelIdList)
        {
            return __ImportChannels(channelIdList, (channelIdentifier, request) => { request.Id = channelIdentifier; });
        }

        public List<Entities.IChannel> FindChannelByName(string[] channelNameList)
        {
            return __ImportChannels(channelNameList, (channelIdentifier, request) => { request.ForUsername = channelIdentifier; });
        }

        private List<Entities.IChannel> __ImportChannels(string[] channelIdentifierList, Action<string, ChannelsResource.ListRequest> SetRequestParameter)
        {
            List<Entities.IChannel> newChannels = new List<Entities.IChannel>();
            ChannelsResource.ListRequest request;
            request = __youtubeService.Channels. List(__channelParts);
            foreach (string channelIdentifier in channelIdentifierList)
            {
                SetRequestParameter(channelIdentifier, request);
                ChannelListResponse nameResponse = request.Execute();

                if (nameResponse.Items == null) return null;
                foreach (Channel channel in nameResponse.Items)
                {
                    newChannels.Add(new Entities.YoutubeChannel
                    {
                        SiteId = channel.Id,
                        Url = channel.Snippet.CustomUrl,
                        Name = channel.Snippet.Title,
                        Description = channel.Snippet.Description,
                    });
                }
            }
            return newChannels;
        }
    }
}
