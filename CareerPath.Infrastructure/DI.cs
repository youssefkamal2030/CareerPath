using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using CareerPath.Infrastructure.Data;
using CareerPath.Infrastructure.Repository;
using CareerPath.Application.Interfaces;
using CareerPath.Infrastructure.DbInitializer;
using CareerPath.Application.Services;
using EmailConfigration.EmailConfig;
using CareerPath.Application.Configuration;
using CareerPath.Domain.Identity;
using CareerPath.Infrastructure.Configuration;

namespace CareerPath.Infrastructure;

public static class InfrastructureDIExtensions
{
    public static IServiceCollection AddInfraStructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register strongly-typed configuration settings
        services.AddAppSettingsConfiguration(configuration);

        // Get settings via IOptions pattern
        var databaseSettings = configuration.GetSection(DatabaseSettings.SectionName).Get<DatabaseSettings>();
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();

        // Database Contexts
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(databaseSettings.RemoteConnection,
            sqlServerOptionsAction: sqlOptions => 
            {
                //sqlOptions.EnableRetryOnFailure(
                //    maxRetryCount: 5,
                //    maxRetryDelay: TimeSpan.FromSeconds(30),
                //    errorNumbersToAdd: null); ----> commented it because im manging the transactions manually 
            }));
            
        services.AddDbContext<AIDataAnalysisDbContext>(options =>
            options.UseSqlServer(databaseSettings.AIDataAnalysisConnection_RemoteConnection,
            sqlServerOptionsAction: sqlOptions => 
            {
                //sqlOptions.EnableRetryOnFailure(
                //    maxRetryCount: 5,
                //    maxRetryDelay: TimeSpan.FromSeconds(30),
                //    errorNumbersToAdd: null);----> commented it because im manging the transactions manually 
            }));
          

        // ASP.NET Core Identity Configuration
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Password settings
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            
            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
            
            // User settings
            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = false; // Allow duplicate emails as per existing logic
            
            // Sign-in settings
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        services.AddDefaultIdentity<ApplicationUser>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 0;
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // JWT Authentication
        var key = GetSecretKeyBytes(jwtSettings.SecretKey);
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false; // Set to true in production
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        // Email Configuration 
        var emailConfig = new EmailConfigration.EmailConfig.EmailConfigration();
        configuration.GetSection("EmailSettings").Bind(emailConfig);
        services.AddSingleton(emailConfig);

        // External Services Configuration
        services.Configure<ExternalServicesSettings>(
            configuration.GetSection("ExternalServices"));

        // Register HttpClient for AI Team API
        services.AddHttpClient("AITeamClient", (serviceProvider, client) =>
        {
            var settings = configuration
                .GetSection("ExternalServices:AITeamApi")
                .Get<AITeamApiSettings>();
                
            client.BaseAddress = new Uri(settings.BaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        // Register UnitOfWork
        services.AddScoped<Application.Interfaces.IUnitOfWork, UnitOfWork>();

        // Register Infrastructure Services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<EmailSender>();
        services.AddScoped<IJobRepository, JobRepository>();
        services.AddScoped<IJobsService, JobsService>();
        services.AddScoped<JobNotificationService>();
        services.AddScoped<ICVAnalysisService, CVAnalysisService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IJobApplicationService, JobApplicationService>();
        services.AddScoped<Dbinitializer>();

        return services;
    }

    private static byte[] GetSecretKeyBytes(string secretKey)
    {
        try
        {
            return Convert.FromBase64String(secretKey);
        }
        catch (FormatException)
        {
            string base64Secret = Convert.ToBase64String(Encoding.UTF8.GetBytes(secretKey));
            return Convert.FromBase64String(base64Secret);
        }
    }
}
