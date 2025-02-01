namespace MediaManagerUI.Modules.VideoTable
{
    public interface IDatabaseSearchableGetVideoTableModule
    {
        string? SearchVideoTitle { get; set; }
        string? SearchChannelName { get; set; }
    }
}
