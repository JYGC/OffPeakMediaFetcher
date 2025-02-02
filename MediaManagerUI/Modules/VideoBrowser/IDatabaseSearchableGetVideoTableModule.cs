namespace MediaManagerUI.Modules.VideoBrowser
{
    public interface IDatabaseSearchableGetVideoTableModule
    {
        string? SearchVideoTitle { get; set; }
        string? SearchChannelName { get; set; }
    }
}
