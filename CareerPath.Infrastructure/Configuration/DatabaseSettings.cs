namespace CareerPath.Infrastructure.Configuration;

/// <summary>
/// Strongly-typed configuration for database connection strings
/// </summary>
public class DatabaseSettings
{
    public const string SectionName = "ConnectionStrings";

    /// <summary>
    /// Local SQL Server connection string
    /// </summary>
    public string LocalConnection { get; set; } = string.Empty;

    /// <summary>
    /// AI Data Analysis database connection string (local)
    /// </summary>
    public string AIDataAnalysisConnection { get; set; } = string.Empty;

    /// <summary>
    /// Remote/Production SQL Server connection string
    /// </summary>
    public string RemoteConnection { get; set; } = string.Empty;

    /// <summary>
    /// AI Data Analysis remote database connection string
    /// </summary>
    public string AIDataAnalysisConnection_RemoteConnection { get; set; } = string.Empty;

    /// <summary>
    /// Gets the appropriate connection string based on environment
    /// </summary>
    /// <param name="useRemote">True to use remote connection, false for local</param>
    public string GetMainConnection(bool useRemote = false)
    {
        return useRemote ? RemoteConnection : LocalConnection;
    }

    /// <summary>
    /// Gets the appropriate AI database connection string based on environment
    /// </summary>
    /// <param name="useRemote">True to use remote connection, false for local</param>
    public string GetAIConnection(bool useRemote = false)
    {
        return useRemote ? AIDataAnalysisConnection_RemoteConnection : AIDataAnalysisConnection;
    }

    /// <summary>
    /// Validates that at least one connection string is configured
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(LocalConnection) ||
               !string.IsNullOrWhiteSpace(RemoteConnection);
    }
}
