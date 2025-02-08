namespace MediaManager.Initializations

open OPMF.Entities
open MediaManager.Services

module MetadataServicesComposition =
    let getToDownload: unit -> Result<ResizeArray<Metadata>, exn> =
        fun _ -> MetadataServices.getToDownload DatabaseContextComposition.getMetadataCollection
    let GetToDownload() = getToDownload()

    let getToDownloadAndWait: int -> int -> Result<ResizeArray<Metadata>, exn> =
        MetadataServices.getToDownloadAndWait DatabaseContextComposition.getMetadataCollection
    let GetToDownloadAndWait skip pageSize = getToDownloadAndWait skip pageSize

    let getNew: int -> int -> Result<ResizeArray<Metadata>, exn> =
        MetadataServices.getNew DatabaseContextComposition.getMetadataCollection
    let GetNew skip pageSize = getNew skip pageSize

    let getManyByWordInTitle: string -> int -> int -> Result<ResizeArray<Metadata>, exn> =
        MetadataServices.getManyByWordInTitle DatabaseContextComposition.getMetadataCollection
    let GetManyByWordInTitle wordInMetadataTitle skip pageSize =
        getManyByWordInTitle wordInMetadataTitle skip pageSize

    let getManyByChannelSiteIdAndWordInTitle
      : string seq -> string -> int -> int -> Result<ResizeArray<Metadata>, exn> =
        MetadataServices.getManyByChannelSiteIdAndWordInTitle
            DatabaseContextComposition.getMetadataCollection
    let GetManyByChannelSiteIdAndWordInTitle channelSiteIds wordInMetadataTitle skip pageSize =
        getManyByChannelSiteIdAndWordInTitle channelSiteIds wordInMetadataTitle skip pageSize

    let insertNew: Metadata seq -> Result<int, exn> =
        MetadataServices.insertNew DatabaseContextComposition.getMetadataCollectionWithDatabaseConnection
    let InsertNew newMetadatas = insertNew newMetadatas