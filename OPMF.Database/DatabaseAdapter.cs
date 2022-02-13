using LiteDB;
using System;
using System.Threading;

namespace OPMF.Database
{
    public class DatabaseAdapter : IDisposable
    {
        // --- Flags ---
        private const string CONNECTION = "shared";

        // --- Dynamic objects ---
        private string __dbPath;
        private LiteDatabase __db;

        // Collections
        private YoutubeMetadataDbCollection __youtubeMetadataDbCollection = null;
        public YoutubeMetadataDbCollection YoutubeMetadataDbCollection
        {
            get
            {
                return __youtubeMetadataDbCollection;
            }
        }
        private YoutubeChannelDbCollection __youtubeChannelDbCollection = null;
        public YoutubeChannelDbCollection YoutubeChannelDbCollection
        {
            get
            {
                return __youtubeChannelDbCollection;
            }
        }
        private OPMFLogDbCollection __oPMFLogDbCollection = null;
        public OPMFLogDbCollection OPMFLogDbCollection
        {
            get
            {
                return __oPMFLogDbCollection;
            }
        }

        public DatabaseAdapter(string dbPath)
        {
            __dbPath = dbPath;
            __db = new LiteDatabase(string.Format(@"Filename={0};connection={1}", __dbPath, CONNECTION));

            __youtubeMetadataDbCollection = new YoutubeMetadataDbCollection(__db);
            __youtubeChannelDbCollection = new YoutubeChannelDbCollection(__db);
            __oPMFLogDbCollection = new OPMFLogDbCollection(__db);
        }

        public void MigrateData()
        {
            DatabaseAdapter newDatabaseAdapter = new DatabaseAdapter(Settings.ConfigHelper.ReadonlySettings.GetDatabasePath() + ".new");
            newDatabaseAdapter.YoutubeMetadataDbCollection.InsertBulk(__youtubeMetadataDbCollection.GetAll());
            newDatabaseAdapter.YoutubeChannelDbCollection.InsertBulk(__youtubeChannelDbCollection.GetAll());
            newDatabaseAdapter.OPMFLogDbCollection.InsertBulk(__oPMFLogDbCollection.GetAll());
        }

        public void Dispose()
        {
            __db.Dispose();
        }

        // --- Static object ---
        public static void AccessDbAdapter(Action<DatabaseAdapter> DbAction)
        {
            try
            {
                using (DatabaseAdapter databaseAdapter = new DatabaseAdapter(Settings.ConfigHelper.ReadonlySettings.GetDatabasePath()))
                {
                    DbAction(databaseAdapter);
                }
            }
            catch (Exception e)
            {
                TextLogging.TextLog.GetCurrent().LogError(e.ToString());
                throw e;
            }
        }
    }
}
