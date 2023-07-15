using LiteDB;
using System;

namespace OPMF.Database
{
    public class DatabaseAdapter : IDisposable
    {
        // --- Flags ---
        private const string __CONNECTION = "shared";

        // --- Dynamic objects ---
        private readonly string __dbPath;
        private readonly LiteDatabase __db;

        // Collections
        private readonly YoutubeMetadataDbCollection __youtubeMetadataDbCollection = null;
        public YoutubeMetadataDbCollection YoutubeMetadataDbCollection
        {
            get
            {
                return __youtubeMetadataDbCollection;
            }
        }
        private readonly YoutubeChannelDbCollection __youtubeChannelDbCollection = null;
        public YoutubeChannelDbCollection YoutubeChannelDbCollection
        {
            get
            {
                return __youtubeChannelDbCollection;
            }
        }
        private readonly OPMFLogDbCollection __oPMFLogDbCollection = null;
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
            __db = new LiteDatabase(string.Format(@"Filename={0};connection={1}", __dbPath, __CONNECTION));

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
                using DatabaseAdapter databaseAdapter = new DatabaseAdapter(Settings.ConfigHelper.ReadonlySettings.GetDatabasePath());
                DbAction(databaseAdapter);
            }
            catch (Exception e)
            {
                TextLogging.TextLog.GetCurrent().LogEntry(e.ToString());
                throw;
            }
        }
    }
}
