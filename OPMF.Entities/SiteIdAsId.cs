using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
