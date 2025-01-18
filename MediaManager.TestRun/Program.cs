using MediaManager.Repositories;
using MediaManager.Dtos;
using System.Text.Json;

var newSiteProviders = new List<SiteProviderDtos.FullSiteProviderDto>
{
    new(Guid.Empty, "Vemeo", "Vemeo.com"),
    new(Guid.Empty, "Youtube", "youtube.com"),
    new(Guid.Empty, "Dailymotion", "dailymotion.com")
};

var addTask = SiteProviderRepository.AddMultipleSiteProvider(newSiteProviders);
_ = addTask.Result;

var names = new List<string>
{
    "Vimeo",
    "Youtube"
};
var testValue = SiteProviderRepository.GetSiteProviderByNames(names);

var jsonResult = JsonSerializer.Serialize(testValue);
Console.WriteLine(jsonResult);