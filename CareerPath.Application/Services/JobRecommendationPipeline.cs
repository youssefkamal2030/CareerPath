using System.Diagnostics;
using CareerPath.Application.Interfaces;
using CareerPath.Contracts.Dto.CVAnalysis;
using CareerPath.Contracts.Dto.JobRecommendation;
using CareerPath.Domain.Recommendations;

namespace CareerPath.Application.Services;

/// <summary>
/// Orchestrates the complete RAG pipeline for job recommendations
/// </summary>
public class JobRecommendationPipeline : IJobRecommendationPipeline
{
    private readonly ICVProcessingService _cvProcessingService;
    private readonly IJobMatchingService _jobMatchingService;

    public JobRecommendationPipeline(
        ICVProcessingService cvProcessingService,
        IJobMatchingService jobMatchingService)
    {
        _cvProcessingService = cvProcessingService;
        _jobMatchingService = jobMatchingService;
    }

    public async Task<JobRecommendationResponse> GenerateRecommendationsAsync(
        CVAnalysisRequest cvRequest,
        JobRecommendationRequest? options = null)
    {
        options ??= new JobRecommendationRequest();
        var stopwatch = Stopwatch.StartNew();
        var result = new JobRecommendationResponse { UserId = cvRequest.UserId };

        try
        {
            var cvAnalysis = await _cvProcessingService.AnalyzeCVAsync(cvRequest);

            if (!cvAnalysis.Success || cvAnalysis.ExtractedData == null)
            {
                result.Metadata["Error"] = cvAnalysis.ErrorMessage ?? "Failed to analyze CV";
                stopwatch.Stop();
                result.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
                return result;
            }

            result.CVAnalysisId = cvAnalysis.AnalysisId;

            var criteria = BuildMatchingCriteria(options, cvAnalysis.ExtractedData);
            var matchedJobs = await _jobMatchingService.MatchJobsAsync(cvAnalysis.ExtractedData, criteria);

            result.TotalJobsEvaluated = matchedJobs.Count;
            result.MatchedJobs = matchedJobs.Take(options.MaxResults).ToList();
            result.RecommendationCount = result.MatchedJobs.Count;

            result.Metadata["AverageMatchScore"] = result.MatchedJobs.Any() 
                ? result.MatchedJobs.Average(j => j.MatchScore.OverallScore) 
                : 0;
            result.Metadata["TopMatchScore"] = result.MatchedJobs.Any() 
                ? result.MatchedJobs.Max(j => j.MatchScore.OverallScore) 
                : 0;

            stopwatch.Stop();
            result.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

            return result;
        }
        catch (Exception ex)
        {
            result.Metadata["Error"] = ex.Message;
            stopwatch.Stop();
            result.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
            return result;
        }
    }

    public async Task<JobRecommendationResponse> GenerateRecommendationsFromAnalysisAsync(
        string analysisId,
        JobRecommendationRequest options)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new JobRecommendationResponse { CVAnalysisId = analysisId };

        try
        {
            var extractedData = await _cvProcessingService.GetExtractedCVDataAsync(analysisId);

            if (extractedData == null)
            {
                result.Metadata["Error"] = "CV analysis not found";
                stopwatch.Stop();
                result.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
                return result;
            }

            var criteria = BuildMatchingCriteria(options, extractedData);
            var matchedJobs = await _jobMatchingService.MatchJobsAsync(extractedData, criteria);

            result.TotalJobsEvaluated = matchedJobs.Count;
            result.MatchedJobs = matchedJobs.Take(options.MaxResults).ToList();
            result.RecommendationCount = result.MatchedJobs.Count;

            result.Metadata["AverageMatchScore"] = result.MatchedJobs.Any() 
                ? result.MatchedJobs.Average(j => j.MatchScore.OverallScore) 
                : 0;

            stopwatch.Stop();
            result.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

            return result;
        }
        catch (Exception ex)
        {
            result.Metadata["Error"] = ex.Message;
            stopwatch.Stop();
            result.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
            return result;
        }
    }

    public async Task<JobRecommendationResponse> RefreshRecommendationsAsync(
        string analysisId,
        JobRecommendationRequest options)
    {
        return await GenerateRecommendationsFromAnalysisAsync(analysisId, options);
    }

    private JobMatchCriteria BuildMatchingCriteria(JobRecommendationRequest options, ExtractedCVDataDto cvData)
    {
        return new JobMatchCriteria
        {
            MaxResults = options.MaxResults,
            MinimumScore = options.MinimumMatchScore,
            PreferredLocations = options.PreferredLocations.Any() 
                ? options.PreferredLocations 
                : new List<string> { cvData.Location ?? string.Empty },
            PreferredSources = options.PreferredSources,
            SkillsWeight = 0.4m,
            ExperienceWeight = 0.3m,
            EducationWeight = 0.2m,
            LocationWeight = 0.1m,
            IncludeRemoteJobs = true,
            MaxJobAgeDays = 30
        };
    }
}
