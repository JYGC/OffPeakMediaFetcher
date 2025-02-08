namespace MediaManager.Services

open System
open System.Collections.Generic
open OPMF.Entities
open MediaManager.Definitions.DatabaseContextDefinitions

module ChannelServices =
    let getAll
      (getCollection: unit -> Result<TChannelCollection, exn>)
      : Result<ResizeArray<Channel>, exn> =
        match getCollection() with
        | Error ex -> Error ex
        | Ok channelCollection ->
            try
                Ok (channelCollection.FindAll() |> ResizeArray<Channel>)
            with e -> Error e

    let getNotBacklisted
      (getCollection: unit -> Result<TChannelCollection, exn>)
      : Result<ResizeArray<Channel>, exn> =
        match getCollection() with
        | Error ex -> Error ex
        | Ok channelCollection ->
            try
                Ok (channelCollection.Query().Where(fun c ->
                    c.BlackListed = false).ToList() |> ResizeArray<Channel>)
            with e -> Error e

    let getManyByWordInName
      (getCollection: unit -> Result<TChannelCollection, exn>)
      (wordInChannelName: string)
      : Result<ResizeArray<Channel>, exn> =
        match getCollection() with
        | Error ex -> Error ex
        | Ok channelCollection ->
            try
                Ok (channelCollection.Query().Where(fun c ->
                    c.Name.Contains(wordInChannelName)).ToList() |> ResizeArray<Channel>)
            with e -> Error e

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
      (channelCollection: TChannelCollection)
      (updateExistingChannelsAndReturnThem:
        (Channel -> Map<string, Channel> -> unit)
        -> TChannelCollection
        -> Map<string,Channel>
        -> List<Channel> * int)
      (inboundChannels: Channel seq)
      : int * int =
        let siteIdChannelFromUiMap =
            inboundChannels |> Seq.map(fun c -> (c.SiteId, c)) |> Map.ofSeq

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
            inboundChannels |> Seq.filter(fun c -> newChannelSiteIds.Contains(c.SiteId))
        let insertNumber = channelCollection.InsertBulk(newChannels)

        (insertNumber, updateNumber)

    let insertOrUpdate
      (getCollectionAndDbCollection: unit -> Result<TChannelCollection * TDatabaseConnection, exn>)
      (channelsFromUi: Channel seq)
      : Result<int * int, exn> =
        match getCollectionAndDbCollection() with
        | Error ex -> Error ex
        | Ok (channelCollection, dbConnection) ->
            try
                let (insertNumber, updateNumber) =
                    _insertOrUpdate
                        channelCollection
                        _updateExistingChannelsAndReturnThem
                        channelsFromUi
                dbConnection.Commit() |> ignore
                Ok (insertNumber, updateNumber)
            with e ->
                dbConnection.Rollback() |> ignore
                Error e

    let private _updateLastCheckedOutAndActivity
      (channelCollection: TChannelCollection)
      (updateExistingChannelsAndReturnThem:
        (Channel -> Map<string, Channel> -> unit)
        -> TChannelCollection
        -> Map<string,Channel>
        -> List<Channel> * int)
      (channelsFromUi: Channel seq)
      : int =
        let siteIdChannelFromUiMap =
            channelsFromUi |> Seq.map(fun c -> (c.SiteId, c)) |> Map.ofSeq

        let updateFunction
            (channelFromDb: Channel)
            (siteIdToChannelsFromUiMap: Map<string,Channel>)
            : unit =
            let channelFromUi = Map.find channelFromDb.SiteId siteIdToChannelsFromUiMap
            channelFromDb.LastCheckedOut <- channelFromUi.LastCheckedOut
            channelFromDb.LastActivityDate <- channelFromUi.LastActivityDate

        let (_, updateNumber) =
            updateExistingChannelsAndReturnThem
                updateFunction
                channelCollection
                siteIdChannelFromUiMap

        updateNumber

    let updateLastCheckedOutAndActivity
      (getCollectionAndDbCollection: unit -> Result<TChannelCollection * TDatabaseConnection, exn>)
      (channelsFromUi: Channel seq)
      : Result<int, exn> =
        match getCollectionAndDbCollection() with
        | Error ex -> Error ex
        | Ok (channelCollection, dbConnection) ->
            try
                let updateNumber =
                    _updateLastCheckedOutAndActivity
                        channelCollection
                        _updateExistingChannelsAndReturnThem
                        channelsFromUi
                dbConnection.Commit() |> ignore
                Ok updateNumber
            with e ->
                dbConnection.Rollback() |> ignore
                Error e