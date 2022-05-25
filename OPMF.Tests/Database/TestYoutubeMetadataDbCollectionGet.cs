using OPMF.Database;
using OPMF.Entities;
using OPMF.Tests.TestData;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace OPMF.Tests.Database
{
    public class TestYoutubeMetadataDbCollectionGet
    {
        public TestYoutubeMetadataDbCollectionGet()
        {
            DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                dbAdapter.YoutubeMetadataDbCollection.InsertNew(VideoMetadata.MetadataList1);
                dbAdapter.YoutubeMetadataDbCollection.InsertNew(VideoMetadata.MetadataList2);
            });
        }

        private void AssertMetadataStatus(Func<DatabaseAdapter, IEnumerable<IMetadata>> DBCallToTest, Func<IMetadata, bool> StatusComparer)
        {
            DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                var all = dbAdapter.YoutubeMetadataDbCollection.GetAll();
                Dictionary<string, IMetadata> storedNewMetadata = DBCallToTest(dbAdapter).ToDictionary(
                    metadata => metadata.SiteId,
                    metadata => metadata
                );
                Dictionary<string, IMetadata> metadataList1Dict = VideoMetadata.MetadataList1.ToDictionary(
                    metadata => metadata.SiteId,
                    metadata => metadata
                );
                Dictionary<string, IMetadata> metadataList2Dict = VideoMetadata.MetadataList2.ToDictionary(
                    metadata => metadata.SiteId,
                    metadata => metadata
                );
                IEnumerable<string> expectedSiteIdsInDB = metadataList1Dict.Keys.Union(metadataList2Dict.Keys);
                IEnumerable<IMetadata> expectedMetadataStatus = expectedSiteIdsInDB.Select(
                    // Duplicate SiteIds in MetadataList2 must not override MetadataList1
                    siteId => metadataList1Dict.ContainsKey(siteId) ? metadataList1Dict[siteId] : metadataList2Dict[siteId]
                ).Where(metadata => StatusComparer(metadata));
                Dictionary<string, IMetadata> expectedDict = expectedMetadataStatus.ToDictionary(
                    metadata => metadata.SiteId,
                    metadata => metadata
                );
                foreach (string siteId in storedNewMetadata.Keys)
                {
                    Assert.True(StatusComparer(storedNewMetadata[siteId]));
                }
                Assert.Equal(expectedDict.Count(), storedNewMetadata.Keys.Count());
            });
        }

        [Fact]
        public void TestGetNew()
        {
            AssertMetadataStatus(dbAdapter => dbAdapter.YoutubeMetadataDbCollection.GetNew(), metadata => metadata.Status == MetadataStatus.New);
        }

        [Fact]
        public void TestGetToDownload()
        {
            AssertMetadataStatus(dbAdapter => dbAdapter.YoutubeMetadataDbCollection.GetToDownload(), metadata => metadata.Status == MetadataStatus.ToDownload);
        }

        [Fact]
        public void TestGetToDownloadAndWait()
        {
            AssertMetadataStatus(
                dbAdapter => dbAdapter.YoutubeMetadataDbCollection.GetToDownloadAndWait(),
                metadata => metadata.Status == MetadataStatus.ToDownload || metadata.Status == MetadataStatus.Wait
            );
        }
    }
}
