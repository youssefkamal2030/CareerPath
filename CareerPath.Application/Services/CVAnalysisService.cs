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
using CareerPath.Domain.Entities.AIDataAnalysis;
using Microsoft.AspNetCore.Http;
namespace CareerPath.Application.Services
{
    public class CVAnalysisService : ICVAnalysisService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CVAnalysisService> _logger;
        private readonly AITeamApiSettings _aiTeamApiSettings;

        public CVAnalysisService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CVAnalysisService> logger,
            IOptions<ExternalServicesSettings> externalServicesSettings,
            HttpClient httpClient)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _aiTeamApiSettings = externalServicesSettings.Value.AITeamApi;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        //this method Retrives the data extracted by the AI team for the front-end
        public async Task<CVAnalysisDto?> GetCVAnalysisByUserIdAsync(string userId)
        {
            return await _unitOfWork.CVAnalysis.GetCVAnalysisByUserIdAsync(userId);
        }

        public async Task<bool> SaveCVAnalysisAsync(string userId, CVAnalysisDto cvAnalysis)
        {
            return await _unitOfWork.CVAnalysis.SaveCVAnalysisAsync(userId, cvAnalysis);
        }

        public async Task<JobRecommendationResponseDto> RecommendJobsAsync(string id)
        {
            try
            {
                var userData = await _unitOfWork.CVAnalysis.GetUserDataForRecommendationAsync(id);
                if (userData == null)
                {
                    _logger.LogError("No user data found for user ID: {UserId}", id);
                    throw new Exception($"No user data found for user ID: {id}");
                }

                _logger.LogInformation("User data for recommendation: Skills: {SkillsCount}, Work Experiences: {WorkExpCount}, Projects: {ProjectsCount}",
                    userData.Skills?.Count ?? 0,
                    userData.WorkExperiences?.Count ?? 0,
                    userData.Projects?.Count ?? 0);

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
                var userData = await _unitOfWork.CVAnalysis.RecommnderSystem(userId);
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
      public async Task SaveCvFile(IFormFile Cv, string userId)
        {
            try
            {
                var userCv = await _unitOfWork.CVAnalysis.SaveUserCV(Cv, userId);
                _logger.LogInformation("Successfully received Cv File for user {UserId}: Cv file --> {Cv}", userId, Cv);
                if (userCv == null)
                {
                    _logger.LogError("Failed to save CV for user {UserId}", userId);
                    throw new Exception($"Failed to save CV for user ID: {userId}");
                }
                else
                {
                    _logger.LogInformation("CV saved successfully for user {UserId}", userId);
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Failed to save the Cv File ");
              
            }
        }
    }
}