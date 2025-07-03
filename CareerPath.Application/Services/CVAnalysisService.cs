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
     public async Task<UserCV> SaveCvFile(IFormFile cv, string userId)
        {
            try
            {
                _logger.LogInformation("Starting CV upload process for user {UserId}. File: {FileName}, Size: {FileSize} bytes",
                    userId, cv?.FileName, cv?.Length);

                var userCv = await _unitOfWork.CVAnalysis.SaveUserCV(cv, userId);

                _logger.LogInformation("CV uploaded successfully for user {UserId}. CV ID: {CvId}",
                    userId, userCv.Id);

                return userCv; 
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid CV file upload attempt for user {UserId}: {Error}", userId, ex.Message);
                throw; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while saving CV for user {UserId}", userId);
                throw new Exception($"Failed to save CV file for user {userId}: {ex.Message}", ex);
            }
        }

        public async Task<(byte[] FileData, string FileName, string ContentType)?> GetUserCVAsync(string userId)
        {
            return await _unitOfWork.CVAnalysis.GetUserCVAsync(userId);
        }

        public async Task<CVAnalysisDto> ExtractCVDataAsync(string userId)
        {
            // 1. Retrieve the user's CV
            var cvResult = await _unitOfWork.CVAnalysis.GetUserCVAsync(userId);
            if (cvResult == null)
                throw new Exception("No CV found for this user.");

            var (fileData, fileName, contentType) = cvResult.Value;

            // 2. Prepare the multipart/form-data request with the correct field name and content type
            using var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(fileData);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
            content.Add(fileContent, "resumefile", fileName ?? "cv.pdf");

            // 3. Send to external /extract endpoint
            var extractUrl = "https://ocelot-delicate-logically.ngrok-free.app/extract"; // TODO: move to config
            var response = await _httpClient.PostAsync(extractUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Extraction failed: {response.StatusCode} - {responseString}");

            // 4. Deserialize and map the response
            var extractResponse = JsonSerializer.Deserialize<ExternalExtractResponse>(responseString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
            if (extractResponse == null || extractResponse.Data == null)
                throw new Exception("Invalid extraction response from external service.");

            // 5. Map extractResponse.Data to CVAnalysisDto
            var cvAnalysis = MapExtractedDataToCVAnalysisDto(extractResponse.Data);
            return cvAnalysis;
        }

        private CVAnalysisDto MapExtractedDataToCVAnalysisDto(ExtractedData data)
        {
            return new CVAnalysisDto
            {
                PersonalInformation = new PersonalInformationDto
                {
                    Name = data.PersonalInformation?.Name,
                    Email = data.PersonalInformation?.Email,
                    Phone = data.PersonalInformation?.Phone,
                    Address = data.PersonalInformation?.Address
                },
                Skills = data.Skills?.Select(s => new SkillDto
                {
                    SkillName = s.SkillName,
                    ProficiencyLevel = s.ProficiencyLevel
                }).ToList() ?? new List<SkillDto>(),
                WorkExperiences = data.WorkExperiences?.Select(w => new WorkExperienceDto
                {
                    JobTitle = w.JobTitle,
                    JobLevel = w.JobLevel,
                    Company = w.Company,
                    StartYear = w.StartYear,
                    StartMonth = w.StartMonth,
                    EndYear = w.EndYear,
                    EndMonth = w.EndMonth,
                    JobDescription = w.JobDescription
                }).ToList() ?? new List<WorkExperienceDto>(),
                Educations = data.Educations?.Select(e => new EducationDto
                {
                    Institution = e.Institution,
                    Degree = e.Degree,
                    FieldOfStudy = e.FieldOfStudy,
                    StartYear = e.StartYear,
                    StartMonth = e.StartMonth,
                    EndYear = e.EndYear,
                    EndMonth = e.EndMonth,
                    EducationLevel = e.EducationLevel
                }).ToList() ?? new List<EducationDto>(),
                Projects = data.Projects?.Select(p => new ProjectDto
                {
                    ProjectName = p.ProjectName,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Url = p.Url,
                    Description = p.Description
                }).ToList() ?? new List<ProjectDto>()
            };
        }

        private class ExternalExtractResponse
        {
            public string Status { get; set; }
            public ExtractedData Data { get; set; }
        }
        private class ExtractedData
        {
            public PersonalInformationDto PersonalInformation { get; set; }
            public List<SkillDto> Skills { get; set; }
            public List<WorkExperienceDto> WorkExperiences { get; set; }
            public List<EducationDto> Educations { get; set; }
            public List<ProjectDto> Projects { get; set; }
        }
    }
}