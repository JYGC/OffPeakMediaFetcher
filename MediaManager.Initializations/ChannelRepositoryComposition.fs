namespace MediaManager.Initializations

open System
open OPMF.Entities
open MediaManager.Repositories

module ChannelRepositoryComposition =
    let getAll: unit -> ResizeArray<Channel> =
        fun _ -> ChannelRepository.getAll DatabaseContextComposition.getChannelCollection

    let insertOrUpdate: Channel seq -> Exception option =
        ChannelRepository.insertOrUpdate
            DatabaseContextComposition.getChannelCollectionWithDatabaseConnection