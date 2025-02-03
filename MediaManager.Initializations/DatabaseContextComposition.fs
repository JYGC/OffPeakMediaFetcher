namespace MediaManager.Initializations

open OPMF.Settings
open MediaManager.Definitions.DatabaseContextDefinitions
open MediaManager.Database

module DatabaseContextComposition =
    let connectionType = "shared"
    let databasePath = ConfigHelper.ReadonlySettings.GetDatabasePath()
    let getDatabaseConnection: unit -> TDatabaseConnection =
        fun _ -> DatabaseContext.getDatabaseConnection databasePath connectionType

    let channelCollectionName = "YoutubeChannel"
    let getChannelCollection: unit -> TChannelCollection =
        fun _ -> DatabaseContext.getChannelCollection getDatabaseConnection channelCollectionName

    let metadataCollectionName = "YoutubeMetadata"
    let getMetadataCollection: unit -> TMetadataCollection =
        fun _ -> DatabaseContext.getMetadataCollection getDatabaseConnection metadataCollectionName