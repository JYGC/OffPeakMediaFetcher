using LiteDB;
using OPMF.Database;
using OPMF.Entities;
using OPMF.Tests.TestData;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace OPMF.Tests.Database
{
    public class TestYoutubeMetadataDbCollectionInsertNew : IClassFixture<SetupFixture>, ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) where TTestCase : ITestCase => testCases.OrderBy(testCase => testCase.TestMethod.Method.Name);

        private void AssertMetadataPropertiesEqual(IMetadata expectedMetadata, IMetadata actualMetadata)
        {
            Assert.Equal(expectedMetadata.ChannelSiteId, actualMetadata.ChannelSiteId);
            Assert.Equal(expectedMetadata.Description, actualMetadata.Description);
            Assert.Equal(expectedMetadata.PublishedAt, actualMetadata.PublishedAt);
            Assert.Equal(expectedMetadata.Status, actualMetadata.Status);
            Assert.Equal(expectedMetadata.Title, actualMetadata.Title);
            Assert.Equal(expectedMetadata.Url, actualMetadata.Url);
        }

        [Fact]
        public void Test1InsertNew()
        {
            DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                dbAdapter.YoutubeMetadataDbCollection.InsertNew(VideoMetadata.MetadataList1);
                Dictionary<string, IMetadata> storedMetadataDict = dbAdapter.YoutubeMetadataDbCollection.GetAll().ToDictionary(
                    metadata => metadata.SiteId,
                    metadata => metadata
                );
                Dictionary<string, IMetadata> list1MetadataDict = VideoMetadata.MetadataList1.ToDictionary(metadata => metadata.SiteId, metadata => metadata);
                Assert.Empty(list1MetadataDict.Keys.Except(storedMetadataDict.Keys).Union(storedMetadataDict.Keys.Except(list1MetadataDict.Keys)));
                foreach (string siteId in list1MetadataDict.Keys)
                {
                    AssertMetadataPropertiesEqual(list1MetadataDict[siteId], storedMetadataDict[siteId]);
                }
            });
        }

        [Fact]
        public void Test2InsertNewAndDuplicates()
        {
            DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                dbAdapter.YoutubeMetadataDbCollection.InsertNew(VideoMetadata.MetadataList2);
                // Check for duplicate SiteId - will Except if duplicate SiteIds found
                Dictionary<string, IMetadata> storedMetadata = dbAdapter.YoutubeMetadataDbCollection.GetAll().ToDictionary(
                    metadata => metadata.SiteId,
                    metadata => metadata
                );
                Dictionary<string, IMetadata> list2Dict = VideoMetadata.MetadataList2.ToDictionary(metadata => metadata.SiteId, metadata => metadata);
                Dictionary<string, IMetadata> list1Dict = VideoMetadata.MetadataList1.ToDictionary(metadata => metadata.SiteId, metadata => metadata);
                Assert.Equal(list1Dict.Keys.Union(list2Dict.Keys).Count(), storedMetadata.Keys.Count());
                foreach (string siteId in list1Dict.Keys)
                {
                    // Duplicate SiteIds must not be altered
                    AssertMetadataPropertiesEqual(list1Dict[siteId], storedMetadata[siteId]);
                }
                string[] newList2SiteId = list2Dict.Keys.Except(list1Dict.Keys).ToArray();
                foreach (string siteId in newList2SiteId)
                {
                    AssertMetadataPropertiesEqual(list2Dict[siteId], storedMetadata[siteId]);
                }
            });
        }
    }
}
