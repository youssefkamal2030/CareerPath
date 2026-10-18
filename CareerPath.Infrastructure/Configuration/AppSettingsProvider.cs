using Microsoft.Extensions.Options;

namespace CareerPath.Infrastructure.Configuration;

/// <summary>
/// Centralized provider for accessing strongly-typed application settings
/// Inject this service to access configuration values throughout the application
/// </summary>
public class AppSettingsProvider : IAppSettingsProvider
{
    private readonly DatabaseSettings _databaseSettings;
    private readonly JwtSettings _jwtSettings;
    private readonly EmailSettings _emailSettings;
    private readonly ExternalApiSettings _externalApiSettings;

    public AppSettingsProvider(
        IOptions<DatabaseSettings> databaseSettings,
        IOptions<JwtSettings> jwtSettings,
        IOptions<EmailSettings> emailSettings,
        IOptions<ExternalApiSettings> externalApiSettings)
    {
        _databaseSettings = databaseSettings.Value;
        _jwtSettings = jwtSettings.Value;
        _emailSettings = emailSettings.Value;
        _externalApiSettings = externalApiSettings.Value;
    }

    /// <summary>
    /// Database connection string settings
    /// </summary>
    public DatabaseSettings Database => _databaseSettings;

    /// <summary>
    /// JWT authentication settings
    /// </summary>
    public JwtSettings Jwt => _jwtSettings;

    /// <summary>
    /// Email/SMTP settings
    /// </summary>
    public EmailSettings Email => _emailSettings;

    /// <summary>
    /// External API settings (Jooble, AI services, etc.)
    /// </summary>
    public ExternalApiSettings ExternalApis => _externalApiSettings;

    /// <summary>
    /// Validates all critical configuration sections
    /// </summary>
    /// <returns>Dictionary of validation errors (empty if all valid)</returns>
    public Dictionary<string, string> ValidateConfiguration()
    {
        var errors = new Dictionary<string, string>();

        if (!_databaseSettings.IsValid())
        {
            errors.Add(DatabaseSettings.SectionName, "At least one database connection string must be configured");
        }

        if (!_jwtSettings.IsValid())
        {
            errors.Add(JwtSettings.SectionName, "JWT settings are invalid. Ensure SecretKey (min 32 chars), Issuer, and Audience are set");
        }

        if (!_emailSettings.IsValid())
        {
            errors.Add(EmailSettings.SectionName, "Email settings are incomplete. Check SMTP server, port, and credentials");
        }

        if (!_externalApiSettings.IsValid())
        {
            errors.Add(ExternalApiSettings.SectionName, "External API settings are incomplete. Check API keys");
        }

        return errors;
    }

    /// <summary>
    /// Checks if all configuration is valid
    /// </summary>
    public bool IsConfigurationValid()
    {
        return ValidateConfiguration().Count == 0;
    }
}

/// <summary>
/// Interface for the AppSettings provider
/// </summary>
public interface IAppSettingsProvider
{
    /// <summary>
    /// Database connection string settings
    /// </summary>
    DatabaseSettings Database { get; }

    /// <summary>
    /// JWT authentication settings
    /// </summary>
    JwtSettings Jwt { get; }

    /// <summary>
    /// Email/SMTP settings
    /// </summary>
    EmailSettings Email { get; }

    /// <summary>
    /// External API settings
    /// </summary>
    ExternalApiSettings ExternalApis { get; }

    /// <summary>
    /// Validates all critical configuration sections
    /// </summary>
    Dictionary<string, string> ValidateConfiguration();

    /// <summary>
    /// Checks if all configuration is valid
    /// </summary>
    bool IsConfigurationValid();
}
