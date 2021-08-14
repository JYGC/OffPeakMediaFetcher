using LiteDB;

namespace OPMF.Database
{
    public class YoutubeMetadataDbCollection : MetadataDbCollection<Entities.IMetadata>, IMetadataDbCollection<Entities.IMetadata>
    {
        private static readonly string __collectionName = "YoutubeMetadata";

        public YoutubeMetadataDbCollection(LiteDatabase db) : base(db, __collectionName) { }
    }
}
