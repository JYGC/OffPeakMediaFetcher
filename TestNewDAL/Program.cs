using ChannelRepo = MediaManager.Initializations.ChannelRepositoryComposition;
using System.Text.Json;
using OPMF.Entities;

OPMF.Settings.ConfigHelper.InitReadonlySettings();

//var test = ChannelRepo.getAll(null);
//var str = JsonSerializer.Serialize(test);
//Console.WriteLine(str);

var newVars = new List<Channel>
{
    new Channel
    {
        SiteId = "UC52kszkc084543jw",
        Name = "Arek Alfjrweg",
        BlackListed = true,
        Description = "sdf sadf asd fsadf sdaf sadf sadf sdfgskgb skag nbkjsng jsdang sadkjgh sjklg ns",
        IsAddedBySingleVideo = false,
        LastActivityDate = DateTime.Now,
        LastCheckedOut = DateTime.Now,
        NotFound = false,
        Thumbnail = new EntityThumbnail
        {
            Height = 200,
            Width = 200,
            Url = "https://www.techtarget.com/rms/onlineImages/network_attached_storage_mobile.jpg",
        },
        Url = "https://www.techtarget.com/rms/onlineImages/network_attached_storage_mobile.jpg"
    },
    new Channel
    {
        SiteId = "UC52kszkc08-acFOuogFl5jw",
        Name = "Gareth 345",
        BlackListed = true,
        Description = "description234234",
        IsAddedBySingleVideo = false,
        LastActivityDate = DateTime.Now,
        LastCheckedOut = DateTime.Now,
        NotFound = false,
        Thumbnail = new EntityThumbnail
        {
            Height = 200,
            Width = 200,
            Url = "https://www.techtarget.com/rms/onlineImages/network_attached_storage_mobile.jpg",
        },
        Url = "https://www.techtarget.com/rms/onlineImages/network_attached_storage_mobile.jpg"
    }
};

var func = ChannelRepo.insertOrUpdate;
var test1 = func.Invoke(newVars);
var test2 = test1?.Value;
_ = test2;