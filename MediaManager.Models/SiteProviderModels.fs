namespace MediaManager.Models

open MediaManager.Models.DatabaseTypeMappings

module SiteProviderModels =
    [<CLIMutable>]
    type SiteProvider = {
        Id: SqliteGuid
        Name: string
        Domain: string
    }
