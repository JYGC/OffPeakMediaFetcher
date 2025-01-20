namespace MediaManager.Repositories

open System
open Dapper.FSharp.SQLite
open MediaManager.Database.TableTypes
open MediaManager.Database.DatabaseProviderTypes
open MediaManager.Database.Types.DataTypes
open MediaManager.Models.ChannelModels
open MediaManager.Dtos.ChannelDtos

module ChannelRepositoryDefinitions =
    let getAll
      (dbConnection: DatabaseConnection)
      (channelsTable: ChannelsTable)
      (channelThumbnailsTable: ChannelThumbnailsTable) =
        let getAllTask =
            select {
                for c in channelsTable do
                    leftJoin ct in channelThumbnailsTable on (c.Id = ct.Channel_Id)
                    selectAll
            }
            |> dbConnection.SelectAsyncOption<Channel, ChannelThumbnail>

        getAllTask.Result
        |> Seq.map(fun r ->
            let (c, ct) = r
            let channelThumbnail: ChannelThumbnailDto option =
                match ct with
                | Some(ct) ->
                    Some(new ChannelThumbnailDto(
                        new Guid(ct.Channel_Id),
                        ct.Url,
                        ct.Width,
                        ct.Height
                    ))
                | None -> None

            new ChannelDto(
                new Guid(c.Id),
                c.SiteId,
                c.Url,
                c.Name,
                channelThumbnail,
                c.Description,
                c.BlackListed,
                c.IsAddedBySingleVideo,
                c.NotFound,
                convertFromSqliteUnixTime(c.LastCheckedOut),
                convertFromSqliteUnixTime(c.LastActivityDate)
            )
        )
