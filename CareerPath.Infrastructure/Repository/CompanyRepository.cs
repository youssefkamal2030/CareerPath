using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CareerPath.Application.Interfaces;
using CareerPath.Domain.Entities;
using CareerPath.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CareerPath.Infrastructure.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;
        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }
   
        public async Task<IEnumerable<Company>> GetCompanies()
        {
            return await _context.Companies.ToListAsync();
        }

        public async Task<Company> GetCompany(string companyId)
        {
            return await _context.Companies.FirstOrDefaultAsync(e => e.Id == companyId);
        }
    }
}
