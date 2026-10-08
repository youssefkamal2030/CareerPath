using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CareerPath.Application.Services;
using CareerPath.Contracts.External.JobBoard;
using CareerPath.Domain.Recommendations;

namespace CareerPath.Infrastructure.Services
{
    public interface IJobBoardService
    {
        Task<JobBoardProviderResponse> FetchJobsAsync(string searchQuery, string? location = null, int page = 1, int pageSize = 50);
        Task<List<Job>> FetchAndIngestJobsAsync(string searchQuery, string? location = null);
        Task<bool> TestProviderConnection();
    }

    public class JobBoardService : IJobBoardService
    {
        private readonly HttpClient _httpClient;
        private readonly IJobIngestionService _jobIngestionService;
        private readonly ILogger<JobBoardService> _logger;
        private readonly string _baseUrl;
        private readonly string _apiKey;

        public JobBoardService(
            HttpClient httpClient, 
            IJobIngestionService jobIngestionService,
            ILogger<JobBoardService> logger,
            string baseUrl = "https://api.adzuna.com/v1/api/jobs",
            string apiKey = "")
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _jobIngestionService = jobIngestionService ?? throw new ArgumentNullException(nameof(jobIngestionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _baseUrl = baseUrl;
            _apiKey = apiKey;
        }

        public async Task<JobBoardProviderResponse> FetchJobsAsync(string searchQuery, string? location = null, int page = 1, int pageSize = 50)
        {
            try
            {
                _logger.LogInformation("Fetching jobs from external provider: Query={Query}, Location={Location}, Page={Page}", 
                    searchQuery, location, page);

                // This is a mock implementation - replace with actual API calls
                var mockJobs = GenerateMockJobs(searchQuery, location, pageSize);
                
                return new JobBoardProviderResponse
                {
                    Success = true,
                    Message = "Jobs fetched successfully",
                    Jobs = mockJobs,
                    TotalCount = mockJobs.Count,
                    Provider = "MockProvider",
                    FetchedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching jobs from external provider");
                return new JobBoardProviderResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Jobs = new List<ExternalJobDto>(),
                    TotalCount = 0,
                    Provider = "MockProvider",
                    FetchedAt = DateTime.UtcNow
                };
            }
        }

        public async Task<List<Job>> FetchAndIngestJobsAsync(string searchQuery, string? location = null)
        {
            var response = await FetchJobsAsync(searchQuery, location);
            
            if (!response.Success)
            {
                _logger.LogWarning("Failed to fetch jobs: {Message}", response.Message);
                return new List<Job>();
            }

            var ingestedJobs = new List<Job>();

            foreach (var externalJob in response.Jobs)
            {
                try
                {
                    // Check if already exists to avoid duplicates
                    if (await _jobIngestionService.JobAlreadyExists(externalJob.ExternalId, externalJob.SourceProvider))
                    {
                        _logger.LogDebug("Job {ExternalId} from {Provider} already exists, skipping", 
                            externalJob.ExternalId, externalJob.SourceProvider);
                        continue;
                    }

                    var job = await _jobIngestionService.IngestJobFromExternalSource(externalJob);
                    ingestedJobs.Add(job);
                    
                    _logger.LogDebug("Successfully ingested job {JobId} from external source {ExternalId}", 
                        job.JobId, job.ExternalId);
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogWarning("Skipped job {ExternalId}: {Message}", externalJob.ExternalId, ex.Message);
                    continue;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to ingest job {ExternalId} from {Provider}", 
                        externalJob.ExternalId, externalJob.SourceProvider);
                    continue;
                }
            }

            _logger.LogInformation("Ingested {Count} jobs out of {Total} fetched", 
                ingestedJobs.Count, response.Jobs.Count);

            return ingestedJobs;
        }

        public async Task<bool> TestProviderConnection()
        {
            try
            {
                // Simple connectivity test
                var response = await FetchJobsAsync("test", null, 1, 1);
                return response.Success;
            }
            catch
            {
                return false;
            }
        }

        private List<ExternalJobDto> GenerateMockJobs(string searchQuery, string? location, int count)
        {
            var random = new Random();
            var companies = new[] { "TechCorp", "InnovateInc", "DevSolutions", "CloudTech", "StartupXYZ" };
            var locations = new[] { "New York, NY", "San Francisco, CA", "Austin, TX", "Seattle, WA", "Chicago, IL" };
            var experienceLevels = new[] { "Entry Level", "Mid Level", "Senior Level", "Lead" };
            var employmentTypes = new[] { "Full-time", "Part-time", "Contract", "Remote" };

            var mockJobs = new List<ExternalJobDto>();

            for (int i = 0; i < count; i++)
            {
                var job = new ExternalJobDto
                {
                    ExternalId = $"ext_job_{Guid.NewGuid().ToString("N")[..8]}",
                    Title = $"{searchQuery} Developer {i + 1}",
                    Company = companies[random.Next(companies.Length)],
                    Description = $"We are looking for a skilled {searchQuery} developer to join our team...",
                    Location = location ?? locations[random.Next(locations.Length)],
                    SalaryRange = $"${random.Next(60, 150)}K - ${random.Next(150, 200)}K",
                    RequiredSkills = $"{searchQuery}, JavaScript, SQL, Git",
                    ExperienceLevel = experienceLevels[random.Next(experienceLevels.Length)],
                    EducationLevel = "Bachelor's Degree",
                    EmploymentType = employmentTypes[random.Next(employmentTypes.Length)],
                    PostedDate = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                    ApplicationDeadline = DateTime.UtcNow.AddDays(random.Next(7, 60)),
                    SourceProvider = "MockProvider",
                    ExternalUrl = $"https://mockjobs.com/job/{i + 1}",
                    Industry = "Technology",
                    CertificationsRequired = "",
                    RequiredLanguage = "English"
                };

                mockJobs.Add(job);
            }

            return mockJobs;
        }
    }
}