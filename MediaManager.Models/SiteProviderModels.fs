namespace MediaManager.Models

open MediaManager.Database.Types.DataTypes

module SiteProviderModels =
    [<CLIMutable>]
    type SiteProvider = {
        Id: SqliteGuid
        Name: string
        Domain: string
    }
