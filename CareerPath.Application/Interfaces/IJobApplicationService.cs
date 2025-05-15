using CareerPath.Contracts.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CareerPath.Application.Interfaces
{
    public interface IJobApplicationService
    {
        Task<JobApplicationDto> GetByIdAsync(int id);
        Task<IEnumerable<JobApplicationDto>> GetByUserIdAsync(string userId);
        Task<IEnumerable<JobApplicationDto>> GetJobsAsync();
     
    }
} 