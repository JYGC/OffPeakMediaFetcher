using System;
using System.Collections.Generic;

using System.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace OPMF.SiteAdapter.Youtube
{
    public class YoutubeAdapter : ISiteAdapter
    {
        private readonly string[] __apiScope = new string[] { YouTubeService.Scope.YoutubeReadonly };
        private readonly int __maxResultsPerResponse = 50;
        private readonly string __videoInfoParts = "snippet,contentDetails";
        private readonly string __channelParts = "snippet";

        private YouTubeService __youtubeService;
        private Database.DatabaseAdapter<YoutubeChannelDbCalls> __channelDatabase;
        private Database.DatabaseAdapter<YoutubeVideoInfoDbCalls> __videoInfoDatabase;

        public YoutubeAdapter()
        {
            UserCredential credential = GoogleAuth.GetCredential(__apiScope);
            __youtubeService = new YouTubeService(new BaseClientService.Initializer() {
                ApplicationName = this.GetType().ToString()
                , HttpClientInitializer = credential
            });

            Console.WriteLine("connect to database");
            __channelDatabase = new Database.DatabaseAdapter<YoutubeChannelDbCalls>("Youtube.Channels.db");
            __videoInfoDatabase = new Database.DatabaseAdapter<YoutubeVideoInfoDbCalls>("Youtube.VideoInfos.db");
        }

        public void FetchVideoInfos()
        {
            List<YoutubeVideoInfo> vidoeInfos = new List<YoutubeVideoInfo>();

            List<YoutubeChannel> channels = __channelDatabase.DBCall.GetNotBacklisted();
            ActivitiesResource.ListRequest request = this.__youtubeService.Activities.List(__videoInfoParts);
            request.ChannelId = string.Join(",", channels.Select(channel => channel.SiteId).ToArray());
            request.PublishedAfter = new DateTime(2020, 08, 13);
            string nextPageToken = null;
            do
            {
                ActivityListResponse response = request.Execute();
                IList<Activity> activities = response.Items;
                foreach (Activity activity in activities)
                {
                    vidoeInfos.Add(new YoutubeVideoInfo()
                    {
                        SiteId = activity.ContentDetails.Upload.VideoId
                        , Title = activity.Snippet.Title
                        , Description = activity.Snippet.Description
                        , ChannelSiteId = activity.Snippet.ChannelId
                        , PublishedAt = Convert.ToDateTime(activity.Snippet.PublishedAt)
                    });
                }
                nextPageToken = request.PageToken = response.NextPageToken;
            }
            while (nextPageToken != null);

            __videoInfoDatabase.DBCall.InsertOrUpdate(vidoeInfos);
        }

        public void ImportChannels()
        {
            List<YoutubeChannel> channels = new List<YoutubeChannel>();

            Console.WriteLine("import channels from google");
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
                    Console.WriteLine("import channel: " + subscription.Snippet.Title);
                    channels.Add(new YoutubeChannel()
                    {
                        SiteId = subscription.Snippet.ResourceId.ChannelId
                        , Name = subscription.Snippet.Title
                        , Description = subscription.Snippet.Description
                        , LastCheckedOut = DateTime.Now
                    });
                }
                nextPageToken = request.PageToken = response.NextPageToken;
            }
            while (nextPageToken != null);

            Database.DatabaseAuxillary.RemoveDuplicateIds(channels);

            __channelDatabase.DBCall.InsertOrUpdate(channels);
        }
    }
}
