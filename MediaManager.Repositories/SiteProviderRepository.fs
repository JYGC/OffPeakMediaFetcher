namespace MediaManager.Repositories

open MediaManager.Database

module SiteProviderRepository =
    let addMultipleSiteProvider siteProviders =
        SiteProviderRepositoryDefinitions.addMultipleSiteProvider
            DatabaseContext.databaseConnection
            DatabaseContext.databaseTables
            siteProviders
    let getSiteProviderByNames names =
        SiteProviderRepositoryDefinitions.getSiteProviderByNames
            DatabaseContext.databaseConnection
            DatabaseContext.databaseTables
            names