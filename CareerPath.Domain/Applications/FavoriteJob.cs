using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CareerPath.Domain.Identity;
using CareerPath.Domain.Recommendations;

namespace CareerPath.Domain.Applications
{
    public class FavoriteJob
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string JobId { get; set; }
        public DateTime DateSaved { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Job Job { get; set; }
    }
}
