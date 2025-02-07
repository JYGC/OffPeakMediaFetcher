namespace MediaManager.Initializations

open OPMF.Entities
open MediaManager.Services

module ChannelServicesComposition =
    let getAll: unit -> ResizeArray<Channel> =
        fun _ -> ChannelServices.getAll DatabaseContextComposition.getChannelCollection
    let GetAll() = getAll()

    let insertOrUpdate: Channel seq -> Result<int * int, exn> =
        ChannelServices.insertOrUpdate
            DatabaseContextComposition.getChannelCollectionWithDatabaseConnection
    let InsertOrUpdate channels = insertOrUpdate(channels)