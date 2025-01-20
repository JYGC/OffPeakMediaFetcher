open System
open System.Collections.Generic
open System.Text.Json
open MediaManager.Dtos.SiteProviderDtos
open MediaManager.Repositories

let newSiteProviders = new List<FullSiteProviderDto>()
newSiteProviders.Add(new FullSiteProviderDto(Guid.Empty, "Vimeo", "Vimeo.com"))
newSiteProviders.Add(new FullSiteProviderDto(Guid.Empty, "Youtube", "youtube.com"))
newSiteProviders.Add(new FullSiteProviderDto(Guid.Empty, "Dailymotion", "dailymotion.com"))

SiteProviderRepository.addMultipleSiteProvider newSiteProviders
|> ignore

let names = new List<string>()
names.Add("Vimeo")
names.Add("Youtube")
let testvalue = SiteProviderRepository.getSiteProviderByNames names

let jsonResult =
    testvalue
    |> JsonSerializer.Serialize

printfn $"{jsonResult}"