namespace MediaManager.Database

open Dapper.FSharp.SQLite
open MediaManager.Models.SiteProviderModels
open MediaManager.Models.ChannelModels
open MediaManager.Models.MetadataModels

module DatabaseSchemas =
    type DatabaseTables () =
        member val SiteProviders = table'<SiteProvider> "SiteProviders" with get
        member val Channels = table'<Channel> "Channels" with get
        member val ChannelThumbnails = table'<ChannelThumbnail> "ChannelThumbnails" with get
        member val Metadatas = table'<Metadata> "Metadatas" with get
        member val MetadataThumbnails = table'<MetadataThumbnail> "MetadataThumbnails" with get

