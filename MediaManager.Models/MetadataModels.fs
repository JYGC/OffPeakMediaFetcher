namespace MediaManager.Models

open MediaManager.Models.DatabaseTypeMappings

module MetadataModels =
    [<CLIMutable>]
    type Metadata = {
        Id: SqliteGuid
        SiteId: string
        Url: string
        Title: string
        Descritpion: string
        Status: SqliteMetadataStatus
        IsBeingDownload: bool
        PublishedAt: SqliteUnixTime
        Channel_Id: SqliteGuid
    }

    [<CLIMutable>]
    type MetadataThumbnail = {
        Metadata_Id: SqliteGuid
        Url: string
        Width: int
        Height: int
    }