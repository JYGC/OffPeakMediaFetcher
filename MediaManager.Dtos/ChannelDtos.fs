namespace MediaManager.Dtos

open System

module ChannelDtos =
    type ChannelThumbnailDto(channel_id, url, width, height) =
        member val public Channel_Id: Guid = channel_id with get,set
        member val public Url: string = url with get, set
        member val public Width: int = width with get, set
        member val public Height: int = height with get, set

    type ChannelDto(
        id, 
        siteId, 
        url, 
        name, 
        thumbnail, 
        description,
        blackListed,
        isAddedBySingleVideo,
        notFound,
        lastCheckedOut,
        lastActivityDate
    ) =
        member val public Id: Guid = id with get,set
        member val public SiteId: string = siteId with get, set
        member val public Url: string = url with get, set
        member val public Name: string = name with get, set
        member val public Thumbnail: ChannelThumbnailDto option = thumbnail with get, set
        member val public Description: string = description with get, set
        member val public BlackListed: bool = blackListed with get, set
        member val public IsAddedBySingleVideo: bool = isAddedBySingleVideo with get, set
        member val public NotFound: bool = notFound with get, set
        member val public LastCheckedOut: DateTime = lastCheckedOut with get, set
        member val public LastActivityDate: DateTime = lastActivityDate with get, set
