namespace MediaManager.Models

open MediaManager.Database.Types.DataTypes

module ChannelModels =
    [<CLIMutable>]
    type Channel = {
        Id: SqliteGuid
        SiteId: string
        Url: string
        Name: string
        Description: string
        BlackListed: bool
        IsAddedBySingleVideo: bool
        NotFound: bool
        LastCheckedOut: SqliteUnixTime
        LastActivityDate: SqliteUnixTime
        SiteProvider_Id: SqliteGuid
    }
    
    [<CLIMutable>]
    type ChannelThumbnail = {
        Channel_Id: SqliteGuid
        Url: string
        Width: int
        Height: int
    }