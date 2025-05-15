using CareerPath.Application.Interfaces;
using CareerPath.Contracts.Dto;
using AutoMapper;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using CareerPath.Application.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
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
            IOptions<ExternalServicesSettings> externalServicesSettings,
            HttpClient httpClient)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
            _aiTeamApiSettings = externalServicesSettings.Value.AITeamApi;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<CVAnalysisDto?> GetCVAnalysisByUserIdAsync(string userId)
        {
            return await _repository.GetCVAnalysisByUserIdAsync(userId);
        }

        public async Task<bool> SaveCVAnalysisAsync(string userId, CVAnalysisDto cvAnalysis)
        {
            return await _repository.SaveCVAnalysisAsync(userId, cvAnalysis);
        }

        public async Task<JobRecommendationResponseDto> RecommendJobsAsync(string id)
        {
            try
            {
                var userData = await _repository.GetUserDataForRecommendationAsync(id);
                if (userData == null)
                {
                    _logger.LogError("No user data found for user ID: {UserId}", id);
                    throw new Exception($"No user data found for user ID: {id}");
                }

                _logger.LogInformation("User data for recommendation: Skills: {SkillsCount}, Work Experiences: {WorkExpCount}, Projects: {ProjectsCount}, Job Descriptions: {JobDescCount}",
                    userData.Skills?.Count ?? 0,
                    userData.WorkExperiences?.Count ?? 0,
                    userData.Projects?.Count ?? 0,
                    userData.JobDescriptions?.Count ?? 0);

                var request = new HttpRequestMessage(HttpMethod.Post, "https://ocelot-delicate-logically.ngrok-free.app/recommend");
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                
                _logger.LogInformation("Sending job recommendation request to {Url} with payload: {Payload}",
                    request.RequestUri,
                    JsonSerializer.Serialize(userData, jsonOptions));

                request.Content = new StringContent(JsonSerializer.Serialize(userData, jsonOptions), Encoding.UTF8, "application/json");
                var response = await _httpClient.SendAsync(request);
                var result = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var recommendations = JsonSerializer.Deserialize<JobRecommendationResponseDto>(result, jsonOptions);
                    _logger.LogInformation("Received {RecommendationCount} job recommendations for user {UserId}",
                        recommendations?.Recommendations?.Count ?? 0, id);
                    return recommendations;
                }
                else
                {
                    _logger.LogError("Failed to get job recommendations. Status code: {StatusCode}, Response: {Response}", response.StatusCode, result);
                    throw new Exception($"Failed to get job recommendations. Status code: {response.StatusCode}, Response: {result}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while getting job recommendations for user {UserId}", id);
                throw;
            }
        }

        public async Task<SkillRecommendationResponseDto> RecommnderSystem(string userId)
        {
            try
            {
                var userData = await _repository.RecommnderSystem(userId);
                if (userData == null)
                {
                    _logger.LogError("No user data found for user ID: {UserId}", userId);
                    throw new Exception($"No user data found for user ID: {userId}");
                }

                _logger.LogInformation("Retrieved skills data for user {UserId}: Skills: {Skills}, Job Descriptions: {JobCount}",
                    userId, userData.UserSkills, userData.JobDescriptions?.Count ?? 0);

                var request = new HttpRequestMessage(HttpMethod.Post, "https://ocelot-delicate-logically.ngrok-free.app/recomendersystem");
                request.Headers.Accept.Clear();
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                _logger.LogInformation("Sending recommendation request to {Url} with payload: {Payload}",
                    request.RequestUri,
                    JsonSerializer.Serialize(userData, jsonOptions));

                request.Content = new StringContent(JsonSerializer.Serialize(userData, jsonOptions), Encoding.UTF8, "application/json");
                var response = await _httpClient.SendAsync(request);
                var resultJson = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    _logger.LogInformation("Successfully received recommendation response for user {UserId}", userId);
                    var recommendationResponse = JsonSerializer.Deserialize<SkillRecommendationResponseDto>(resultJson, jsonOptions);
                    _logger.LogInformation("Deserialized {Count} job recommendations", recommendationResponse?.Recommendations?.Count ?? 0);
                    return recommendationResponse;
                }
                else
                {
                    _logger.LogError("Failed to process recommendation. Status code: {StatusCode}, Response: {Response}", 
                        response.StatusCode, resultJson);
                    throw new Exception($"Failed to process recommendation. Status code: {response.StatusCode}, Response: {resultJson}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while processing recommendation for user {UserId}", userId);
                throw;
            }
        }
    }
}