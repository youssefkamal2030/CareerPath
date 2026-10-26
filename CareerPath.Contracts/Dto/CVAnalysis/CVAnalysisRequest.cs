using Microsoft.AspNetCore.Http;

namespace CareerPath.Contracts.Dto.CVAnalysis;

/// <summary>
/// Request for CV analysis and data extraction
/// </summary>
public class CVAnalysisRequest
{
    public IFormFile CVFile { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
}
