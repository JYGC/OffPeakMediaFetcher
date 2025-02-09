namespace MediaManager.Services

open System.Collections.Generic

open OPMF.Entities
open MediaManager.Definitions.DatabaseContextDefinitions

module MetadataServices =
    let getAll
      (getCollection: unit -> Result<TMetadataCollection, exn>)
      : Result<ResizeArray<Metadata>, exn> =
        match getCollection() with
        | Error ex -> Error ex
        | Ok channelCollection ->
            try
                Ok (channelCollection.FindAll() |> ResizeArray<Metadata>)
            with e -> Error e

    let getToDownload
      (getCollection: unit -> Result<TMetadataCollection, exn>)
      : Result<ResizeArray<Metadata>, exn> =
        match getCollection() with
        | Error ex -> Error ex
        | Ok metadataCollection ->
            try
                Ok (metadataCollection.Query().Where(fun m ->
                    m.Status = MetadataStatus.ToDownload).ToList())
            with e -> Error e

    let getToDownloadAndWait
      (getCollection: unit -> Result<TMetadataCollection, exn>)
      (skip: int)
      (pageSize: int)
      : Result<ResizeArray<Metadata>, exn> =
        match getCollection() with
        | Error ex -> Error ex
        | Ok metadataCollection ->
            try
                let waitToDownloadMetadatas =
                    [MetadataStatus.ToDownload; MetadataStatus.Wait] |> List<MetadataStatus>
                Ok (metadataCollection.Query().Where(fun m ->
                    waitToDownloadMetadatas.Contains(m.Status)
                ).Skip(skip).Limit(pageSize).ToList())
            with e -> Error e

    let getNew
      (getCollection: unit -> Result<TMetadataCollection, exn>)
      (skip: int)
      (pageSize: int)
      : Result<ResizeArray<Metadata>, exn> =
        match getCollection() with
        | Error ex -> Error ex
        | Ok metadataCollection ->
            try
                Ok (metadataCollection.Query().Where(fun m ->
                    m.Status = MetadataStatus.New).Skip(skip).Limit(pageSize).ToList())
            with e -> Error e

    let getManyByWordInTitle
      (getCollection: unit -> Result<TMetadataCollection, exn>)
      (wordInMetadataTitle: string)
      (skip: int)
      (pageSize: int)
      : Result<ResizeArray<Metadata>, exn> =
        match getCollection() with
        | Error ex -> Error ex
        | Ok metadataCollection ->
            try
                Ok (metadataCollection.Query().Where(fun m ->
                    m.Title.Contains(wordInMetadataTitle)
                ).OrderByDescending(fun i -> i.PublishedAt).Skip(skip).Limit(pageSize).ToList())
            with e -> Error e

    let getManyByChannelSiteIdAndWordInTitle
      (getCollection: unit -> Result<TMetadataCollection, exn>)
      (channelSiteIds: string seq)
      (wordInMetadataTitle: string)
      (skip: int)
      (pageSize: int)
      : Result<ResizeArray<Metadata>, exn> =
        match getCollection() with
        | Error ex -> Error ex
        | Ok metadataCollection ->
            try
                let channelSiteIdList = channelSiteIds |> ResizeArray |> List<string>
                Ok (metadataCollection.Query().Where(fun m ->
                    channelSiteIdList.Contains(m.ChannelSiteId)
                    && m.Title.Contains(wordInMetadataTitle)).Skip(skip).Limit(pageSize).ToList())
            with e -> Error e

    let insertNew
      (getCollectionAndDbCollection:
        unit -> Result<TMetadataCollection * TDatabaseConnection, exn>)
      (inboundMetadata: Metadata seq)
      : Result<int, exn> =
        match getCollectionAndDbCollection() with
        | Error ex -> Error ex
        | Ok (metadataCollection, dbConnection) ->
            try
                let inboundMetadataSiteIds =
                    inboundMetadata |> Seq.map(fun m -> m.SiteId) |> List<string>
                let existingMetadataSiteIds = metadataCollection.Query().Where(fun m ->
                    inboundMetadataSiteIds.Contains(m.SiteId)).Select(fun m -> m.SiteId).ToList()
                let newMetadataSiteIds =
                    Array.except existingMetadataSiteIds (inboundMetadataSiteIds |> Seq.toArray)
                    |> List<string>
                let newMetadata = inboundMetadata |> Seq.filter(fun m ->
                    newMetadataSiteIds.Contains(m.SiteId))
                let insertNumber = metadataCollection.InsertBulk(newMetadata)
                dbConnection.Commit() |> ignore
                Ok (insertNumber)
            with e ->
                dbConnection.Rollback() |> ignore
                Error e