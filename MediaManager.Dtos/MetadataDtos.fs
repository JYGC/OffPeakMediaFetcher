namespace MediaManager.Dtos

open System

module MetadataDtos =
    type MetadataDto (id, siteId, url, title, description) =
        member val public Id: Guid = id with get, set
        member val public SiteId: string = siteId with get, set
        member val public Url: string = url with get, set
        member val public Title: string = title with get, set
        member val public Description: string = description with get, set