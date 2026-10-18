# AppSettings Configuration Usage Guide

## Overview

The `AppSettingsProvider` system provides strongly-typed access to all configuration values from `appsettings.json` using the IOptions pattern.

## Quick Start

### 1. Configuration is Auto-Registered

The configuration is automatically registered in `DI.cs`:

```csharp
services.AddAppSettingsConfiguration(configuration);
```

### 2. Inject Configuration in Your Services

You have **three ways** to access configuration:

#### Option A: Inject Individual Settings (Recommended for specific needs)

```csharp
public class MyService
{
    private readonly JwtSettings _jwtSettings;
    
    public MyService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }
    
    public void DoSomething()
    {
        var secretKey = _jwtSettings.SecretKey;
        var issuer = _jwtSettings.Issuer;
    }
}
```

#### Option B: Inject AppSettingsProvider (Recommended for multiple settings)

```csharp
public class MyService
{
    private readonly IAppSettingsProvider _settings;
    
    public MyService(IAppSettingsProvider settings)
    {
        _settings = settings;
    }
    
    public void DoSomething()
    {
        // Access JWT settings
        var jwtSecret = _settings.Jwt.SecretKey;
        
        // Access Database settings
        var connectionString = _settings.Database.GetMainConnection(useRemote: true);
        
        // Access Email settings
        var smtpServer = _settings.Email.SmtpServer;
        
        // Access External API settings
        var joobleKey = _settings.ExternalApis.Jooble_API_Key;
    }
}
```

#### Option C: Direct IConfiguration Access (Not recommended, but still works)

```csharp
public class MyService
{
    private readonly IConfiguration _configuration;
    
    public MyService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public void DoSomething()
    {
        var secretKey = _configuration["Jwt:SecretKey"];
    }
}
```

## Available Configuration Classes

### 1. DatabaseSettings
```csharp
public class DatabaseSettings
{
    public string LocalConnection { get; set; }
    public string RemoteConnection { get; set; }
    public string AIDataAnalysisConnection { get; set; }
    public string AIDataAnalysisConnection_RemoteConnection { get; set; }
    
    // Helper methods
    public string GetMainConnection(bool useRemote = false)
    public string GetAIConnection(bool useRemote = false)
}
```

**Usage:**
```csharp
public ExampleService(IOptions<DatabaseSettings> dbSettings)
{
    var db = dbSettings.Value;
    var connectionString = db.GetMainConnection(useRemote: true);
}
```

### 2. JwtSettings
```csharp
public class JwtSettings
{
    public string SecretKey { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int? ExpirationMinutes { get; set; }
}
```

**Usage:**
```csharp
public TokenService(IOptions<JwtSettings> jwtSettings)
{
    var jwt = jwtSettings.Value;
    var key = Encoding.UTF8.GetBytes(jwt.SecretKey);
}
```

### 3. EmailSettings
```csharp
public class EmailSettings
{
    public string From { get; set; }
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool EnableSsl { get; set; }
}
```

**Usage:**
```csharp
public EmailService(IOptions<EmailSettings> emailSettings)
{
    var email = emailSettings.Value;
    var client = new SmtpClient(email.SmtpServer, email.Port);
}
```

### 4. ExternalApiSettings
```csharp
public class ExternalApiSettings
{
    public string Jooble_API_Key { get; set; }
    public string AITeamApiUrl { get; set; }
    public string AITeamApiKey { get; set; }
}
```

**Usage:**
```csharp
public JobBoardService(IOptions<ExternalApiSettings> apiSettings)
{
    var apis = apiSettings.Value;
    var joobleKey = apis.Jooble_API_Key;
}
```

## Real-World Examples

### Example 1: Email Service

```csharp
public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    
    public SmtpEmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }
    
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
        {
            EnableSsl = _emailSettings.EnableSsl,
            Credentials = new NetworkCredential(
                _emailSettings.Username,
                _emailSettings.Password)
        };
        
        var message = new MailMessage(
            _emailSettings.From,
            to,
            subject,
            body);
            
        await client.SendMailAsync(message);
    }
}
```

### Example 2: Job Board API Service

```csharp
public class JoobleService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    
    public JoobleService(
        HttpClient httpClient,
        IOptions<ExternalApiSettings> apiSettings)
    {
        _httpClient = httpClient;
        _apiKey = apiSettings.Value.Jooble_API_Key;
    }
    
    public async Task<JobSearchResults> SearchJobsAsync(string query)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "api/search");
        request.Headers.Add("X-API-Key", _apiKey);
        // ... rest of implementation
    }
}
```

### Example 3: Using AppSettingsProvider for Multiple Settings

```csharp
public class ApplicationStartupService
{
    private readonly IAppSettingsProvider _settings;
    private readonly ILogger<ApplicationStartupService> _logger;
    
    public ApplicationStartupService(
        IAppSettingsProvider settings,
        ILogger<ApplicationStartupService> logger)
    {
        _settings = settings;
        _logger = logger;
    }
    
    public void ValidateConfiguration()
    {
        var errors = _settings.ValidateConfiguration();
        
        if (errors.Any())
        {
            foreach (var error in errors)
            {
                _logger.LogError($"Configuration Error - {error.Key}: {error.Value}");
            }
            throw new InvalidOperationException("Invalid configuration");
        }
        
        _logger.LogInformation("Configuration validated successfully");
    }
    
    public void LogConfigurationStatus()
    {
        _logger.LogInformation($"Database: {_settings.Database.GetMainConnection(true)}");
        _logger.LogInformation($"JWT Issuer: {_settings.Jwt.Issuer}");
        _logger.LogInformation($"Email Server: {_settings.Email.SmtpServer}:{_settings.Email.Port}");
        _logger.LogInformation($"Jooble API Configured: {!string.IsNullOrEmpty(_settings.ExternalApis.Jooble_API_Key)}");
    }
}
```

## Configuration Validation

### Automatic Validation at Startup

Add validation in `Program.cs` after building the app:

```csharp
var app = builder.Build();

// Validate configuration at startup
app.Services.ValidateAppSettings(throwOnError: true);

// ... rest of startup
```

### Manual Validation

```csharp
public class MyController : ControllerBase
{
    private readonly IAppSettingsProvider _settings;
    
    public MyController(IAppSettingsProvider settings)
    {
        _settings = settings;
    }
    
    [HttpGet("config-status")]
    public IActionResult GetConfigurationStatus()
    {
        var errors = _settings.ValidateConfiguration();
        
        if (errors.Any())
        {
            return BadRequest(new { errors });
        }
        
        return Ok(new { message = "Configuration is valid" });
    }
}
```

## Benefits

✅ **Type Safety**: Compile-time checking instead of runtime string lookups  
✅ **IntelliSense**: Full IDE support for configuration properties  
✅ **Validation**: Built-in validation methods  
✅ **Testability**: Easy to mock for unit tests  
✅ **Documentation**: Self-documenting code with XML comments  
✅ **Refactoring**: Rename-safe (no magic strings)  
✅ **Single Responsibility**: Each configuration class has a clear purpose  

## Migration Guide

### Before (Old Way):
```csharp
var connectionString = configuration.GetConnectionString("RemoteConnection");
var jwtSecret = configuration["Jwt:SecretKey"];
var smtpServer = configuration["EmailSettings:SmtpServer"];
```

### After (New Way):
```csharp
// Option 1: Individual settings
var connectionString = dbSettings.Value.RemoteConnection;
var jwtSecret = jwtSettings.Value.SecretKey;
var smtpServer = emailSettings.Value.SmtpServer;

// Option 2: AppSettingsProvider
var connectionString = _settings.Database.RemoteConnection;
var jwtSecret = _settings.Jwt.SecretKey;
var smtpServer = _settings.Email.SmtpServer;
```
