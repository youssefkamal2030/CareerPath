using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CareerPath.Domain.Entities.AIDataAnalysis;

namespace CareerPath.Domain.Entities
{
    public class Company
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CompanyProfile { get; set; }
        public string Location { get; set; }
        public string Website { get; set; }
        public DateTime FoundedDate { get; set; }
        public int EmployeeCount { get; set; }
        public string Industry { get; set; }
        public string LogoUrl { get; set; }
        public string Contacts { get; set; }
        public string officeLocation { get;set; }

        public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
