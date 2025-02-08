using OPMF.Tests.TestData;
using Xunit;

using ChannelServices = MediaManager.Initializations.ChannelServicesComposition;
using MetadataServices = MediaManager.Initializations.MetadataServicesComposition;

namespace OPMF.Tests.Services
{
    public class TestMetadataServicesGetResults : IClassFixture<AppFolderFixture>
    {
        public TestMetadataServicesGetResults()
        {
            _ = ChannelServices.InsertOrUpdate(ChannelMetadata.ChannelList1);
            _ = ChannelServices.InsertOrUpdate(ChannelMetadata.ChannelList2);
            _ = MetadataServices.InsertNew(VideoMetadata.MetadataList1);
        }

        [Fact]
        public void TestGetToDownload()
        {
            Assert.True(false);
        }

        [Fact]
        public void TestGetNew()
        {
            Assert.True(false);
        }

        [Fact]
        public void TestGetToDownloadAndWait()
        {
            Assert.True(false);
        }

        [Fact]
        public void TestGetManyByWordInTitle()
        {
            Assert.True(false);
        }

        [Fact]
        public void TestGetManyByChannelSiteIdAndWordInTitle()
        {
            Assert.True(false);
        }
    }
}
