﻿using System;

namespace OPMF.Entities
{
    public interface IVideoInfo
    {
        string SiteId { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        bool Ignore { get; set; }
        bool Downloaded { get; set; }
        DateTime PublishedAt { get; set; }
        string ChannelSiteId { get; set; }
    }

    public class VideoInfo : IVideoInfo, IId
    {
        public string Id { get; set; }
        public string SiteId
        {
            get
            {
                return Id;
            }
            set
            {
                Id = value;
            }
        }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Approved { get; set; } = false;
        public bool Ignore { get; set; } = false;
        public bool Downloaded { get; set; } = false;
        public DateTime PublishedAt { get; set; }
        public string ChannelSiteId { get; set; }
    }
}