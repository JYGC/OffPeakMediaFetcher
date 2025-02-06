namespace MediaManager.Database

open LiteDB

open OPMF.Entities
open MediaManager.Definitions.DatabaseContextDefinitions

module DatabaseContext =
    let getDatabaseConnection databasePath connectionType: TDatabaseConnection =
        new LiteDatabase($"Filename={databasePath};connection={connectionType}")

    let getChannelCollection
      (getDbConnection: unit -> TDatabaseConnection)
      (collectionName: string)
      : TChannelCollection =
        getDbConnection().GetCollection<Channel>(collectionName)

    let getChannelCollectionWithDatabaseConnection
      (getDbConnection: unit -> TDatabaseConnection)
      (collectionName: string)
      : TChannelCollection * TDatabaseConnection =
        let dbConnection = getDbConnection()
        (dbConnection.GetCollection<Channel>(collectionName), dbConnection)

    let getMetadataCollection
      (getDbConnection: unit -> TDatabaseConnection)
      (collectionName: string)
      : TMetadataCollection =
        getDbConnection().GetCollection<Metadata>(collectionName)