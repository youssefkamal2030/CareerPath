using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CareerPath.Contracts.Dto;
using CareerPath.Domain.Entities;

namespace CareerPath.Application.Interfaces
{
    public interface ICompanyService
    {
        Task<IEnumerable<CompanyDto>> GetCompanies();

        Task<CompanyDto> GetCompany(string companyId);
        
    }
}
