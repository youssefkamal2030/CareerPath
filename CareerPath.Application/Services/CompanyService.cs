using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CareerPath.Application.Interfaces;
using CareerPath.Contracts.Dto;
using AutoMapper;
namespace CareerPath.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;
        public CompanyService(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<CompanyDto>> GetCompanies()
        {
            var companies = await _companyRepository.GetCompanies();
            return _mapper.Map<IEnumerable<CompanyDto>>(companies);
        }
        public async Task<CompanyDto> GetCompany(string companyId)
        {
            var company = await _companyRepository.GetCompany(companyId);
            return _mapper.Map<CompanyDto>(company);
        }
    }


}
