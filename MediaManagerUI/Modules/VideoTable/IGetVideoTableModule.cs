namespace MediaManagerUI.Modules.VideoTable
{
    public interface IGetVideoTableModule
    {
        bool IsLoading { get; }
        List<OPMF.Entities.ChannelMetadata> Results { get; }
        Task GetResultsAsync();
    }
}
