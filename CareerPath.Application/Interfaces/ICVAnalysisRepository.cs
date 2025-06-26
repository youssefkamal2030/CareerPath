using CareerPath.Contracts.Dto;
using CareerPath.Domain.Entities.AIDataAnalysis;
using Microsoft.AspNetCore.Http;
namespace CareerPath.Application.Interfaces
{
    public interface ICVAnalysisRepository
    {
        Task<CVAnalysisDto?> GetCVAnalysisByUserIdAsync(string userId);
        Task<bool> SaveCVAnalysisAsync(string userId, CVAnalysisDto cvAnalysis);
        Task<JobRecommendationRequestDto> GetUserDataForRecommendationAsync(string userId);
        Task<RecommnderSystemDto> RecommnderSystem(string userId);
        Task<(byte[] FileData, string FileName, string ContentType)?> GetUserCVAsync(string userId);
        Task<UserCV> SaveUserCV(IFormFile UserCv, string userId);
    }
} 