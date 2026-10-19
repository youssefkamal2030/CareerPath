using CareerPath.Domain.Recommendations;

namespace CareerPath.Infrastructure.Mappers;

/// <summary>
/// Interface for mapping external job source data to our internal Job domain model
/// Each external job source (Jooble, Indeed, LinkedIn, etc.) should have its own implementation
/// </summary>
/// <typeparam name="TSource">The source type from the external API (e.g., JoobleJob)</typeparam>
public interface IJobSourceMapper<TSource> where TSource : class
{
    /// <summary>
    /// The name/identifier of this job source provider (e.g., "Jooble", "Indeed")
    /// </summary>
    string ProviderName { get; }

    /// <summary>
    /// Maps a single external job item to our internal Job domain model
    /// </summary>
    /// <param name="sourceJob">The external job data from the API</param>
    /// <returns>Mapped Job entity ready for persistence</returns>
    Job MapToJob(TSource sourceJob);

    /// <summary>
    /// Maps a collection of external job items to our internal Job domain models
    /// </summary>
    /// <param name="sourceJobs">Collection of external job data</param>
    /// <returns>Collection of mapped Job entities</returns>
    IEnumerable<Job> MapToJobs(IEnumerable<TSource> sourceJobs);

    /// <summary>
    /// Validates if the source job has minimum required data for mapping
    /// </summary>
    /// <param name="sourceJob">The external job data to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    bool IsValid(TSource sourceJob);

    /// <summary>
    /// Gets validation errors for a source job (if any)
    /// </summary>
    /// <param name="sourceJob">The external job data to validate</param>
    /// <returns>List of validation error messages</returns>
    IEnumerable<string> GetValidationErrors(TSource sourceJob);
}
