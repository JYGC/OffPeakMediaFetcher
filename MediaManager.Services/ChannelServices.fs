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

    let getBySiteId
      (getCollection: unit -> Result<TChannelCollection, exn>)
      (siteId: string)
      : Result<ResizeArray<Channel>, exn> =
        match getCollection() with
        | Error ex -> Error ex
        | Ok channelCollection ->
            try
                Ok (channelCollection.Query().Where(fun m -> m.SiteId = siteId).ToList())
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
      (updateFunction: Channel -> Channel -> unit)
      (channelCollection: TChannelCollection)
      (inboundChannels: Channel seq)
      : List<Channel> * int =
        let siteIdChannelFromUiMap =
            inboundChannels |> Seq.map(fun c -> (c.SiteId, c)) |> Map.ofSeq
        let channelsFromUiSiteIds: List<string> = siteIdChannelFromUiMap |> Map.keys |> ResizeArray
        let channelsToUpdate = channelCollection.Query().Where(fun c ->
            channelsFromUiSiteIds.Contains(c.SiteId)).ToList()
        channelsToUpdate
        |> Seq.iter(fun channelFromDb ->
            let inboundChannel = Map.find channelFromDb.SiteId siteIdChannelFromUiMap
            updateFunction channelFromDb inboundChannel)
        let updateNumber = channelCollection.Update(channelsToUpdate)
        (channelsToUpdate, updateNumber)

    let private _insertOrUpdate
      (channelCollection: TChannelCollection)
      (updateExistingChannelsAndReturnThem:
        (Channel -> Channel -> unit)
        -> TChannelCollection
        -> Channel seq
        -> List<Channel> * int)
      (inboundChannels: Channel seq)
      : int * int =
        let updateFunction (channelFromDb: Channel) (inboundChannel: Channel): unit =
            channelFromDb.Name <- inboundChannel.Name
            if not (String.IsNullOrWhiteSpace(inboundChannel.Description)) then
                channelFromDb.Description <- inboundChannel.Description
            channelFromDb.Thumbnail.Url <- inboundChannel.Thumbnail.Url
            channelFromDb.Thumbnail.Width <- inboundChannel.Thumbnail.Width
            channelFromDb.Thumbnail.Height <- inboundChannel.Thumbnail.Height
            channelFromDb.Url <- inboundChannel.Url

        let (channelsToUpdate, updateNumber) =
            updateExistingChannelsAndReturnThem
                updateFunction
                channelCollection
                inboundChannels

        let inboundChannelsSiteIds: List<string> =
            inboundChannels |> Seq.map(fun c -> c.SiteId) |> ResizeArray
        let channelsToUpdateSiteIds: List<string> =
            channelsToUpdate |> Seq.map(fun c -> c.SiteId) |> ResizeArray
        let newChannelSiteIds: List<string> =
            List.except channelsToUpdateSiteIds (inboundChannelsSiteIds |> Seq.toList)
            |> ResizeArray
        let newChannels =
            inboundChannels |> Seq.filter(fun c -> newChannelSiteIds.Contains(c.SiteId))
        let insertNumber = channelCollection.InsertBulk(newChannels)

        (insertNumber, updateNumber)

    let insertOrUpdate
      (getCollectionAndDbCollection: unit -> Result<TChannelCollection * TDatabaseConnection, exn>)
      (inboundChannels: Channel seq)
      : Result<int * int, exn> =
        match getCollectionAndDbCollection() with
        | Error ex -> Error ex
        | Ok (channelCollection, dbConnection) ->
            try
                let (insertNumber, updateNumber) =
                    _insertOrUpdate
                        channelCollection
                        _updateExistingChannelsAndReturnThem
                        inboundChannels
                dbConnection.Commit() |> ignore
                Ok (insertNumber, updateNumber)
            with e ->
                dbConnection.Rollback() |> ignore
                Error e

    let private _updateLastCheckedOutAndActivity
      (channelCollection: TChannelCollection)
      (updateExistingChannelsAndReturnThem:
        (Channel -> Channel -> unit)
        -> TChannelCollection
        -> Channel seq
        -> List<Channel> * int)
      (inboundChannels: Channel seq)
      : int =
        let updateFunction (channelFromDb: Channel) (inboundChannel: Channel): unit =
            channelFromDb.LastCheckedOut <- inboundChannel.LastCheckedOut
            channelFromDb.LastActivityDate <- inboundChannel.LastActivityDate

        let (_, updateNumber) =
            updateExistingChannelsAndReturnThem
                updateFunction
                channelCollection
                inboundChannels

        updateNumber

    let updateLastCheckedOutAndActivity
      (getCollectionAndDbCollection: unit -> Result<TChannelCollection * TDatabaseConnection, exn>)
      (inboundChannels: Channel seq)
      : Result<int, exn> =
        match getCollectionAndDbCollection() with
        | Error ex -> Error ex
        | Ok (channelCollection, dbConnection) ->
            try
                let updateNumber =
                    _updateLastCheckedOutAndActivity
                        channelCollection
                        _updateExistingChannelsAndReturnThem
                        inboundChannels
                dbConnection.Commit() |> ignore
                Ok updateNumber
            with e ->
                dbConnection.Rollback() |> ignore
                Error e

    let _updateBlackListStatus
      (channelCollection: TChannelCollection)
      (updateExistingChannelsAndReturnThem:
        (Channel -> Channel -> unit)
        -> TChannelCollection
        -> Channel seq
        -> List<Channel> * int)
      (inboundChannels: Channel seq)
      : int =
        let updateFunction (channelFromDb: Channel) (inboundChannel: Channel): unit =
            channelFromDb.BlackListed <- inboundChannel.BlackListed

        let (_, updateNumber) =
            updateExistingChannelsAndReturnThem
                updateFunction
                channelCollection
                inboundChannels

        updateNumber

    let updateBlackListStatus
      (getCollectionAndDbCollection: unit -> Result<TChannelCollection * TDatabaseConnection, exn>)
      (inboundChannels: Channel seq)
      : Result<int, exn> =
        match getCollectionAndDbCollection() with
        | Error ex -> Error ex
        | Ok (channelCollection, dbConnection) ->
            try
                let updateNumber =
                    _updateBlackListStatus
                        channelCollection
                        _updateExistingChannelsAndReturnThem
                        inboundChannels
                dbConnection.Commit() |> ignore
                Ok updateNumber
            with e ->
                dbConnection.Rollback() |> ignore
                Error e