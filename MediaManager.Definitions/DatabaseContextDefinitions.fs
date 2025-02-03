namespace MediaManager.Definitions

open LiteDB
open OPMF.Entities

module DatabaseContextDefinitions =
    type TDatabaseConnection = LiteDatabase
    type TChannelCollection = ILiteCollection<Channel>
    type TMetadataCollection = ILiteCollection<Metadata>

    type TGetDatabaseConnection = string -> string -> TDatabaseConnection
    type TGetChannelCollection = (unit -> TDatabaseConnection) -> string -> TChannelCollection
    type TGetMetadataCollection = (unit -> TDatabaseConnection) -> string -> TMetadataCollection