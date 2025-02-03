namespace MediaManager.Initializations

open OPMF.Entities
open MediaManager.Repositories

module ChannelRepositoryComposition =
    let getAll: unit -> ResizeArray<Channel> =
        fun _ -> ChannelRepository.getAll DatabaseContextComposition.getChannelCollection

    module Cs =
        let getAll () = getAll()
