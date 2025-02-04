namespace MediaManager.Initializations

open OPMF.Entities
open MediaManager.Repositories

module ChannelRepositoryComposition =
    let private _getAll: unit -> ResizeArray<Channel> =
        fun _ -> ChannelRepository.getAll DatabaseContextComposition.getChannelCollection

    let GetAll () = _getAll()

    let private _insertOrUpdate: Channel seq -> exn option =
        ChannelRepository.insertOrUpdate
            DatabaseContextComposition.getChannelCollectionWithDatabaseConnection

    let InsertOrUpdate channels = _insertOrUpdate(channels)