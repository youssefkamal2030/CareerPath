using CareerPath.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CareerPath.Application.Interfaces
{
    public interface IJobApplicationRepository
    {
        Task<JobApplication> GetByIdAsync(int id);
        Task<IEnumerable<JobApplication>> GetByUserIdAsync(string userId);
        Task<IEnumerable<JobApplication>> GetAllJobsAsync();
    }
} 