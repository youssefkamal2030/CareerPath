using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareerPath.Application.Interfaces;
using CareerPath.Contracts.Dto.CVAnalysis;
using CareerPath.Contracts.Dto.JobRecommendation;

namespace CareerPath.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JobRecommendationsController : ControllerBase
{
    private readonly IJobRecommendationPipeline _recommendationPipeline;
    private readonly ICVProcessingService _cvProcessingService;
    private readonly ILogger<JobRecommendationsController> _logger;

    public JobRecommendationsController(
        IJobRecommendationPipeline recommendationPipeline,
        ICVProcessingService cvProcessingService,
        ILogger<JobRecommendationsController> logger)
    {
        _recommendationPipeline = recommendationPipeline;
        _cvProcessingService = cvProcessingService;
        _logger = logger;
    }

    [HttpPost("upload-cv")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(JobRecommendationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadCVAndGetRecommendations(
        IFormFile cvFile,
        [FromForm] int maxResults = 20,
        [FromForm] decimal minScore = 50)
    {
        try
        {
            if (cvFile == null || cvFile.Length == 0)
                return BadRequest(new { message = "CV file is required" });

            var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
            var fileExtension = Path.GetExtension(cvFile.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest(new { message = "Invalid file type. Allowed: PDF, DOC, DOCX" });

            if (cvFile.Length > 10 * 1024 * 1024)
                return BadRequest(new { message = "File size must not exceed 10MB" });

            var userId = User.FindFirst("uid")?.Value 
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated" });

            var cvRequest = new CVAnalysisRequest
            {
                CVFile = cvFile,
                UserId = userId
            };

            var options = new JobRecommendationRequest
            {
                MaxResults = Math.Min(maxResults, 100),
                MinimumMatchScore = Math.Clamp(minScore, 0, 100),
                IncludeExplanations = true
            };

            _logger.LogInformation("Generating job recommendations for user {UserId}", userId);

            var result = await _recommendationPipeline.GenerateRecommendationsAsync(cvRequest, options);

            if (result.Metadata.ContainsKey("Error"))
            {
                return BadRequest(new { message = "Failed to generate recommendations", error = result.Metadata["Error"] });
            }

            _logger.LogInformation("Generated {Count} recommendations in {Time}ms", 
                result.RecommendationCount, result.ProcessingTimeMs);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating recommendations from CV upload");
            return StatusCode(500, new { message = "An error occurred while processing your request" });
        }
    }

    [HttpGet("by-analysis/{analysisId}")]
    [ProducesResponseType(typeof(JobRecommendationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRecommendationsByAnalysis(
        string analysisId,
        [FromQuery] int maxResults = 20,
        [FromQuery] decimal minScore = 50)
    {
        try
        {
            var userId = User.FindFirst("uid")?.Value 
                ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not authenticated" });

            var options = new JobRecommendationRequest
            {
                MaxResults = Math.Min(maxResults, 100),
                MinimumMatchScore = Math.Clamp(minScore, 0, 100),
                IncludeExplanations = true
            };

            var result = await _recommendationPipeline.GenerateRecommendationsFromAnalysisAsync(analysisId, options);

            if (result.Metadata.ContainsKey("Error"))
            {
                var error = result.Metadata["Error"].ToString();
                if (error?.Contains("not found") == true)
                    return NotFound(new { message = "CV analysis not found" });

                return BadRequest(new { message = error });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recommendations for analysis {AnalysisId}", analysisId);
            return StatusCode(500, new { message = "An error occurred while processing your request" });
        }
    }

    [HttpGet("cv-data/{analysisId}")]
    [ProducesResponseType(typeof(ExtractedCVDataDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetExtractedCVData(string analysisId)
    {
        try
        {
            var cvData = await _cvProcessingService.GetExtractedCVDataAsync(analysisId);

            if (cvData == null)
                return NotFound(new { message = "CV analysis not found" });

            return Ok(cvData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving CV data for analysis {AnalysisId}", analysisId);
            return StatusCode(500, new { message = "An error occurred while processing your request" });
        }
    }
}
