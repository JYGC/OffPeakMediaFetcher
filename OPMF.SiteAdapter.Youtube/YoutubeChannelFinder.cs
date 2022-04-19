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
        private const int __maxResultsPerResponse = 50;
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

        public List<Entities.IChannel> ImportChannels() // Test this
        {
            List<Entities.IChannel> channels = new List<Entities.IChannel>();

            Console.WriteLine("importing channels from google");
            SubscriptionsResource.ListRequest request = this.__youtubeService.Subscriptions.List(__channelParts);
            request.Mine = true;
            request.MaxResults = __maxResultsPerResponse;
            string nextPageToken = null;
            do
            {
                SubscriptionListResponse response = request.Execute();
                IList<Subscription> subscriptions = response.Items;
                foreach (Subscription subscription in subscriptions)
                {
                    Console.WriteLine("importing: " + subscription.Snippet.Title);
                    channels.Add(new Entities.YoutubeChannel()
                    {
                        SiteId = subscription.Snippet.ResourceId.ChannelId
                        ,
                        Name = subscription.Snippet.Title
                        ,
                        Description = subscription.Snippet.Description
                    });
                }
                nextPageToken = request.PageToken = response.NextPageToken;
            }
            while (nextPageToken != null);

            return channels;
        }
    }
}
