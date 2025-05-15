using System;
using System.Collections.Generic;

namespace CareerPath.Contracts.Dto
{
    public class CompanyDto
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
        public string OfficeLocation { get; set; }
        public ICollection<JobDto> Jobs { get; set; }
    }

    public class CreateCompanyDto
    {
        public string Name { get; set; }
        public string CompanyProfile { get; set; }
        public string Location { get; set; }
        public string Website { get; set; }
        public DateTime FoundedDate { get; set; }
        public int EmployeeCount { get; set; }
        public string Industry { get; set; }
        public string LogoUrl { get; set; }
        public string Contacts { get; set; }
        public string OfficeLocation { get; set; }
    }

    public class UpdateCompanyDto
    {
        public string Name { get; set; }
        public string CompanyProfile { get; set; }
        public string Location { get; set; }
        public string Website { get; set; }
        public DateTime? FoundedDate { get; set; }
        public int? EmployeeCount { get; set; }
        public string Industry { get; set; }
        public string LogoUrl { get; set; }
        public string Contacts { get; set; }
        public string OfficeLocation { get; set; }
    }
} 