namespace OPMF.Entities
{
    public interface IMetadataChannel
    {
        IPropertyChangedMetadata Metadata { get; set; }
        IChannel Channel { get; set; }
    }

    /// <summary>
    /// This is needed for VideoBrowser DataGrid to show video with channel name.
    /// </summary>
    public class MetadataChannel : IMetadataChannel
    {
        public IPropertyChangedMetadata Metadata { get; set; }
        public IChannel Channel { get; set; }
    }
}
