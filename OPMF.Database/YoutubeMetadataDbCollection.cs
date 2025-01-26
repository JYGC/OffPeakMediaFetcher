using LiteDB;

namespace OPMF.Database
{
    public class YoutubeMetadataDbCollection : MetadataDbCollection<Entities.Metadata>, IMetadataDbCollection<Entities.Metadata>
    {
        private static readonly string __collectionName = "YoutubeMetadata";

        public YoutubeMetadataDbCollection(LiteDatabase db) : base(db, __collectionName) { }
    }
}
