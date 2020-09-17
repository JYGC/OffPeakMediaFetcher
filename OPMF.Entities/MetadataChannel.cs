namespace OPMF.Entities
{
    public interface IMetadataChannel
    {
        IPropertyChangedMetadata Metadata { get; set; }
        IChannel Channel { get; set; }
    }

    public class MetadataChannel : IMetadataChannel
    {
        public IPropertyChangedMetadata Metadata { get; set; }
        public IChannel Channel { get; set; }
    }
}
