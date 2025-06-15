using System.Collections.Generic;
using System.Threading.Tasks;
using CareerPath.Domain.Entities.AIDataAnalysis;

namespace CareerPath.Infrastructure.Repository
{
    public interface IJobRepository
    {
      
        Task<Job> CreateJobAsync(Job job);

       
        Task<Job> UpdateJobAsync(Job job);

        
        Task<bool> DeleteJobAsync(string id);

       
        Task<Job> GetByIdAsync(string id);

      
        Task<IEnumerable<Job>> GetAllAsync();

       
        Task<IEnumerable<Job>> GetByUserIdAsync(string userId);
    }
}