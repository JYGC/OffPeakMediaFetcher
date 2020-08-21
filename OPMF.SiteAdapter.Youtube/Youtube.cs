using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

using Newtonsoft.Json;
using OPMF.Entities;

namespace OPMF.SiteAdapter.Youtube
{
    public class Youtube : ISiteAdapter
    {
        private readonly int __maxResultsPerResponse = 5;
        private readonly string __videoInfoParts = "snippet,contentDetails";
        private readonly string __channelParts = "snippet";

        private YouTubeService __youtubeService;
        private Database.IDatabase<Entities.IChannel> __channelDatabase;
        private Database.IDatabase<Entities.IVideoInfo> __videoInfoDatabase;

        public Youtube(UserCredential credential = null)
        {
            __youtubeService = new YouTubeService(new BaseClientService.Initializer() {
                ApplicationName = this.GetType().ToString()
                , HttpClientInitializer = credential
            });

            __channelDatabase = new Database.Unqlite.Unqlite<Entities.IChannel>("Youtube.Channels.db");
            __videoInfoDatabase = new Database.Unqlite.Unqlite<Entities.IVideoInfo>("Youtube.VideoInfos.db");
        }

        //private TReturn __fetchItemList<TReturn>()
        //{
        //    string 
        //}

        public void FetchVideoInfos()
        {
            Dictionary<string, Entities.IVideoInfo> vidoeInfos = new Dictionary<string, Entities.IVideoInfo>();

            Dictionary<string, Entities.IChannel> channels = __channelDatabase.GetAll().Where(
                channel => channel.Value.BlackListed = false
            ).ToDictionary(item => item.Key, item => item.Value);
            ActivitiesResource.ListRequest request = this.__youtubeService.Activities.List(__videoInfoParts);
            request.ChannelId = string.Join(",", channels.Select(channel => channel.Value.SiteId).ToArray());
            request.PublishedAfter = new DateTime(2020, 08, 13);
            string nextPageToken = null;
            do
            {
                ActivityListResponse response = request.Execute();
                IList<Activity> activities = response.Items;
                foreach (Activity activity in activities)
                {
                    vidoeInfos.Add(activity.ContentDetails.Upload.VideoId, new VideoInfo()
                    {
                        SiteId = activity.ContentDetails.Upload.VideoId
                        , Title = activity.Snippet.Title
                        , Description = activity.Snippet.Description
                        , ChannelSiteId = activity.Snippet.ChannelId
                        , PublishedAt = Convert.ToDateTime(activity.Snippet.PublishedAt)
                    });
                }
            }
            while (nextPageToken != null);

            __videoInfoDatabase.Save(vidoeInfos);
        }

        public void ImportChannels()
        {
            Dictionary<string, Entities.IChannel> channels = new Dictionary<string, Entities.IChannel>();

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
                    channels.Add(subscription.Snippet.ResourceId.ChannelId, new YoutubeChannel()
                    {
                        SiteId = subscription.Snippet.ResourceId.ChannelId
                        , Name = subscription.Snippet.Title
                        , Description = subscription.Snippet.Description
                    });
                }
                nextPageToken = request.PageToken = response.NextPageToken;
            }
            while (nextPageToken != null);

            __channelDatabase.Save(channels);
        }
    }
}
