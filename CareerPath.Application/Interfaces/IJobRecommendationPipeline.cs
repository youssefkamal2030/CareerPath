using CareerPath.Contracts.Dto.CVAnalysis;
using CareerPath.Contracts.Dto.JobRecommendation;

namespace CareerPath.Application.Interfaces;

/// <summary>
/// Orchestrates the complete RAG pipeline for job recommendations
/// </summary>
public interface IJobRecommendationPipeline
{
    /// <summary>
    /// Executes the complete recommendation pipeline from CV upload to ranked job matches
    /// </summary>
    Task<JobRecommendationResponse> GenerateRecommendationsAsync(CVAnalysisRequest cvRequest, JobRecommendationRequest? options = null);

    /// <summary>
    /// Generates recommendations from previously analyzed CV data
    /// </summary>
    Task<JobRecommendationResponse> GenerateRecommendationsFromAnalysisAsync(string analysisId, JobRecommendationRequest options);

    /// <summary>
    /// Refreshes recommendations with updated options
    /// </summary>
    Task<JobRecommendationResponse> RefreshRecommendationsAsync(string analysisId, JobRecommendationRequest options);
}
