namespace MediaManager.Services

open System
open System.Collections.Generic
open OPMF.Entities
open MediaManager.Definitions.DatabaseContextDefinitions

module ChannelServices =
    let getAll (getCollection: unit -> TChannelCollection): ResizeArray<Channel> =
        getCollection().FindAll() |> ResizeArray<Channel>

    let getNotBacklisted (getCollection: unit -> TChannelCollection): ResizeArray<Channel> =
        getCollection().Query().Where(fun c -> c.BlackListed = false).ToList()
        |> ResizeArray<Channel>

    let getManyByWordInName
      (getCollection: unit -> TChannelCollection)
      (wordInChannelName: string)
      : ResizeArray<Channel> =
        getCollection().Query().Where(fun c -> c.Name.Contains(wordInChannelName)).ToList()
        |> ResizeArray<Channel>

    let private _updateExistingChannelsAndReturnThem
      (updateFunction: Channel -> Map<string, Channel> -> unit)
      (channelCollection: TChannelCollection)
      (siteIdChannelFromUiMap: Map<string, Channel>)
      : List<Channel> * int =
        let channelsFromUiSiteIds: List<string> = siteIdChannelFromUiMap |> Map.keys |> ResizeArray
        let channelsToUpdate = channelCollection.Query().Where(fun c ->
            channelsFromUiSiteIds.Contains(c.SiteId)).ToList()
        channelsToUpdate
        |> Seq.iter(fun channelFromDb -> updateFunction channelFromDb siteIdChannelFromUiMap)
        let updateNumber = channelCollection.Update(channelsToUpdate)
        (channelsToUpdate, updateNumber)

    let private _insertOrUpdate
      (getCollectionAndDbCollection: unit -> TChannelCollection * TDatabaseConnection)
      (updateExistingChannelsAndReturnThem:
        (Channel -> Map<string, Channel> -> unit)
        -> TChannelCollection
        -> Map<string,Channel>
        -> List<Channel> * int)
      (channelsFromUi: Channel seq)
      : Result<int * int, exn> =
        let (channelCollection, dbConnection) = getCollectionAndDbCollection()
        try
            let siteIdChannelFromUiMap =
                channelsFromUi |> Seq.map(fun c -> (c.SiteId, c)) |> Map.ofSeq

            let updateFunction
              (channelFromDb: Channel)
              (siteIdToChannelsFromUiMap: Map<string,Channel>)
              : unit =
                let channelFromUi = Map.find channelFromDb.SiteId siteIdToChannelsFromUiMap
                channelFromDb.Name <- channelFromUi.Name // Use channels from Ui instead?
                if not (String.IsNullOrWhiteSpace(channelFromUi.Description)) then
                    channelFromDb.Description <- channelFromUi.Description
                channelFromDb.Thumbnail.Url <- channelFromUi.Thumbnail.Url
                channelFromDb.Thumbnail.Width <- channelFromUi.Thumbnail.Width
                channelFromDb.Thumbnail.Height <- channelFromUi.Thumbnail.Height
                channelFromDb.Url <- channelFromUi.Url

            let (channelsToUpdate, updateNumber) =
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
            let insertNumber = channelCollection.InsertBulk(newChannels)

            dbConnection.Commit() |> ignore
            Ok (insertNumber, updateNumber)
        with e ->
            dbConnection.Rollback() |> ignore
            Error e

    let insertOrUpdate
      (getCollectionAndDbCollection: unit -> TChannelCollection * TDatabaseConnection)
      (channelsFromUi: Channel seq)
      : Result<int * int, exn> =
        _insertOrUpdate
            getCollectionAndDbCollection
            _updateExistingChannelsAndReturnThem
            channelsFromUi

    //let private UpdateLastCheckedOutAndActivity