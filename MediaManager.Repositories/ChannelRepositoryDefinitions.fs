namespace MediaManager.Repositories

open System
open System.Data.SQLite
open Dapper.FSharp.SQLite
open MediaManager.Database.DatabaseSchemas
open MediaManager.Database.Types.SqliteTypes
open MediaManager.Models.ChannelModels
open MediaManager.Dtos.ChannelDtos

module ChannelRepositoryDefinitions =
    let getAll
      (dbConnection: SQLiteConnection)
      (dataBaseTables: DatabaseTables) =
        let getAllTask =
            select {
                for c in dataBaseTables.Channels do
                    leftJoin ct in dataBaseTables.ChannelThumbnails on (c.Id = ct.Channel_Id)
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
