using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublisherDomain.Models
{
    public class CoverAssignment
    {
        public int CoverId { get; set; }
        public int ArtistId { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
