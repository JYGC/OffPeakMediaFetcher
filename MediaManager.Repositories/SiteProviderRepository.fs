namespace MediaManager.Repositories

open System.Threading.Tasks
open System.Collections.Generic
open MediaManager.Database
open MediaManager.Dtos

module SiteProviderRepository =
    let addMultipleSiteProvider: IEnumerable<SiteProviderDtos.FullSiteProviderDto> -> Task<int> =
        SiteProviderRepositoryDefinitions.addMultipleSiteProvider
            DatabaseContext.databaseConnection
            DatabaseContext.siteProvidersTable
    let getSiteProviderByNames: IEnumerable<string> -> List<SiteProviderDtos.FullSiteProviderDto> =
        SiteProviderRepositoryDefinitions.getSiteProviderByNames
            DatabaseContext.databaseConnection
            DatabaseContext.siteProvidersTable

    module Cs =
        let addMultipleSiteProvider siteProviders = addMultipleSiteProvider siteProviders
        let getSiteProviderByNames names = getSiteProviderByNames names