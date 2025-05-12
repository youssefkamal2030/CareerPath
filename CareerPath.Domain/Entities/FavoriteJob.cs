using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CareerPath.Domain.Entities.AIDataAnalysis;

namespace CareerPath.Domain.Entities
{
    public class FavoriteJob
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string JobId { get; set; }
        public DateTime DateSaved { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("JobId")]
        public virtual Job Job { get; set; }
    }
}
