using CareerPath.Contracts.Dto.CVAnalysis;

namespace CareerPath.Application.Interfaces;

/// <summary>
/// Service for processing CVs and extracting structured data
/// </summary>
public interface ICVProcessingService
{
    /// <summary>
    /// Analyzes CV and extracts structured data for job matching
    /// </summary>
    Task<CVAnalysisResponse> AnalyzeCVAsync(CVAnalysisRequest request);

    /// <summary>
    /// Retrieves previously extracted CV data by analysis ID
    /// </summary>
    Task<ExtractedCVDataDto?> GetExtractedCVDataAsync(string analysisId);
}
