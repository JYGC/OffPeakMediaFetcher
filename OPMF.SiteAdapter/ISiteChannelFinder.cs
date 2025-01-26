using System.Collections.Generic;

namespace OPMF.SiteAdapter
{
    public interface ISiteChannelFinder
    {
        List<Entities.Channel> FindChannelById(string[] channelIdList);
        List<Entities.Channel> FindChannelByName(string[] channelNameList);
        List<Entities.Channel> ImportChannels();
    }
}
