using LiteDB;
using System;
using System.IO;
using System.Threading;

namespace OPMF.Database
{
    public class DatabaseAdapter : IDisposable
    {
        // --- Static object ---
        public static void AccessDbAdapter(Action<DatabaseAdapter> DbAction)
        {
            bool retryAccessingDB = true;
            while (retryAccessingDB)
            {
                try
                {
                    using (DatabaseAdapter databaseAdapter = new DatabaseAdapter(Settings.ConfigHelper.ReadonlySettings.GetDatabasePath()))
                    {
                        DbAction(databaseAdapter);
                    }
                    retryAccessingDB = false;
                }
                catch (IOException e)
                {
                    if (e.Message == @"The process cannot access the file 'C:\Users\Junying\AppData\Local\OffPeakMediaFetcher\Test\Databases\OPMF.db' because it is being used by another process.")
                    {
                        Thread.Sleep(500);
                    }
                    else
                    {
                        throw e;
                    }
                }
        }
    }

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

        public DatabaseAdapter(string dbPath)
        {
            __dbPath = dbPath;
            __db = new LiteDatabase(__dbPath);

            __youtubeMetadataDbCollection = new YoutubeMetadataDbCollection(__db);
            __youtubeChannelDbCollection = new YoutubeChannelDbCollection(__db);
        }

        public void MigrateData()
        {
            DatabaseAdapter newDatabaseAdapter = new DatabaseAdapter(Settings.ConfigHelper.ReadonlySettings.GetDatabasePath() + ".new");
            newDatabaseAdapter.YoutubeMetadataDbCollection.InsertBulk(__youtubeMetadataDbCollection.GetAll());
            newDatabaseAdapter.YoutubeChannelDbCollection.InsertBulk(__youtubeChannelDbCollection.GetAll());
        }

        public void Dispose()
        {
            __db.Dispose();
        }
    }
}
