namespace CareerPath.Infrastructure.Configuration;

/// <summary>
/// Strongly-typed configuration for external API sources
/// </summary>
public class ExternalApiSettings
{
    public const string SectionName = "Sources";

    /// <summary>
    /// Jooble API key for job search integration
    /// </summary>
    public string Jooble_API_Key { get; set; } = string.Empty;

    /// <summary>
    /// AI Team API base URL (if needed)
    /// </summary>
    public string? AITeamApiUrl { get; set; }

    /// <summary>
    /// AI Team API key (if needed)
    /// </summary>
    public string? AITeamApiKey { get; set; }

    /// <summary>
    /// Validates that all required API keys are configured
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Jooble_API_Key);
    }
}
