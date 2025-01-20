namespace MediaManager.Database

open Dapper.FSharp.SQLite
open MediaManager.Models.SiteProviderModels
open MediaManager.Models.ChannelModels
open MediaManager.Models.MetadataModels

module TableTypes =
    type SiteProvidersTable = QuerySource<SiteProvider>
    type ChannelsTable = QuerySource<Channel>
    type ChannelThumbnailsTable = QuerySource<ChannelThumbnail>
    type MetadatasTable = QuerySource<Metadata>
    type MetadataThumbnailsTable = QuerySource<MetadataThumbnail>
