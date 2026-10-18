namespace CareerPath.Infrastructure.Configuration;

/// <summary>
/// Strongly-typed configuration for JWT authentication settings
/// </summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    /// <summary>
    /// Secret key used for signing JWT tokens (minimum 32 characters recommended)
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// JWT token issuer
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// JWT token audience
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Token expiration time in minutes (optional, defaults to application logic)
    /// </summary>
    public int? ExpirationMinutes { get; set; }

    /// <summary>
    /// Validates that all required JWT settings are configured
    /// </summary>
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(SecretKey) &&
               SecretKey.Length >= 32 &&
               !string.IsNullOrWhiteSpace(Issuer) &&
               !string.IsNullOrWhiteSpace(Audience);
    }
}
