using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CareerPath.Domain.Entities;
using CareerPath.Domain.Entities.AIDataAnalysis;
namespace CareerPath.Application.AIDataAnalysis_Interfaces
{
    public interface IJobsRepository
    {
    
        Task<Job> GetByIdAsync(string id);
        Task<IEnumerable<Job>> GetAllAsync();
        Task<IEnumerable<Job>> GetByUserIdAsync(string userId);
        Task<Job> UpdateJobAsync(Job job);
        Task<bool> DeleteJobAsync(string id);
        Task<Job> CreateJobAsync(Job job);

    }
}
