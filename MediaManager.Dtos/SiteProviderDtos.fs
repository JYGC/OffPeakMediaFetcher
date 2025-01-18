namespace MediaManager.Dtos

open System

[<CLIMutable>]
module SiteProviderDtos = 
    type FullSiteProviderDto (id, name, domain) =
        member val public Id: Guid = id with get, set
        member val public Name: string = name with get, set
        member val public Domain: string = domain with get, set
