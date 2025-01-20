namespace MediaManager.Repositories

open System
open System.Collections.Generic
open Dapper.FSharp.SQLite
open MediaManager.Database.TableTypes
open MediaManager.Database.DatabaseProviderTypes
open MediaManager.Dtos.SiteProviderDtos
open MediaManager.Models.SiteProviderModels

module SiteProviderRepositoryDefinitions =
    let addMultipleSiteProvider
      (dbConnection: DatabaseConnection)
      (siteProvidersTable: SiteProvidersTable)
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
            into siteProvidersTable
            values newSiteProviders
        }
        |> dbConnection.InsertAsync

    let getSiteProviderByNames
      (dbConnection: DatabaseConnection)
      (siteProvidersTable: SiteProvidersTable)
      (names: IEnumerable<string>)
      : List<FullSiteProviderDto> =
        let namesList = names |> Seq.toList
        let getSiteProvidersTask =
            select {
                for sp in siteProvidersTable do
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
