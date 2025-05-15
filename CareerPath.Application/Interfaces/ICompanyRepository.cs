using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CareerPath.Domain.Entities;

namespace CareerPath.Application.Interfaces
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<Company>> GetCompanies();
        Task<Company> GetCompany(string companyId);
    }
}
