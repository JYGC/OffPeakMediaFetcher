namespace OPMF.Entities
{
    public interface IMetadataChannel
    {
        IStatusChangedMetadata Metadata { get; set; }
        IChannel Channel { get; set; }
    }

    public class MetadataChannel : IMetadataChannel
    {
        public IStatusChangedMetadata Metadata { get; set; }
        public IChannel Channel { get; set; }
    }
}
