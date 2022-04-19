using System;
using System.Collections.Generic;
using System.Text;

namespace OPMF.SiteAdapter
{
    public interface ISiteChannelFinder
    {
        List<Entities.IChannel> FindChannelById(string[] channelIdList);
        List<Entities.IChannel> FindChannelByName(string[] channelNameList);
        List<Entities.IChannel> ImportChannels();
    }
}
