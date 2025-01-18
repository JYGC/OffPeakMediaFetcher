namespace MediaManager.Models

open MediaManager.Database.Types.SqliteTypes

module SiteProviderModels =
    [<CLIMutable>]
    type SiteProvider = {
        Id: SqliteGuid
        Name: string
        Domain: string
    }
