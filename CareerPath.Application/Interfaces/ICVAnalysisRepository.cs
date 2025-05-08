using CareerPath.Contracts.Dto;

namespace CareerPath.Application.Interfaces
{
    public interface ICVAnalysisRepository
    {
        Task<CVAnalysisDto?> GetCVAnalysisByUserIdAsync(string userId);
        Task<bool> SaveCVAnalysisAsync(string userId, CVAnalysisDto cvAnalysis);
    }
} 