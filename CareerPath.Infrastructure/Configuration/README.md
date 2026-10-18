# Configuration Management System

## 📁 Folder Contents

This folder contains strongly-typed configuration classes that provide type-safe access to `appsettings.json` values using the IOptions pattern.

### Files

- **DatabaseSettings.cs** - Database connection strings configuration
- **JwtSettings.cs** - JWT authentication configuration
- **EmailSettings.cs** - SMTP email service configuration
- **ExternalApiSettings.cs** - External API keys (Jooble, AI services, etc.)
- **AppSettingsProvider.cs** - Centralized provider for accessing all settings
- **ConfigurationExtensions.cs** - Extension methods for registering configuration services
- **ExampleUsageService.cs** - Code examples demonstrating usage patterns
- **USAGE_EXAMPLE.md** - Comprehensive usage documentation

## 🚀 Quick Start

### 1. Configuration is Already Registered

The configuration system is automatically registered in `DI.cs`:

```csharp
services.AddAppSettingsConfiguration(configuration);
```

### 2. Inject in Your Service

Choose the approach that fits your needs:

**Option A: Individual Settings** (Best for services using one config section)
```csharp
public class MyService
{
    private readonly JwtSettings _jwtSettings;
    
    public MyService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
        var secret = _jwtSettings.SecretKey;
    }
}
```

**Option B: AppSettingsProvider** (Best for services using multiple config sections)
```csharp
public class MyService
{
    private readonly IAppSettingsProvider _settings;
    
    public MyService(IAppSettingsProvider settings)
    {
        _settings = settings;
        var secret = _settings.Jwt.SecretKey;
        var dbConnection = _settings.Database.RemoteConnection;
    }
}
```

## 📦 Available Configuration Classes

| Class | Purpose | Section Name |
|-------|---------|--------------|
| `DatabaseSettings` | Connection strings for SQL Server databases | `ConnectionStrings` |
| `JwtSettings` | JWT authentication configuration | `Jwt` |
| `EmailSettings` | SMTP email service settings | `EmailSettings` |
| `ExternalApiSettings` | External API keys and URLs | `Sources` |

## 🔍 Features

✅ **Type Safety** - No more magic strings, compile-time checking  
✅ **IntelliSense** - Full IDE support with autocomplete  
✅ **Validation** - Built-in `IsValid()` methods for all settings  
✅ **Helper Methods** - Convenience methods like `GetMainConnection(useRemote)`  
✅ **Testability** - Easy to mock for unit tests  
✅ **Documentation** - XML comments on all properties  

## 📖 Usage Examples

### Example 1: Email Service
```csharp
public class EmailService
{
    private readonly EmailSettings _emailSettings;
    
    public EmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }
    
    public void SendEmail()
    {
        var smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port);
        // ... rest of implementation
    }
}
```

### Example 2: Token Service
```csharp
public class TokenService
{
    private readonly JwtSettings _jwtSettings;
    
    public TokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }
    
    public string GenerateToken()
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        // ... rest of implementation
    }
}
```

### Example 3: Database Context
```csharp
public class MyDbContext : DbContext
{
    private readonly DatabaseSettings _dbSettings;
    
    public MyDbContext(IOptions<DatabaseSettings> dbSettings)
    {
        _dbSettings = dbSettings.Value;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(_dbSettings.GetMainConnection(useRemote: true));
    }
}
```

### Example 4: Using AppSettingsProvider
```csharp
public class ApplicationStartup
{
    private readonly IAppSettingsProvider _settings;
    
    public ApplicationStartup(IAppSettingsProvider settings)
    {
        _settings = settings;
    }
    
    public void ValidateAndLog()
    {
        // Validate all configuration
        var errors = _settings.ValidateConfiguration();
        if (errors.Any())
        {
            throw new Exception($"Configuration errors: {string.Join(", ", errors)}");
        }
        
        // Access multiple settings
        Console.WriteLine($"Database: {_settings.Database.RemoteConnection}");
        Console.WriteLine($"JWT Issuer: {_settings.Jwt.Issuer}");
        Console.WriteLine($"Email Server: {_settings.Email.SmtpServer}");
        Console.WriteLine($"API Key: {_settings.ExternalApis.Jooble_API_Key}");
    }
}
```

## ✅ Validation

All configuration classes have built-in validation:

```csharp
public class MyService
{
    public MyService(IOptions<JwtSettings> jwtSettings)
    {
        var jwt = jwtSettings.Value;
        
        if (!jwt.IsValid())
        {
            throw new InvalidOperationException("JWT configuration is invalid");
        }
    }
}
```

Or validate everything at once:

```csharp
public void ValidateAllSettings(IAppSettingsProvider settings)
{
    var errors = settings.ValidateConfiguration();
    
    if (errors.Any())
    {
        foreach (var (section, message) in errors)
        {
            Console.WriteLine($"[{section}] {message}");
        }
    }
}
```

## 🔧 Adding New Configuration

To add a new configuration section:

1. **Create a new settings class:**
```csharp
public class MyNewSettings
{
    public const string SectionName = "MyNewSection";
    
    public string SomeSetting { get; set; } = string.Empty;
    
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(SomeSetting);
    }
}
```

2. **Register it in `ConfigurationExtensions.cs`:**
```csharp
services.Configure<MyNewSettings>(
    configuration.GetSection(MyNewSettings.SectionName));
```

3. **Add to `AppSettingsProvider.cs`:**
```csharp
private readonly MyNewSettings _myNewSettings;

public MyNewSettings MyNew => _myNewSettings;
```

4. **Add to `appsettings.json`:**
```json
{
  "MyNewSection": {
    "SomeSetting": "value"
  }
}
```

## 📚 Documentation

- See **USAGE_EXAMPLE.md** for comprehensive examples
- See **ExampleUsageService.cs** for code examples
- Each configuration class has XML documentation

## 🧪 Testing

Configuration classes are easy to mock:

```csharp
[Test]
public void TestMyService()
{
    // Arrange
    var mockSettings = new Mock<IOptions<JwtSettings>>();
    mockSettings.Setup(x => x.Value).Returns(new JwtSettings
    {
        SecretKey = "test-secret-key-for-testing-purposes",
        Issuer = "TestIssuer",
        Audience = "TestAudience"
    });
    
    var service = new MyService(mockSettings.Object);
    
    // Act & Assert
    // ...
}
```

## 🔐 Security

⚠️ **Important:** Never commit `appsettings.json` with real secrets!

- Use `appsettings.json.example` for templates
- Keep real credentials in local `appsettings.json` (gitignored)
- Use environment variables in production
- See `CONFIGURATION.md` in the project root for security guidelines

## 📝 Migration from Old Code

**Before:**
```csharp
var secretKey = configuration["Jwt:SecretKey"];
var connectionString = configuration.GetConnectionString("RemoteConnection");
```

**After:**
```csharp
var secretKey = _jwtSettings.SecretKey;
var connectionString = _databaseSettings.RemoteConnection;
```

## 🎯 Best Practices

1. ✅ **Use IOptions pattern** for dependency injection
2. ✅ **Validate configuration** at startup or in constructors
3. ✅ **Use AppSettingsProvider** when needing multiple config sections
4. ✅ **Use individual settings** when only one config section is needed
5. ✅ **Add validation logic** to all new configuration classes
6. ✅ **Document properties** with XML comments
7. ❌ **Avoid IConfiguration** direct access (breaks type safety)
8. ❌ **Don't use magic strings** for config keys

## 🔗 Related Files

- `/CareerPath.Infrastructure/DI.cs` - Service registration
- `/CareerPath/appsettings.json` - Configuration file (gitignored)
- `/CareerPath/appsettings.json.example` - Configuration template
- `/CONFIGURATION.md` - Setup and security guide
