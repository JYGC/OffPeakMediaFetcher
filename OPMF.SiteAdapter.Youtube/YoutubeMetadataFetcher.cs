using System;
using System.Collections.Generic;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

using Newtonsoft.Json;

namespace OPMF.SiteAdapter.Youtube
{
    public class YoutubeMetadataFetcher : IMetadataFetcher<Entities.IChannel, Entities.IMetadata>
    {
        private readonly string[] __apiScope = new string[] { YouTubeService.Scope.YoutubeReadonly };
        private readonly string __videoInfoParts = "snippet,contentDetails";
        private readonly string __urlSaffolding = "https://www.youtube.com/watch?v={0}";

        private YouTubeService __youtubeService;

        public YoutubeMetadataFetcher()
        {
            UserCredential credential = GoogleAuthentication.GetCredential(__apiScope);
            __youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApplicationName = GetType().ToString()
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
                                    SiteId = activity.ContentDetails.Upload.VideoId,
                                    Url = string.Format(__urlSaffolding, activity.ContentDetails.Upload.VideoId),
                                    Title = activity.Snippet.Title,
                                    Thumbnail = new Entities.EntityThumbnail
                                    {
                                        Url = activity.Snippet.Thumbnails.Default__.Url,
                                        Width = activity.Snippet.Thumbnails.Default__.Width.Value,
                                        Height = activity.Snippet.Thumbnails.Default__.Height.Value,
                                    },
                                    Description = activity.Snippet.Description,
                                    ChannelSiteId = activity.Snippet.ChannelId,
                                    PublishedAt = publishedAt
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
                            Logging.Logger.GetCurrent().LogEntry(new Entities.OPMFLog
                            {
                                Message = "skipping channel: " + e.Message,
                                Variables = new Dictionary<string, object>
                                {
                                    { "activity object json", JsonConvert.SerializeObject(activity) },
                                    { "exception thrown", e },
                                    { "nextPageToken", nextPageToken }
                                },
                                Type = Entities.OPMFLogType.Info
                            });
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
    }
}
