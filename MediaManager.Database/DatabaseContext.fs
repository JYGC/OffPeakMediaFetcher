namespace MediaManager.Database

open System.Data.SQLite
open Dapper.FSharp.SQLite
open MediaManager.Models.SiteProviderModels
open MediaManager.Models.ChannelModels
open MediaManager.Models.MetadataModels

module DatabaseContext =
    OptionTypes.register()
    let databaseFilePath = "C:\\Users\\jygcn\\AppData\\Local\\OffPeakMediaFetcherDev\\Databases\\test.db"
    let databaseConnection = new SQLiteConnection($"Data Source={databaseFilePath}")
    
    let siteProvidersTable = table'<SiteProvider> "SiteProviders"
    let channelsTable = table'<Channel> "Channels"
    let channelThumbnailsTable = table'<ChannelThumbnail> "ChannelThumbnails"
    let metadatasTable = table'<Metadata> "Metadatas"
    let metadataThumbnailsTable = table'<MetadataThumbnail> "MetadataThumbnails"