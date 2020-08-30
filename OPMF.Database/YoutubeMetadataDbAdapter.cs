namespace OPMF.Database
{
    public class YoutubeMetadataDbAdapter : MetadataDbAdapter<Entities.IMetadata>, IMetadataDbAdapter<Entities.IMetadata>
    {
        private static readonly string __dbFileName = "Youtube.Metadata.db";
        private static readonly string __collectionName = "YoutubeMetadata";

        public YoutubeMetadataDbAdapter() : base(__dbFileName, __collectionName) { }
    }
}
