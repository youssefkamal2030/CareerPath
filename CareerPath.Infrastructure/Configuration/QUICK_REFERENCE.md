# AppSettings Configuration - Quick Reference Card

## 🚀 Quick Start

### Inject Individual Settings
```csharp
public MyService(IOptions<JwtSettings> jwtSettings)
{
    _jwtSettings = jwtSettings.Value;
    var secret = _jwtSettings.SecretKey;
}
```

### Inject Multiple Settings
```csharp
public MyService(IAppSettingsProvider settings)
{
    _settings = settings;
    var secret = _settings.Jwt.SecretKey;
    var db = _settings.Database.RemoteConnection;
}
```

## 📚 Configuration Classes

| Class | Access | Properties |
|-------|--------|-----------|
| `DatabaseSettings` | `_settings.Database` | LocalConnection, RemoteConnection, AIDataAnalysisConnection |
| `JwtSettings` | `_settings.Jwt` | SecretKey, Issuer, Audience, ExpirationMinutes |
| `EmailSettings` | `_settings.Email` | From, SmtpServer, Port, Username, Password, EnableSsl |
| `ExternalApiSettings` | `_settings.ExternalApis` | Jooble_API_Key, AITeamApiUrl, AITeamApiKey |

## 🔧 Common Patterns

### Pattern 1: Email Service
```csharp
public EmailService(IOptions<EmailSettings> emailSettings)
{
    _email = emailSettings.Value;
    var client = new SmtpClient(_email.SmtpServer, _email.Port);
}
```

### Pattern 2: Database Context
```csharp
public MyDbContext(IOptions<DatabaseSettings> dbSettings)
{
    _db = dbSettings.Value;
    var connection = _db.GetMainConnection(useRemote: true);
}
```

### Pattern 3: API Service
```csharp
public ApiService(IOptions<ExternalApiSettings> apiSettings)
{
    _api = apiSettings.Value;
    httpClient.DefaultRequestHeaders.Add("X-API-Key", _api.Jooble_API_Key);
}
```

### Pattern 4: Multiple Settings
```csharp
public MyService(IAppSettingsProvider settings)
{
    if (!settings.IsConfigurationValid())
    {
        throw new Exception("Configuration invalid");
    }
}
```

## ✅ Validation

### In Constructor
```csharp
public MyService(IOptions<JwtSettings> jwtSettings)
{
    _jwt = jwtSettings.Value;
    if (!_jwt.IsValid())
        throw new InvalidOperationException("JWT config invalid");
}
```

### Using Provider
```csharp
var errors = _settings.ValidateConfiguration();
if (errors.Any())
{
    // Handle errors
}
```

## 🧪 Unit Testing

### Mock Individual Settings
```csharp
var mock = new Mock<IOptions<JwtSettings>>();
mock.Setup(x => x.Value).Returns(new JwtSettings
{
    SecretKey = "test-key-32-chars-minimum",
    Issuer = "Test",
    Audience = "Test"
});
var service = new MyService(mock.Object);
```

### Mock AppSettingsProvider
```csharp
var mock = new Mock<IAppSettingsProvider>();
mock.Setup(x => x.Jwt).Returns(new JwtSettings { ... });
mock.Setup(x => x.Database).Returns(new DatabaseSettings { ... });
var service = new MyService(mock.Object);
```

## 📋 Helper Methods

### DatabaseSettings
```csharp
_db.GetMainConnection(useRemote: true)  // Gets RemoteConnection
_db.GetMainConnection(useRemote: false) // Gets LocalConnection
_db.GetAIConnection(useRemote: true)    // Gets AI remote
_db.GetAIConnection(useRemote: false)   // Gets AI local
_db.IsValid()                           // Validates settings
```

### All Settings Classes
```csharp
_jwtSettings.IsValid()          // true if all required fields present
_emailSettings.IsValid()        // true if SMTP configured properly
_apiSettings.IsValid()          // true if API keys present
_databaseSettings.IsValid()     // true if at least one connection exists
```

## 🎯 Which Pattern to Use?

| Scenario | Pattern | Example |
|----------|---------|---------|
| Service uses ONE config section | `IOptions<T>` | TokenService needs only JWT |
| Service uses MULTIPLE sections | `IAppSettingsProvider` | Startup service needs all configs |
| Controller needs config info | `IAppSettingsProvider` | Config status endpoint |
| Repository/DbContext | `IOptions<DatabaseSettings>` | Only needs DB connection |
| Email service | `IOptions<EmailSettings>` | Only needs SMTP settings |
| API client | `IOptions<ExternalApiSettings>` | Only needs API keys |

## ⚡ Performance Notes

- ✅ `IOptions<T>` is registered as **Singleton** - created once
- ✅ `IAppSettingsProvider` is **Singleton** - no overhead
- ✅ Configuration is loaded at startup and cached
- ✅ No performance penalty for using strongly-typed settings
- ✅ Actually FASTER than string-based lookups

## 🔐 Security Reminders

- ❌ Never commit `appsettings.json` with real secrets
- ✅ Use `appsettings.json.example` for templates
- ✅ Real secrets only in local `appsettings.json` (gitignored)
- ✅ Production uses environment variables
- ❌ Never log sensitive configuration values

## 📖 Full Documentation

- **README.md** - Overview and quick start
- **USAGE_EXAMPLE.md** - Comprehensive examples
- **ExampleUsageService.cs** - Working code samples
- **ExampleController.cs.txt** - Controller examples

## 🆘 Troubleshooting

### "Cannot resolve IOptions<JwtSettings>"
✅ Ensure `services.AddAppSettingsConfiguration(configuration)` is called in DI.cs

### "Configuration validation failed"
✅ Check your `appsettings.json` has all required sections

### "Value is null"
✅ Use `.Value` property: `jwtSettings.Value.SecretKey`

### "Section not found"
✅ Verify section name matches: `"Jwt"`, `"ConnectionStrings"`, `"EmailSettings"`, `"Sources"`

## 🔗 Related

- Location: `CareerPath.Infrastructure/Configuration/`
- Registration: `CareerPath.Infrastructure/DI.cs`
- Validation: `CareerPath/Program.cs`
- Template: `CareerPath/appsettings.json.example`
