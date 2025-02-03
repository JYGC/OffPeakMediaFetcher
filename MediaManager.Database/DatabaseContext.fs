namespace MediaManager.Database

open LiteDB

open OPMF.Entities
open MediaManager.Definitions.DatabaseContextDefinitions

module DatabaseContext =
    let getDatabaseConnection: TGetDatabaseConnection =
        fun databasePath connectionType ->
            new LiteDatabase($"Filename={databasePath};connection={connectionType}")

    let getChannelCollection: TGetChannelCollection =
        fun getDbConnection collectionName ->
            getDbConnection().GetCollection<Channel>(collectionName)

    let getMetadataCollection: TGetMetadataCollection =
        fun getDbConnection collectionName ->
            getDbConnection().GetCollection<Metadata>(collectionName)