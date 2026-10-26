using CareerPath.Contracts.Dto.CVAnalysis;
using CareerPath.Contracts.Dto.JobRecommendation;
using CareerPath.Domain.Recommendations;

namespace CareerPath.Application.Interfaces;

/// <summary>
/// Service for matching CVs with jobs and calculating match scores
/// </summary>
public interface IJobMatchingService
{
    /// <summary>
    /// Finds and scores jobs that match the extracted CV data
    /// </summary>
    Task<List<JobMatchDto>> MatchJobsAsync(ExtractedCVDataDto cvData, JobMatchCriteria criteria);

    /// <summary>
    /// Calculates detailed match score for a single job
    /// </summary>
    JobMatchScoreDto CalculateMatchScore(ExtractedCVDataDto cvData, Job job, JobMatchCriteria criteria);
}
