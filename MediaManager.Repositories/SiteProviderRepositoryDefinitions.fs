namespace MediaManager.Repositories

open System
open System.Collections.Generic
open System.Data.SQLite
open Dapper.FSharp.SQLite
open MediaManager.Database.DatabaseSchemas
open MediaManager.Dtos.SiteProviderDtos
open MediaManager.Models.SiteProviderModels

module SiteProviderRepositoryDefinitions =
    let addMultipleSiteProvider
      (dbConnection: SQLiteConnection)
      (dataBaseTables: DatabaseTables)
      (siteProviders: IEnumerable<FullSiteProviderDto>) =
        let newSiteProviders = [
            for sp in siteProviders ->
                {
                    Id = Guid.NewGuid() |> string;
                    Name = sp.Name;
                    Domain = sp.Domain
                }
        ]

        insert {
            into dataBaseTables.SiteProviders
            values newSiteProviders
        }
        |> dbConnection.InsertAsync

    let getSiteProviderByNames
      (dbConnection: SQLiteConnection)
      (dataBaseTables: DatabaseTables)
      (names: IEnumerable<string>)
      : List<FullSiteProviderDto> =
        let namesList = names |> Seq.toList
        let getSiteProvidersTask =
            select {
                for sp in dataBaseTables.SiteProviders do
                    where (isIn sp.Name namesList)
            }
            |> dbConnection.SelectAsync<SiteProvider>

        getSiteProvidersTask.Result
        |> Seq.map(fun sp ->
            new FullSiteProviderDto(
                new Guid(sp.Id),
                sp.Name,
                sp.Domain
            )
        )
        |> ResizeArray<FullSiteProviderDto>
