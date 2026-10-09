using System.Collections.Generic;
using System.Threading.Tasks;
using CareerPath.Contracts.External.JobBoard;
using CareerPath.Domain.Recommendations;

namespace CareerPath.Application.Interfaces
{
    public interface IJobBoardService
    {
        Task<JobBoardProviderResponse> FetchJobsAsync(string searchQuery, string? location = null, int page = 1, int pageSize = 50);
        Task<List<Job>> FetchAndIngestJobsAsync(string searchQuery, string? location = null);
        Task<bool> TestProviderConnection();
    }
}