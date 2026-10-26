namespace CareerPath.Contracts.Dto.CVAnalysis;

/// <summary>
/// Response from CV analysis with extracted data
/// </summary>
public class CVAnalysisResponse
{
    public string AnalysisId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public ExtractedCVDataDto ExtractedData { get; set; } = null!;
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
