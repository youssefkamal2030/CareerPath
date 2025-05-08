using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CareerPath.Contracts.Dto;

namespace CareerPath.Application.Interfaces
{
    public interface ICVAnalysisService
    {
        Task<CVAnalysisDto?> GetCVAnalysisByUserIdAsync(string userId);
        Task<bool> SaveCVAnalysisAsync(string userId, CVAnalysisDto cvAnalysis);
        Task<RecommendJobsResponseDto> RecommendJobsAsync(RecommendJobsRequestDto requestDto);
    }
}
