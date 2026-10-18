using Microsoft.Extensions.Options;

namespace CareerPath.Infrastructure.Configuration;

/// <summary>
/// Example service demonstrating how to use AppSettings configuration
/// This is a reference implementation - delete or modify as needed
/// </summary>
public class ExampleUsageService
{
    private readonly IAppSettingsProvider _settings;
    private readonly JwtSettings _jwtSettings;
    private readonly DatabaseSettings _databaseSettings;

    /// <summary>
    /// Example 1: Using AppSettingsProvider for multiple settings
    /// </summary>
    public ExampleUsageService(IAppSettingsProvider settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// Example 2: Using individual IOptions for specific settings
    /// </summary>
    public ExampleUsageService(
        IOptions<JwtSettings> jwtSettings,
        IOptions<DatabaseSettings> databaseSettings)
    {
        _jwtSettings = jwtSettings.Value;
        _databaseSettings = databaseSettings.Value;
    }

    /// <summary>
    /// Example 3: Using AppSettingsProvider (recommended for services needing multiple configs)
    /// </summary>
    public void ExampleUsingAppSettingsProvider()
    {
        // Access JWT settings
        var jwtSecret = _settings.Jwt.SecretKey;
        var issuer = _settings.Jwt.Issuer;

        // Access Database settings with helper method
        var mainConnection = _settings.Database.GetMainConnection(useRemote: true);
        var aiConnection = _settings.Database.GetAIConnection(useRemote: false);

        // Access Email settings
        var smtpServer = _settings.Email.SmtpServer;
        var smtpPort = _settings.Email.Port;
        var emailFrom = _settings.Email.From;

        // Access External API settings
        var joobleApiKey = _settings.ExternalApis.Jooble_API_Key;

        // Validate all configuration
        var isValid = _settings.IsConfigurationValid();
        var errors = _settings.ValidateConfiguration();
    }

    /// <summary>
    /// Example 4: Using individual IOptions (recommended for services needing specific config)
    /// </summary>
    public void ExampleUsingIndividualOptions()
    {
        // Access JWT settings
        var secretKey = _jwtSettings.SecretKey;
        var issuer = _jwtSettings.Issuer;
        var audience = _jwtSettings.Audience;

        // Validate specific settings
        var isJwtValid = _jwtSettings.IsValid();

        // Access Database settings
        var connection = _databaseSettings.RemoteConnection;
        var isDatabaseValid = _databaseSettings.IsValid();
    }
}

/// <summary>
/// Example: Real-world service using Email configuration
/// </summary>
public class ExampleEmailService
{
    private readonly EmailSettings _emailSettings;

    public ExampleEmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;

        // Validate on construction (optional)
        if (!_emailSettings.IsValid())
        {
            throw new InvalidOperationException("Email settings are not properly configured");
        }
    }

    public void SendEmail()
    {
        // Use strongly-typed configuration
        var smtpServer = _emailSettings.SmtpServer;
        var port = _emailSettings.Port;
        var username = _emailSettings.Username;
        var password = _emailSettings.Password;
        var from = _emailSettings.From;
        var enableSsl = _emailSettings.EnableSsl;

        // ... email sending logic
    }
}

/// <summary>
/// Example: Real-world service using External API configuration
/// </summary>
public class ExampleJoobleService
{
    private readonly ExternalApiSettings _apiSettings;
    private readonly HttpClient _httpClient;

    public ExampleJoobleService(
        IOptions<ExternalApiSettings> apiSettings,
        HttpClient httpClient)
    {
        _apiSettings = apiSettings.Value;
        _httpClient = httpClient;

        // Validate on construction
        if (string.IsNullOrWhiteSpace(_apiSettings.Jooble_API_Key))
        {
            throw new InvalidOperationException("Jooble API key is not configured");
        }
    }

    public async Task<string> SearchJobsAsync(string keyword)
    {
        // Use the API key from configuration
        var apiKey = _apiSettings.Jooble_API_Key;

        // Add API key to request
        _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);

        // ... API call logic
        return await Task.FromResult("Jobs data");
    }
}
