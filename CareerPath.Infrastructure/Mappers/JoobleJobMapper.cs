using System.Globalization;
using Microsoft.Extensions.Logging;
using CareerPath.Contracts.External.Jooble;
using CareerPath.Domain.Recommendations;

namespace CareerPath.Infrastructure.Mappers;

/// <summary>
/// Maps Jooble API job structure to our internal Job domain model
/// </summary>
public class JoobleJobMapper : IJobSourceMapper<JoobleJob>
{
    private readonly ILogger<JoobleJobMapper> _logger;

    public string ProviderName => "Jooble";

    public JoobleJobMapper(ILogger<JoobleJobMapper> logger)
    {
        _logger = logger;
    }

    public Job MapToJob(JoobleJob sourceJob)
    {
        if (!IsValid(sourceJob))
        {
            var errors = string.Join(", ", GetValidationErrors(sourceJob));
            throw new ArgumentException($"Invalid Jooble job data: {errors}");
        }

        try
        {
            // Parse the updated date
            var postedDate = ParseJoobleDate(sourceJob.Updated);

            // Extract employment type from Jooble's 'type' field
            var employmentType = ExtractEmploymentType(sourceJob.Type);

            // Extract experience level from title or snippet (Jooble doesn't provide this directly)
            var experienceLevel = ExtractExperienceLevel(sourceJob.Title, sourceJob.Snippet);

            // Jooble uses 'snippet' as description
            var description = CleanDescription(sourceJob.Snippet);

            // Create Job using factory method
            var job = Job.FromExternalSource(
                externalId: sourceJob.Id ?? GenerateExternalId(sourceJob),
                title: sourceJob.Title,
                company: sourceJob.Company,
                description: description,
                location: sourceJob.Location,
                salaryRange: sourceJob.Salary ?? string.Empty,
                requiredSkills: string.Empty, // Jooble doesn't provide this separately
                experienceLevel: experienceLevel,
                educationLevel: string.Empty, // Jooble doesn't provide this
                employmentType: employmentType,
                postedDate: postedDate,
                applicationDeadline: null, // Jooble doesn't provide deadline
                sourceProvider: ProviderName,
                externalUrl: sourceJob.Link,
                industry: string.Empty, // Could be extracted from job categories if available
                certificationsRequired: string.Empty,
                requiredLanguage: "English" // Default assumption
            );

            _logger.LogDebug("Mapped Jooble job {ExternalId} to internal Job {JobId}", 
                sourceJob.Id, job.JobId);

            return job;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error mapping Jooble job {JobId}: {Title}", 
                sourceJob.Id, sourceJob.Title);
            throw;
        }
    }

    public IEnumerable<Job> MapToJobs(IEnumerable<JoobleJob> sourceJobs)
    {
        var jobs = new List<Job>();
        var errors = new List<string>();

        foreach (var sourceJob in sourceJobs)
        {
            try
            {
                if (IsValid(sourceJob))
                {
                    var job = MapToJob(sourceJob);
                    jobs.Add(job);
                }
                else
                {
                    var validationErrors = string.Join(", ", GetValidationErrors(sourceJob));
                    _logger.LogWarning("Skipping invalid Jooble job {JobId}: {Errors}", 
                        sourceJob.Id, validationErrors);
                    errors.Add($"Job {sourceJob.Id}: {validationErrors}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to map Jooble job {JobId}", sourceJob.Id);
                errors.Add($"Job {sourceJob.Id}: {ex.Message}");
            }
        }

        if (errors.Any())
        {
            _logger.LogWarning("Mapped {Success} jobs successfully, {Failed} failed", 
                jobs.Count, errors.Count);
        }

        return jobs;
    }

    public bool IsValid(JoobleJob sourceJob)
    {
        return !GetValidationErrors(sourceJob).Any();
    }

    public IEnumerable<string> GetValidationErrors(JoobleJob sourceJob)
    {
        var errors = new List<string>();

        if (sourceJob == null)
        {
            errors.Add("Source job is null");
            return errors;
        }

        if (string.IsNullOrWhiteSpace(sourceJob.Title))
            errors.Add("Title is required");

        if (string.IsNullOrWhiteSpace(sourceJob.Company))
            errors.Add("Company is required");

        if (string.IsNullOrWhiteSpace(sourceJob.Link))
            errors.Add("Link/URL is required");

        if (string.IsNullOrWhiteSpace(sourceJob.Id))
            errors.Add("Job ID is required");

        return errors;
    }

    #region Private Helper Methods

    private DateTime ParseJoobleDate(string? dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
            return DateTime.UtcNow.AddDays(-7); // Default: assume posted a week ago

        try
        {
            // Jooble uses ISO 8601 format: "2023-09-15T12:55:35.3870000"
            if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, 
                DateTimeStyles.AdjustToUniversal, out var parsedDate))
            {
                return parsedDate;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to parse Jooble date: {DateString}", dateString);
        }

        return DateTime.UtcNow.AddDays(-7);
    }

    private string ExtractEmploymentType(string? typeField)
    {
        if (string.IsNullOrWhiteSpace(typeField))
            return "Full-time"; // Default

        var normalized = typeField.Trim().ToLowerInvariant();

        // Map Jooble employment types to our standard types
        return normalized switch
        {
            "full-time" or "fulltime" => "Full-time",
            "part-time" or "parttime" => "Part-time",
            "contract" => "Contract",
            "temporary" or "temp" => "Temporary",
            "internship" or "intern" => "Internship",
            "freelance" => "Freelance",
            _ => typeField.Trim() // Keep original if unrecognized
        };
    }

    private string ExtractExperienceLevel(string title, string snippet)
    {
        var combinedText = $"{title} {snippet}".ToLowerInvariant();

        // Check for experience level keywords
        if (combinedText.Contains("senior") || combinedText.Contains("sr.") || 
            combinedText.Contains("lead") || combinedText.Contains("principal"))
            return "Senior Level";

        if (combinedText.Contains("junior") || combinedText.Contains("jr.") || 
            combinedText.Contains("entry") || combinedText.Contains("graduate"))
            return "Entry Level";

        if (combinedText.Contains("mid-level") || combinedText.Contains("mid level") || 
            combinedText.Contains("intermediate"))
            return "Mid Level";

        if (combinedText.Contains("intern") || combinedText.Contains("trainee"))
            return "Internship";

        return "Mid Level"; // Default assumption
    }

    private string CleanDescription(string? snippet)
    {
        if (string.IsNullOrWhiteSpace(snippet))
            return string.Empty;

        // Remove HTML tags if any
        var cleaned = System.Text.RegularExpressions.Regex.Replace(snippet, "<.*?>", string.Empty);

        // Remove excessive whitespace
        cleaned = System.Text.RegularExpressions.Regex.Replace(cleaned, @"\s+", " ");

        // Remove common Jooble artifacts
        cleaned = cleaned.Replace("...", "").Trim();

        return cleaned;
    }

    private string GenerateExternalId(JoobleJob sourceJob)
    {
        // Fallback: generate ID from link or other unique properties
        if (!string.IsNullOrWhiteSpace(sourceJob.Link))
        {
            // Extract ID from URL or use hash of URL
            var hash = sourceJob.Link.GetHashCode();
            return $"jooble_{Math.Abs(hash)}";
        }

        // Last resort: use company + title hash
        var combined = $"{sourceJob.Company}_{sourceJob.Title}_{sourceJob.Location}";
        return $"jooble_{Math.Abs(combined.GetHashCode())}";
    }

    #endregion
}
