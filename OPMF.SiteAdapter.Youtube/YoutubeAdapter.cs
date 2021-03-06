﻿using System;
using System.Collections.Generic;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

using Newtonsoft.Json;

namespace OPMF.SiteAdapter.Youtube
{
    public class YoutubeAdapter : ISiteAdapter<Entities.IChannel, Entities.IMetadata>
    {
        private readonly string[] __apiScope = new string[] { YouTubeService.Scope.YoutubeReadonly };
        private readonly int __maxResultsPerResponse = 50;
        private readonly string __videoInfoParts = "snippet,contentDetails";
        private readonly string __channelParts = "snippet";
        private readonly string __urlSaffolding = "https://www.youtube.com/watch?v={0}";

        private YouTubeService __youtubeService;

        public YoutubeAdapter()
        {
            UserCredential credential = GoogleAuthentication.GetCredential(__apiScope);
            __youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApplicationName = this.GetType().ToString()
                , HttpClientInitializer = credential
            });
        }

        public List<Entities.IMetadata> FetchMetadata(ref List<Entities.IChannel> channels)
        {
            List<Entities.IMetadata> vidoeInfos = new List<Entities.IMetadata>();
            foreach (Entities.YoutubeChannel channel in channels)
            {
                Console.WriteLine("fetching video metadata for youtube channel: " + channel.Name);
                ActivitiesResource.ListRequest request = __youtubeService.Activities.List(__videoInfoParts);
                request.ChannelId = channel.SiteId;
                request.PublishedAfter = channel.LastCheckedOut.HasValue ? channel.LastCheckedOut.Value.AddDays(-2) : DateTime.Now.AddDays(-Settings.ConfigHelper.Config.NewChannelPastVideoDayLimit);
                bool updateLastCheckedOutAndActivity = true;
                DateTime? lastActivityDate = channel.LastActivityDate;
                DateTime checkOutDatetime;
                string nextPageToken = null;
                do
                {
                    ActivityListResponse response = request.Execute();
                    checkOutDatetime = DateTime.Now;
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
                                DateTime publishedAt = Convert.ToDateTime(activity.Snippet.PublishedAt);
                                Console.WriteLine("fetched: " + activity.Snippet.Title);
                                vidoeInfos.Add(new Entities.YoutubeMetadata()
                                {
                                    SiteId = activity.ContentDetails.Upload.VideoId
                                    , Url = string.Format(__urlSaffolding, activity.ContentDetails.Upload.VideoId)
                                    , Title = activity.Snippet.Title
                                    , Description = activity.Snippet.Description
                                    , ChannelSiteId = activity.Snippet.ChannelId
                                    , PublishedAt = publishedAt
                                });

                                lastActivityDate = (!lastActivityDate.HasValue || lastActivityDate.Value.CompareTo(publishedAt) == -1) ? publishedAt : lastActivityDate;
                            }
                        }
                        catch (Exception e)
                        {
                            updateLastCheckedOutAndActivity = false;
                            Console.WriteLine("exception thrown: " + e.Message + "\n" +
                                              "object json:\n" +
                                              JsonConvert.SerializeObject(activity) + "\n" +
                                              "skipping channel");
                        }
                    }
                    nextPageToken = request.PageToken = response.NextPageToken;
                }
                while (nextPageToken != null);

                if (updateLastCheckedOutAndActivity)
                {
                    channel.LastCheckedOut = checkOutDatetime;
                    channel.LastActivityDate = lastActivityDate;
                }
            }

            return vidoeInfos;
        }

        public List<Entities.IChannel> ImportChannels()
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
                        , Name = subscription.Snippet.Title
                        , Description = subscription.Snippet.Description
                    });
                }
                nextPageToken = request.PageToken = response.NextPageToken;
            }
            while (nextPageToken != null);

            return channels;
        }
    }
}
