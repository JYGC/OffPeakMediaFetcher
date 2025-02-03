namespace MediaManager.Repositories

open OPMF.Entities
open MediaManager.Definitions.ChannelRepositoryDefinitions

module ChannelRepository =
    let getAll: TGetAll =
        fun getCollection ->
            getCollection().FindAll()
            |> ResizeArray<Channel>
