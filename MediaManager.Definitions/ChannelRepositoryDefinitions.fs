namespace MediaManager.Definitions

open System.Collections.Generic
open OPMF.Entities
open DatabaseContextDefinitions

module ChannelRepositoryDefinitions =
    type TGetAll = (unit -> TChannelCollection) -> List<Channel>
    type TGetNotBlacklisted = (unit -> TChannelCollection) -> List<Channel>
    type TGetManyByWordInName = (unit -> TChannelCollection) -> string -> List<Channel>
    type TInsertOrUpdate = (unit -> TChannelCollection) -> IEnumerable<Channel> -> unit
    type TUpdateLastCheckedOutAndActivity = (unit -> TChannelCollection) -> IEnumerable<Channel> -> unit
    type TUpdateBlackListStatus = (unit -> TChannelCollection) -> IEnumerable<Channel> -> unit