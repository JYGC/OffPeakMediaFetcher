namespace MediaManager.Initializations

open OPMF.Settings
open MediaManager.Definitions.DatabaseContextDefinitions
open MediaManager.Database

module DatabaseContextComposition =
    let connectionType = "shared"
    let databasePath = ConfigHelper.ReadonlySettings.GetDatabasePath()
    let getDatabaseConnection: unit -> Result<TDatabaseConnection, exn> =
        fun _ -> DatabaseContext.getDatabaseConnection databasePath connectionType

    let channelCollectionName = "YoutubeChannel"
    let getChannelCollection
      : unit -> Result<TChannelCollection, exn> =
        fun _ -> DatabaseContext.getChannelCollection getDatabaseConnection channelCollectionName
    let getChannelCollectionWithDatabaseConnection
      : unit -> Result<TChannelCollection * TDatabaseConnection, exn> =
        fun _ ->
            DatabaseContext.getChannelCollectionWithDatabaseConnection
                getDatabaseConnection
                channelCollectionName

    let metadataCollectionName = "YoutubeMetadata"
    let getMetadataCollection: unit -> Result<TMetadataCollection, exn> =
        fun _ -> DatabaseContext.getMetadataCollection getDatabaseConnection metadataCollectionName
    let getMetadataCollectionWithDatabaseConnection
      : unit -> Result<TMetadataCollection * TDatabaseConnection, exn> =
        fun _ ->
            DatabaseContext.getMetadataCollectionWithDatabaseConnection
                getDatabaseConnection
                metadataCollectionName