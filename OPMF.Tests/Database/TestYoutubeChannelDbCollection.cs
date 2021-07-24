using Xunit;

namespace OPMF.Tests.Database
{
    public class TestYoutubeChannelDbCollection : IClassFixture<SetupFixture>
    {
        [Fact]
        public void TestBulkInsert()
        {
            OPMF.Database.DatabaseAdapter.AccessDbAdapter((dbAdapter) =>
            {
                //
            });
        }
    }
}
