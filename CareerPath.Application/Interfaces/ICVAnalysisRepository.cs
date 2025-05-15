using CareerPath.Contracts.Dto;
using CareerPath.Domain.Entities.AIDataAnalysis;
namespace CareerPath.Application.Interfaces
{
    public interface ICVAnalysisRepository
    {
        Task<CVAnalysisDto?> GetCVAnalysisByUserIdAsync(string userId);
        Task<bool> SaveCVAnalysisAsync(string userId, CVAnalysisDto cvAnalysis);
        Task<JobRecommendationRequestDto> GetUserDataForRecommendationAsync(string userId);
        Task<RecommnderSystemDto> RecommnderSystem(string userId);
            
     }
} 