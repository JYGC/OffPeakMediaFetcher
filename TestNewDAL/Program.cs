using MediaManager.Initializations;

OPMF.Settings.ConfigHelper.InitReadonlySettings();

var test = ChannelRepositoryComposition.getAll(null);
_ = test;