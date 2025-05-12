using CareerPath.Application.Interfaces;
using CareerPath.Contracts.Dto;
using AutoMapper;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using CareerPath.Application.Configuration;
using Microsoft.Extensions.Logging;

namespace CareerPath.Application.Services
{
    public class CVAnalysisService : ICVAnalysisService
    {
        private readonly ICVAnalysisRepository _repository;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CVAnalysisService> _logger;
        private readonly AITeamApiSettings _aiTeamApiSettings;

        public CVAnalysisService(
            ICVAnalysisRepository repository,
            IMapper mapper,
            ILogger<CVAnalysisService> logger,
            IOptions<ExternalServicesSettings> externalServicesSettings)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _aiTeamApiSettings = externalServicesSettings.Value.AITeamApi;
        }

        public async Task<CVAnalysisDto?> GetCVAnalysisByUserIdAsync(string userId)
        {
            return await _repository.GetCVAnalysisByUserIdAsync(userId);
        }

        public async Task<bool> SaveCVAnalysisAsync(string userId, CVAnalysisDto cvAnalysis)
        {
            return await _repository.SaveCVAnalysisAsync(userId, cvAnalysis);
        }

        public async Task<RecommendJobsResponseDto> RecommendJobsAsync(RecommendJobsRequestDto requestDto)
        {
            try
            {
                // Use the endpoint from configuration
                var response = await _httpClient.PostAsJsonAsync(_aiTeamApiSettings.RecommendJobsEndpoint, requestDto);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<RecommendJobsResponseDto>();
                    return result ?? new RecommendJobsResponseDto 
                    { 
                        Jobs = new List<RecommendedJob>(),
                        Reason = "No recommendations found" 
                    };
                }

                _logger.LogError("Failed to get job recommendations. Status code: {StatusCode}", response.StatusCode);
                throw new ApplicationException($"Failed to get job recommendations. Status code: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting job recommendations");
                throw new ApplicationException("Error getting job recommendations", ex);
            }
        }
    }
} 