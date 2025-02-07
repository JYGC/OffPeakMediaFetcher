using System;
using System.Text.Json;
using Xunit;

using OPMF.Tests.TestData;
using ChannelServices = MediaManager.Initializations.ChannelServicesComposition;
using System.Linq;
using OPMF.Entities;
using System.Collections.Generic;

namespace OPMF.Tests.Services
{
    public class TestChannelServiceHelper
    {
        public static void AssertChannelIsEqual(Channel expectedChannel, Channel actualChannel)
        {
            Assert.Equal(expectedChannel.Name, actualChannel.Name);
            if (!string.IsNullOrWhiteSpace(expectedChannel.Description))
            {
                Assert.Equal(expectedChannel.Description, actualChannel.Description);
            }
            Assert.Equal(expectedChannel.IsAddedBySingleVideo, actualChannel.IsAddedBySingleVideo);
            Assert.Equal(expectedChannel.BlackListed, actualChannel.BlackListed);
            Assert.Equal(expectedChannel.LastActivityDate, actualChannel.LastActivityDate);
            Assert.Equal(expectedChannel.LastCheckedOut, actualChannel.LastCheckedOut);
            Assert.Equal(expectedChannel.NotFound, actualChannel.NotFound);
            Assert.Equal(expectedChannel.Url, actualChannel.Url);
            Assert.Equal(JsonSerializer.Serialize(expectedChannel.Thumbnail), JsonSerializer.Serialize(actualChannel.Thumbnail));
        }

        public static void ModifyChannel(Channel channel, int i)
        {
            channel.Name = $"{channel.Name}{from number in Enumerable.Range(0, i * i) select Math.Pow(i, i)}";
            channel.Description = $"{channel.Description}{from number in Enumerable.Range(0, i * i) select Math.Pow(i, i)}";
            //channel.IsAddedBySingleVideo = !channel.IsAddedBySingleVideo;
            //channel.BlackListed = !channel.BlackListed;
            //channel.LastActivityDate = channel.LastActivityDate.HasValue ? channel.LastActivityDate.Value.AddDays(i) : DateTime.UtcNow;
            //channel.LastCheckedOut = channel.LastCheckedOut.HasValue ? channel.LastCheckedOut.Value.AddDays(i) : DateTime.UtcNow;
            //channel.NotFound = !channel.NotFound;
            channel.Url = $"{channel.Url}{from number in Enumerable.Range(0, i * i) select Math.Pow(i, i)}";
            channel.Thumbnail.Width = 2 * i;
            channel.Thumbnail.Height = 3 * i * i;
        }
    }

    [TestCaseOrderer(ordererTypeName: "OPMF.Tests.PriorityOrderer", ordererAssemblyName: "OPMF.Tests")]
    public class TestChannelServicesInsertOrUpdate : IClassFixture<AppFolderFixture>
    {
        [Fact, TestPriority(1)]
        public void Test1InsertNew()
        {
            var channel45List = ChannelMetadata.ChannelList1.Take(ChannelMetadata.ChannelList1.Length * 4 / 5).ToList();
            var result = ChannelServices.InsertOrUpdate(channel45List);
            Assert.True(result.IsOk);
            var (insertNumber, updateNumber) = result.ResultValue;
            Assert.Equal(channel45List.Count, insertNumber);
            Assert.Equal(0, updateNumber);
            var siteIdToChannelsFromDb = ChannelServices.GetAll().ResultValue.ToDictionary(c => c.SiteId, c => c);
            
            foreach (var channel in channel45List)
            {
                Assert.Contains(channel.SiteId, (IDictionary<string, Channel>)siteIdToChannelsFromDb);
                TestChannelServiceHelper.AssertChannelIsEqual(channel, siteIdToChannelsFromDb[channel.SiteId]);
            }
        }

        [Fact, TestPriority(2)]
        public void Test2UpdateExisting()
        {
            var channel23List = ChannelMetadata.ChannelList1.Take(ChannelMetadata.ChannelList1.Length * 2 / 3).ToList();
            var modifiedChannels = new List<Channel>();
            for (int i = 0; i < channel23List.Count; i++)
            {
                var copiedChannel = JsonSerializer.Deserialize<Channel>(JsonSerializer.Serialize(channel23List[i]));
                TestChannelServiceHelper.ModifyChannel(copiedChannel, i);
                modifiedChannels.Add(copiedChannel);
            }

            var result = ChannelServices.InsertOrUpdate(modifiedChannels);
            Assert.True(result.IsOk);
            var (insertNumber, updateNumber) = result.ResultValue;
            Assert.Equal(0, insertNumber);
            Assert.Equal(modifiedChannels.Count, updateNumber);
            var siteIdToChannelsFromDb = ChannelServices.GetAll().ResultValue.ToDictionary(c => c.SiteId, c => c);

            foreach (var channel in modifiedChannels)
            {
                Assert.Contains(channel.SiteId, (IDictionary<string, Channel>)siteIdToChannelsFromDb);
                TestChannelServiceHelper.AssertChannelIsEqual(channel, siteIdToChannelsFromDb[channel.SiteId]);
            }
        }

        [Fact, TestPriority(3)]
        public void Test3InsertAndUpdate()
        {
            var result = ChannelServices.InsertOrUpdate(ChannelMetadata.ChannelList1);
            Assert.True(result.IsOk);
            var (insertNumber, updateNumber) = result.ResultValue;
            var channelListCount = ChannelMetadata.ChannelList1.Length;
            var channel45ListCount = ChannelMetadata.ChannelList1.Take(channelListCount * 4 / 5).Count();
            Assert.Equal(channelListCount - channel45ListCount, insertNumber);
            Assert.Equal(channel45ListCount, updateNumber);
            var siteIdToChannelsFromDb = ChannelServices.GetAll().ResultValue.ToDictionary(c => c.SiteId, c => c);

            foreach (var channel in ChannelMetadata.ChannelList1)
            {
                Assert.Contains(channel.SiteId, (IDictionary<string, Channel>)siteIdToChannelsFromDb);
                TestChannelServiceHelper.AssertChannelIsEqual(channel, siteIdToChannelsFromDb[channel.SiteId]);
            }
        }
    }

    [TestCaseOrderer(ordererTypeName: "OPMF.Tests.PriorityOrderer", ordererAssemblyName: "OPMF.Tests")]
    public class TestChannelServicesInsertOrUpdateDuplicates : IClassFixture<AppFolderFixture>
    {
        [Fact, TestPriority(1)]
        public void Test1InsertDuplicate()
        {
            var result = ChannelServices.InsertOrUpdate(ChannelMetadata.ChannelList2);
            Assert.True(result.IsError);
            Assert.IsType<LiteDB.LiteException>(result.ErrorValue);
            Assert.Contains("Cannot insert duplicate key in unique index", result.ErrorValue.Message);
            var channelsFromDb = ChannelServices.GetAll().ResultValue;
            Assert.Empty(channelsFromDb);
        }
    }
}
