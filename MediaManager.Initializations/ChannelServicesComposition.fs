namespace MediaManager.Initializations

open OPMF.Entities
open MediaManager.Services

module ChannelServicesComposition =
    let getAll: unit -> Result<ResizeArray<Channel>, exn> =
        fun _ -> ChannelServices.getAll DatabaseContextComposition.getChannelCollection
    let GetAll() = getAll()

    let getNotBacklisted: unit -> Result<ResizeArray<Channel>, exn> =
        fun _ -> ChannelServices.getNotBacklisted DatabaseContextComposition.getChannelCollection
    let GetNotBacklisted() = getNotBacklisted()

    let getManyByWordInName: string -> Result<ResizeArray<Channel>, exn> =
        ChannelServices.getManyByWordInName DatabaseContextComposition.getChannelCollection
    let GetManyByWordInName wordInChannelName = getManyByWordInName wordInChannelName

    let insertOrUpdate: Channel seq -> Result<int * int, exn> =
        ChannelServices.insertOrUpdate
            DatabaseContextComposition.getChannelCollectionWithDatabaseConnection
    let InsertOrUpdate channels = insertOrUpdate channels

    let updateLastCheckedOutAndActivity: Channel seq -> Result<int, exn> =
        ChannelServices.updateLastCheckedOutAndActivity
            DatabaseContextComposition.getChannelCollectionWithDatabaseConnection
    let UpdateLastCheckedOutAndActivity channels = updateLastCheckedOutAndActivity channels