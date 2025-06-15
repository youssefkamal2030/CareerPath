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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CompanyService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<CompanyDto>> GetCompanies()
        {
            var companies = await _unitOfWork.Companies.GetCompanies();
            return _mapper.Map<IEnumerable<CompanyDto>>(companies);
        }
        public async Task<CompanyDto> GetCompany(string companyId)
        {
            var company = await _unitOfWork.Companies.GetCompany(companyId);
            return _mapper.Map<CompanyDto>(company);
        }
    }


}
