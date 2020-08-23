using System;
using System.Collections.Generic;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Newtonsoft.Json;

namespace OPMF.SiteAdapter.Youtube
{
    public class YoutubeAdapter : ISiteAdapter
    {
        private readonly string[] __apiScope = new string[] { YouTubeService.Scope.YoutubeReadonly };
        private readonly int __maxResultsPerResponse = 50;
        private readonly string __videoInfoParts = "snippet,contentDetails";
        private readonly string __channelParts = "snippet";
        private readonly string __youtubeChannelDbName = "Youtube.Channels.db";
        private readonly string __youtubeVideoInfoDbName = "Youtube.VideoInfos.db";

        private YouTubeService __youtubeService;
        private YoutubeChannelDbAdapter __channelDbAdapter;
        private YoutubeVideoInfoDbAdapter __videoInfoDbAdapter;

        public YoutubeAdapter()
        {
            UserCredential credential = GoogleAuth.GetCredential(__apiScope);
            __youtubeService = new YouTubeService(new BaseClientService.Initializer() {
                ApplicationName = this.GetType().ToString()
                , HttpClientInitializer = credential
            });

            Console.WriteLine("connecting to databases");
            __channelDbAdapter = new YoutubeChannelDbAdapter(__youtubeChannelDbName);
            __videoInfoDbAdapter = new YoutubeVideoInfoDbAdapter(__youtubeVideoInfoDbName);
        }

        public void Migrate()
        {
            //
        }

        public void FetchVideoInfos()
        {
            List<YoutubeVideoInfo> vidoeInfos = new List<YoutubeVideoInfo>();

            List<YoutubeChannel> channels = __channelDbAdapter.GetNotBacklisted();
            foreach (YoutubeChannel channel in channels)
            {
                Console.WriteLine("fetching new video informtion for youtube channel: " + channel.Name);
                ActivitiesResource.ListRequest request = this.__youtubeService.Activities.List(__videoInfoParts);
                //request.ChannelId = string.Join(",", channels.Select(channel => channel.SiteId).ToArray());
                request.ChannelId = channel.SiteId;
                request.PublishedAfter = channel.LastCheckedOut;
                bool updateLastCheckedOut = true;
                DateTime checkOutDatetime = DateTime.Now;
                string nextPageToken = null;
                do
                {
                    ActivityListResponse response = request.Execute();
                    IList<Activity> activities = response.Items;
                    foreach (Activity activity in activities)
                    {
                        try
                        {
                            if (activity.ContentDetails.Upload == null)
                            {
                                Console.WriteLine("no video id detected. skip");
                            }
                            else
                            {
                                Console.WriteLine("fetched: " + activity.Snippet.Title);
                                vidoeInfos.Add(new YoutubeVideoInfo()
                                {
                                    SiteId = activity.ContentDetails.Upload.VideoId
                                    , Title = activity.Snippet.Title
                                    , Description = activity.Snippet.Description
                                    , ChannelSiteId = activity.Snippet.ChannelId
                                    , PublishedAt = Convert.ToDateTime(activity.Snippet.PublishedAt)
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            updateLastCheckedOut = false;
                            Console.WriteLine("exception thrown: " + e.Message + "\n" +
                                              "object json:\n" +
                                              JsonConvert.SerializeObject(activity) + "\n" +
                                              "skipping channel");
                        }
                    }
                    nextPageToken = request.PageToken = response.NextPageToken;
                }
                while (nextPageToken != null);

                if (updateLastCheckedOut)
                {
                    channel.LastCheckedOut = checkOutDatetime;
                }
            }

            Console.WriteLine("saving video information to database");
            __videoInfoDbAdapter.InsertOrIgnore(vidoeInfos);
            Console.WriteLine("updating channels");
            __channelDbAdapter.InsertOrUpdate(channels);
        }

        public void ImportChannels()
        {
            List<YoutubeChannel> channels = new List<YoutubeChannel>();

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

            Console.WriteLine("saving channels to database");
            Database.DatabaseAuxillary.RemoveDuplicateIds(channels);
            __channelDbAdapter.InsertOrUpdate(channels);
        }
    }
}
