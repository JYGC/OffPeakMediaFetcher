namespace OPMF.Entities
{
    public class SiteIdAsId
    {
        public string Id { get; set; }
        public string SiteId
        {
            get
            {
                return Id;
            }
            set
            {
                Id = value;
            }
        }
    }
}
