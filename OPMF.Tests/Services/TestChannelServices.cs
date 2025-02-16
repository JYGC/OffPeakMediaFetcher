﻿using System;
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
            Assert.Equal(expectedChannel.Blacklisted, actualChannel.Blacklisted);
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
            var channel45List = ChannelTestData.ChannelList1.Take(ChannelTestData.ChannelList1.Length * 4 / 5).ToList();
            var result = ChannelServices.InsertOrUpdate(channel45List);
            Assert.True(result.IsOk);
            var (insertNumber, updateNumber) = result.ResultValue;
            Assert.Equal(channel45List.Count, insertNumber);
            Assert.Equal(0, updateNumber);
            var siteIdToChannelsFromDb = ChannelServices.GetAll().ResultValue.ToDictionary(c => c.SiteId, c => c);

            Assert.All(channel45List, c =>
            {
                Assert.Contains(c.SiteId, (IDictionary<string, Channel>)siteIdToChannelsFromDb);
                TestChannelServiceHelper.AssertChannelIsEqual(c, siteIdToChannelsFromDb[c.SiteId]);
            });
        }

        [Fact, TestPriority(2)]
        public void Test2UpdateExisting()
        {
            var channel23List = ChannelTestData.ChannelList1.Take(ChannelTestData.ChannelList1.Length * 2 / 3).ToList();
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

            Assert.All(modifiedChannels, c =>
            {
                Assert.Contains(c.SiteId, (IDictionary<string, Channel>)siteIdToChannelsFromDb);
                TestChannelServiceHelper.AssertChannelIsEqual(c, siteIdToChannelsFromDb[c.SiteId]);
            });
        }

        [Fact, TestPriority(3)]
        public void Test3InsertAndUpdate()
        {
            var result = ChannelServices.InsertOrUpdate(ChannelTestData.ChannelList1);
            Assert.True(result.IsOk);
            var (insertNumber, updateNumber) = result.ResultValue;
            var channelListCount = ChannelTestData.ChannelList1.Length;
            var channel45ListCount = ChannelTestData.ChannelList1.Take(channelListCount * 4 / 5).Count();
            Assert.Equal(channelListCount - channel45ListCount, insertNumber);
            Assert.Equal(channel45ListCount, updateNumber);
            var siteIdToChannelsFromDb = ChannelServices.GetAll().ResultValue.ToDictionary(c => c.SiteId, c => c);

            Assert.All(ChannelTestData.ChannelList1, c =>
            {
                Assert.Contains(c.SiteId, (IDictionary<string, Channel>)siteIdToChannelsFromDb);
                TestChannelServiceHelper.AssertChannelIsEqual(c, siteIdToChannelsFromDb[c.SiteId]);
            });
        }
    }

    public class TestChannelServicesInsertOrUpdateDuplicates : IClassFixture<AppFolderFixture>
    {
        [Fact]
        public void Test1InsertDuplicate()
        {
            var result = ChannelServices.InsertOrUpdate(ChannelTestData.ChannelList2);
            Assert.True(result.IsError);
            Assert.IsType<LiteDB.LiteException>(result.ErrorValue);
            Assert.Contains("Cannot insert duplicate key in unique index", result.ErrorValue.Message);
            var channelsFromDb = ChannelServices.GetAll().ResultValue;
            Assert.Empty(channelsFromDb);
        }
    }

    public class TestChannelServicesGetResults : IClassFixture<AppFolderFixture>
    {
        public TestChannelServicesGetResults()
        {
            _ = ChannelServices.InsertOrUpdate(ChannelTestData.ChannelList1);
            _ = ChannelServices.InsertOrUpdate(ChannelTestData.ChannelList2);
        }

        [Fact]
        public void TestGetBySiteId()
        {
            var channelList2Index = ChannelTestData.ChannelList2.Length - 8;
            var channelList2Channel = ChannelTestData.ChannelList2[channelList2Index];
            var result = ChannelServices.GetBySiteId(channelList2Channel.SiteId);
            Assert.True(result.IsOk);
            TestChannelServiceHelper.AssertChannelIsEqual(channelList2Channel, result.ResultValue);
        }

        [Fact]
        public void TestGetNonExistingBySiteId()
        {
            var result = ChannelServices.GetBySiteId("sg dghds ghdsdsf");
            Assert.True(result.IsError);
        }

        [Fact]
        public void TestGetNotBacklisted()
        {
            var result = ChannelServices.GetNotBacklisted();
            Assert.True(result.IsOk);
            Assert.All(result.ResultValue, c => Assert.False(c.Blacklisted));
            var allNotBlacklist = ChannelServices.GetAll().ResultValue.Where(c => !c.Blacklisted).ToList();
            Assert.Equal(allNotBlacklist.Count, result.ResultValue.Count);
        }

        [Fact]
        public void TestGetManyByWordInName()
        {
            var wordInChannelName = "LetsPlay";
            var result = ChannelServices.GetManyByWordInName(wordInChannelName);
            Assert.True(result.IsOk);
            Assert.All(result.ResultValue, c => Assert.Contains(wordInChannelName, c.Name));
            var allResultsWithWordInChannelName = ChannelServices.GetAll().ResultValue
                .Where(c => c.Name != null && c.Name.Contains(wordInChannelName)).ToList();
            Assert.Equal(allResultsWithWordInChannelName.Count, result.ResultValue.Count);
        }
    }

    [TestCaseOrderer(ordererTypeName: "OPMF.Tests.PriorityOrderer", ordererAssemblyName: "OPMF.Tests")]
    public class TestChannelServicesUpdates : IClassFixture<AppFolderFixture>
    {
        public TestChannelServicesUpdates()
        {
            _ = ChannelServices.InsertOrUpdate(ChannelTestData.ChannelList1);
            _ = ChannelServices.InsertOrUpdate(ChannelTestData.ChannelList2);
        }

        [Fact, TestPriority(1)]
        public void TestUpdateLastCheckedOutAndActivity()
        {
            var updatedChannels = ChannelTestData.ChannelList1.Select(c =>
            {
                c.LastActivityDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month,
                    DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                c.LastCheckedOut = new DateTime(DateTime.Now.Year, DateTime.Now.Month,
                    DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                return c;
            }).ToList();
            var result = ChannelServices.UpdateLastCheckedOutAndActivity(updatedChannels);
            Assert.True(result.IsOk);
            Assert.Equal(updatedChannels.Count, result.ResultValue);
            var siteIdToChannelMap = ChannelServices.GetAll().ResultValue
                .ToDictionary(c => c.SiteId, c => c);
            Assert.All(updatedChannels, expectedChannel =>
                TestChannelServiceHelper.AssertChannelIsEqual(
                    expectedChannel, siteIdToChannelMap[expectedChannel.SiteId]));
        }

        [Fact, TestPriority(2)]
        public void TestUpdateBlackListStatus()
        {
            var updatedChannels = ChannelTestData.ChannelList1.Select(c =>
            {
                c.Blacklisted = true;
                return c;
            }).ToList();
            var result = ChannelServices.UpdateBlackListStatus(updatedChannels);
            Assert.True(result.IsOk);
            Assert.Equal(updatedChannels.Count, result.ResultValue);
            var siteIdToChannelMap = ChannelServices.GetAll().ResultValue
                .ToDictionary(c => c.SiteId, c => c);
            Assert.All(updatedChannels, expectedChannel =>
                TestChannelServiceHelper.AssertChannelIsEqual(
                    expectedChannel, siteIdToChannelMap[expectedChannel.SiteId]));
        }
    }
}
