namespace MediaManager.Repositories

open System
open System.Collections.Generic
open OPMF.Entities
open MediaManager.Definitions.DatabaseContextDefinitions

module ChannelRepository =
    let getAll (getCollection: unit -> TChannelCollection): ResizeArray<Channel> =
        getCollection().FindAll()
        |> ResizeArray<Channel>

    let getNotBacklisted (getCollection: unit -> TChannelCollection): ResizeArray<Channel> =
        getCollection().Query().Where(fun c -> c.BlackListed = false).ToList()
        |> ResizeArray<Channel>

    let getManyByWordInName
      (getCollection: unit -> TChannelCollection)
      (wordInChannelName: string): ResizeArray<Channel> =
        getCollection().Query().Where(fun c -> c.Name.Contains(wordInChannelName)).ToList()
        |> ResizeArray<Channel>

    let private _updateExistingChannelsAndReturnThem
      (updateFunction: Channel -> Map<string, Channel> -> unit)
      (channelCollection: TChannelCollection)
      (siteIdChannelFromUiMap: Map<string, Channel>)
      : List<Channel> =
        let channelsFromUiSiteIds: List<string> = siteIdChannelFromUiMap |> Map.keys |> ResizeArray
        let channelsToUpdate = channelCollection.Query().Where(fun c ->
            channelsFromUiSiteIds.Contains(c.SiteId)).ToList()
        channelsToUpdate
        |> Seq.iter(fun channelFromDb -> updateFunction channelFromDb siteIdChannelFromUiMap)
        channelCollection.Update(channelsToUpdate) |> ignore
        channelsToUpdate

    let private _insertOrUpdate
      (getCollectionAndDbCollection: unit -> TChannelCollection * TDatabaseConnection)
      (updateExistingChannelsAndReturnThem:
        (Channel -> Map<string, Channel> -> unit)
        -> TChannelCollection
        -> Map<string,Channel>
        -> List<Channel>)
      (channelsFromUi: Channel seq)
      : Exception Option =
        let (channelCollection, dbConnection) = getCollectionAndDbCollection()
        try
            let siteIdChannelFromUiMap =
                channelsFromUi |> Seq.map(fun c -> (c.SiteId, c)) |> Map.ofSeq

            let updateFunction
              (channelFromDb: Channel)
              (siteIdChannelFromUiMap: Map<string,Channel>)
              : unit =
                let channel = Map.find channelFromDb.SiteId siteIdChannelFromUiMap
                channelFromDb.Name <- channel.Name
                channelFromDb.Url <- channel.Url
                channelFromDb.Thumbnail.Url <- channel.Thumbnail.Url
                channelFromDb.Thumbnail.Width <- channel.Thumbnail.Width
                channelFromDb.Thumbnail.Height <- channel.Thumbnail.Height
                if not (String.IsNullOrWhiteSpace(channel.Description)) then
                    channelFromDb.Description <- channel.Description

            let channelsToUpdate =
                updateExistingChannelsAndReturnThem
                    updateFunction
                    channelCollection
                    siteIdChannelFromUiMap

            let channelsFromUiSiteIds: List<string> =
                siteIdChannelFromUiMap |> Map.keys |> ResizeArray
            let channelsToUpdateSiteIds: List<string> =
                channelsToUpdate |> Seq.map(fun c -> c.SiteId) |> ResizeArray
            let newChannelSiteIds: List<string> =
                List.except channelsToUpdateSiteIds (channelsFromUiSiteIds |> Seq.toList)
                |> ResizeArray
            let newChannels =
                channelsFromUi |> Seq.filter(fun c -> newChannelSiteIds.Contains(c.SiteId))
            channelCollection.InsertBulk(newChannels) |> ignore

            dbConnection.Commit() |> ignore
            None
        with e ->
            dbConnection.Rollback() |> ignore
            Some(e)

    let insertOrUpdate
      (getCollectionAndDbCollection: unit -> TChannelCollection * TDatabaseConnection)
      (channelsFromUi: Channel seq)
      : Exception Option =
        _insertOrUpdate
            getCollectionAndDbCollection
            _updateExistingChannelsAndReturnThem
            channelsFromUi

    //let private UpdateLastCheckedOutAndActivity