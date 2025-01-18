namespace MediaManager.Repositories

open MediaManager.Database

module SiteProviderRepository =
    let AddMultipleSiteProvider siteProviders =
        SiteProviderRepositoryDefinitions.AddMultipleSiteProvider
            DatabaseContext.databaseConnection
            DatabaseContext.databaseTables
            siteProviders
    let GetSiteProviderByNames names =
        SiteProviderRepositoryDefinitions.GetSiteProviderByNames
            DatabaseContext.databaseConnection
            DatabaseContext.databaseTables
            names