using System;
using System.Collections.Generic;

namespace OffPeakMediaFetcher
{
    internal class MetadataFetcher
    {
        public void Run()
        {
            List<OPMF.Entities.IChannel> channels = null;
            List<OPMF.Entities.IMetadata> metadatas = null;
            try
            {
                OPMF.SiteAdapter.IMetadataFetcher<OPMF.Entities.IChannel, OPMF.Entities.IMetadata> siteAdapter = new OPMF.SiteAdapter.Youtube.YoutubeMetadataFetcher();

                Console.WriteLine("getting channels");
                OPMF.Database.DatabaseAdapter.AccessDbAdapter(dbConn =>
                {
                    channels = new List<OPMF.Entities.IChannel>(dbConn.YoutubeChannelDbCollection.GetNotBacklisted());
                });
                metadatas = siteAdapter.FetchMetadata(ref channels);
                Console.WriteLine("saving metadata to database");
                OPMF.Database.DatabaseAdapter.AccessDbAdapter(dbConn =>
                {
                    dbConn.YoutubeMetadataDbCollection.InsertNew(metadatas);
                    Console.WriteLine("updating channels");
                    dbConn.YoutubeChannelDbCollection.UpdateLastCheckedOutAndActivity(channels);
                });
            }
            catch (Exception e)
            {
                OPMF.Logging.Logger.GetCurrent().LogEntry(new OPMF.Entities.OPMFError(e)
                {
                    Variables = new Dictionary<string, object>
                    {
                        { "channels", channels },
                        { "metadatas", metadatas }
                    }
                });
                throw e;
            }
        }
    }
}
