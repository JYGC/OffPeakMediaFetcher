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

        public YoutubeMetadataDbCollection YoutubeMetadataDbCollection { get; } = null;
        public YoutubeChannelDbCollection YoutubeChannelDbCollection { get; } = null;
        public OPMFLogDbCollection OPMFLogDbCollection { get; } = null;

        public DatabaseAdapter(string dbPath)
        {
            __dbPath = dbPath;
            __db = new LiteDatabase(string.Format(@"Filename={0};connection={1}", __dbPath, __CONNECTION));

            YoutubeMetadataDbCollection = new YoutubeMetadataDbCollection(__db);
            YoutubeChannelDbCollection = new YoutubeChannelDbCollection(__db);
            OPMFLogDbCollection = new OPMFLogDbCollection(__db);
        }

        public void MigrateData()
        {
            DatabaseAdapter newDatabaseAdapter = new DatabaseAdapter(Settings.ConfigHelper.ReadonlySettings.GetDatabasePath() + ".new");
            newDatabaseAdapter.YoutubeMetadataDbCollection.InsertBulk(YoutubeMetadataDbCollection.GetAll());
            newDatabaseAdapter.YoutubeChannelDbCollection.InsertBulk(YoutubeChannelDbCollection.GetAll());
            newDatabaseAdapter.OPMFLogDbCollection.InsertBulk(OPMFLogDbCollection.GetAll());
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
