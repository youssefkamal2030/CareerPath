using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CareerPath.Application.Services;
using CareerPath.Contracts.External.JobBoard;
using CareerPath.Contracts.External.Jooble;
using CareerPath.Domain.Recommendations;
using CareerPath.Application.Interfaces;
using CareerPath.Infrastructure.Configuration;
using CareerPath.Infrastructure.Mappers;
namespace CareerPath.Infrastructure.Services
{
    public class JobBoardService : IJobBoardService
    {
        private readonly HttpClient _httpClient;
        private readonly IJobIngestionService _jobIngestionService;
        private readonly ILogger<JobBoardService> _logger;
        private readonly ExternalApiSettings _apiSettings;
        private readonly IJobMapperFactory _mapperFactory;
        private readonly IJobRepository _jobRepository;
        private readonly string _joobleBaseUrl;
        private readonly string _apiKey;
        private readonly bool _useRealApi;

        public JobBoardService(
            HttpClient httpClient, 
            IJobIngestionService jobIngestionService,
            ILogger<JobBoardService> logger,
            IOptions<ExternalApiSettings> apiSettings,
            IJobMapperFactory mapperFactory,
            IJobRepository jobRepository)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _jobIngestionService = jobIngestionService ?? throw new ArgumentNullException(nameof(jobIngestionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _apiSettings = apiSettings?.Value ?? throw new ArgumentNullException(nameof(apiSettings));
            _mapperFactory = mapperFactory ?? throw new ArgumentNullException(nameof(mapperFactory));
            _jobRepository = jobRepository ?? throw new ArgumentNullException(nameof(jobRepository));
            
            // Get API key from configuration
            _apiKey = _apiSettings.Jooble_API_Key;
            _joobleBaseUrl = "https://jooble.org/api/";

            // Validate API key on construction
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                _logger.LogWarning("Jooble API key is not configured. Service will use mock data.");
                _useRealApi = false;
            }
            else
            {
                _logger.LogInformation("JobBoardService initialized with Jooble API key from configuration");
                _useRealApi = true;
            }
        }

        public async Task<JobBoardProviderResponse> FetchJobsAsync(string searchQuery, string? location = null, int page = 1, int pageSize = 50)
        {
            try
            {
                _logger.LogInformation("Fetching jobs from Jooble API: Query={Query}, Location={Location}, Page={Page}", 
                    searchQuery, location, page);

                // Use real API if configured, otherwise fallback to mock data
                JoobleApiResponse? joobleResponse = _useRealApi 
                    ? await FetchJobsFromJoobleApiAsync(searchQuery, location, page)
                    : null;

                if (joobleResponse != null)
                {
                    // Successfully fetched from Jooble API
                    _logger.LogInformation("Successfully fetched {Count} jobs from Jooble API", joobleResponse.Jobs.Count);
                    
                    // NOTE: Jooble response is kept as-is for now
                    // Mapping to internal ExternalJobDto will be done by adapter/mapper later
                    return new JobBoardProviderResponse
                    {
                        Success = true,
                        Message = "Jobs fetched successfully from Jooble",
                        Jobs = new List<ExternalJobDto>(), // Empty for now - will be mapped by adapter
                        TotalCount = joobleResponse.TotalCount,
                        Provider = "Jooble",
                        FetchedAt = DateTime.UtcNow,
                        RawApiResponse = joobleResponse // Store raw response for mapper
                    };
                }
                else
                {
                    // Fallback to mock data
                    _logger.LogInformation("Using mock data (API key not configured)");
                    var mockJobs = GenerateMockJobs(searchQuery, location, pageSize);
                    
                    return new JobBoardProviderResponse
                    {
                        Success = true,
                        Message = "Jobs fetched successfully (mock data)",
                        Jobs = mockJobs,
                        TotalCount = mockJobs.Count,
                        Provider = "MockProvider",
                        FetchedAt = DateTime.UtcNow
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching jobs from Jooble API");
                return new JobBoardProviderResponse
                {
                    Success = false,
                    Message = ex.Message,
                    Jobs = new List<ExternalJobDto>(),
                    TotalCount = 0,
                    Provider = "Jooble",
                    FetchedAt = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Fetches jobs directly from Jooble API
        /// Returns raw JoobleApiResponse object for later mapping
        /// </summary>
        private async Task<JoobleApiResponse?> FetchJobsFromJoobleApiAsync(string keywords, string? location, int page)
        {
            try
            {
                // Build the Jooble API URL
                var apiUrl = $"{_joobleBaseUrl}{_apiKey}";

                // Create request payload
                var requestBody = new JoobleSearchRequest
                {
                    Keywords = keywords,
                    Location = location ?? string.Empty,
                    Page = page
                };

                // Serialize request to JSON
                var jsonContent = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                _logger.LogDebug("Jooble API Request: {Url}, Body: {Body}", apiUrl, jsonContent);

                // Create HTTP request
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, apiUrl)
                {
                    Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
                };
                
                // Add headers to help with API compatibility
                httpRequest.Headers.Add("User-Agent", "CareerPathApp/1.0");
                httpRequest.Headers.Add("Accept", "application/json");

                // Send request
                var httpResponse = await _httpClient.SendAsync(httpRequest);

                // Check for success
                if (!httpResponse.IsSuccessStatusCode)
                {
                    var errorContent = await httpResponse.Content.ReadAsStringAsync();
                    _logger.LogError("Jooble API returned error: {StatusCode}, {Content}", 
                        httpResponse.StatusCode, errorContent);
                    return null;
                }

                // Read and deserialize response
                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                _logger.LogDebug("Jooble API Response: {Content}", responseContent);

                var joobleResponse = JsonSerializer.Deserialize<JoobleApiResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });

                return joobleResponse;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error calling Jooble API");
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing Jooble API response");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calling Jooble API");
                return null;
            }
        }

        public async Task<List<Job>> FetchAndIngestJobsAsync(string searchQuery, string? location = null)
        {
            _logger.LogInformation("Starting job fetch and ingest for query: {Query}, location: {Location}", 
                searchQuery, location);

            var ingestedJobs = new List<Job>();

            try
            {
                // Step 1: Fetch jobs from Jooble API
                var joobleResponse = await FetchJobsFromJoobleApiAsync(searchQuery, location, 1);

                if (joobleResponse == null || !joobleResponse.Jobs.Any())
                {
                    _logger.LogWarning("No jobs returned from Jooble API");
                    return ingestedJobs;
                }

                _logger.LogInformation("Fetched {Count} jobs from Jooble API. Starting mapping and ingestion...", 
                    joobleResponse.Jobs.Count);

                // Step 2: Get the Jooble mapper
                var mapper = _mapperFactory.GetMapper<JoobleJob>("Jooble");

                // Step 3: Map Jooble jobs to our domain Job entities
                var mappedJobs = mapper.MapToJobs(joobleResponse.Jobs).ToList();
                _logger.LogInformation("Mapped {Count} jobs successfully", mappedJobs.Count);

                // Step 4: Save to repository (with duplicate check)
                foreach (var job in mappedJobs)
                {
                    try
                    {
                        // Check if job already exists by ExternalId
                        var existingJob = await _jobRepository.GetByExternalIdAsync(job.ExternalId, job.SourceProvider);
                        
                        if (existingJob != null)
                        {
                            _logger.LogDebug("Job {ExternalId} already exists, skipping", job.ExternalId);
                            continue;
                        }

                        // Add new job
                        await _jobRepository.AddAsync(job);
                        ingestedJobs.Add(job);

                        _logger.LogDebug("Added job {JobId} - {Title} to repository", 
                            job.JobId, job.JobTitle);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to save job {ExternalId}: {Title}", 
                            job.ExternalId, job.JobTitle);
                    }
                }

                _logger.LogInformation("Successfully ingested {Count} new jobs out of {Total} fetched", 
                    ingestedJobs.Count, joobleResponse.Jobs.Count);

                return ingestedJobs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during fetch and ingest process");
                throw;
            }
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