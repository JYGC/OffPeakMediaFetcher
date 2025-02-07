namespace MediaManager.Database

open LiteDB

open OPMF.Entities
open MediaManager.Definitions.DatabaseContextDefinitions

module DatabaseContext =
    let getDatabaseConnection databasePath connectionType: Result<TDatabaseConnection, exn> =
        try Ok (new LiteDatabase($"Filename={databasePath};connection={connectionType}"))
        with e -> Error e

    let getChannelCollection
      (getDbConnection: unit -> Result<TDatabaseConnection, exn>)
      (collectionName: string)
      : Result<TChannelCollection, exn> =
        match getDbConnection() with
        | Error ex -> Error ex
        | Ok dbConnection ->
            try Ok (dbConnection.GetCollection<Channel>(collectionName))
            with e -> Error e

    let getChannelCollectionWithDatabaseConnection
      (getDbConnection: unit -> Result<TDatabaseConnection, exn>)
      (collectionName: string)
      : Result<TChannelCollection * TDatabaseConnection, exn> =
        match getDbConnection() with
        | Error ex -> Error ex
        | Ok dbConnection ->
            try Ok (dbConnection.GetCollection<Channel>(collectionName), dbConnection)
            with e -> Error e

    let getMetadataCollection
      (getDbConnection: unit -> Result<TDatabaseConnection, exn>)
      (collectionName: string)
      : Result<TMetadataCollection, exn> =
        match getDbConnection() with
        | Error ex -> Error ex
        | Ok dbConnection ->
            try Ok (dbConnection.GetCollection<Metadata>(collectionName))
            with e -> Error e

    let getMetadataCollectionWithDatabaseConnection
      (getDbConnection: unit -> Result<TDatabaseConnection, exn>)
      (collectionName: string)
      : Result<TMetadataCollection * TDatabaseConnection, exn> =
        match getDbConnection() with
        | Error ex -> Error ex
        | Ok dbConnection ->
            try Ok (dbConnection.GetCollection<Metadata>(collectionName), dbConnection)
            with e -> Error e