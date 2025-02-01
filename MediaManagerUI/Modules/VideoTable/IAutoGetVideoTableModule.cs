namespace MediaManagerUI.Modules.VideoTable
{
    public interface IAutoGetVideoTableModule: IGetVideoTableModule
    {
        Task AutoLoadGetResultsAsync();
    }
}
