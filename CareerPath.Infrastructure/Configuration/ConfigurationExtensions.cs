using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CareerPath.Infrastructure.Configuration;

/// <summary>
/// Extension methods for registering configuration services
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Registers all strongly-typed configuration classes and the AppSettingsProvider
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddAppSettingsConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register strongly-typed configuration sections
        services.Configure<DatabaseSettings>(
            configuration.GetSection(DatabaseSettings.SectionName));

        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SectionName));

        services.Configure<EmailSettings>(
            configuration.GetSection(EmailSettings.SectionName));

        services.Configure<ExternalApiSettings>(
            configuration.GetSection(ExternalApiSettings.SectionName));

        // Register the centralized settings provider
        services.AddSingleton<IAppSettingsProvider, AppSettingsProvider>();

        return services;
    }

    /// <summary>
    /// Validates configuration at startup and throws if invalid
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="throwOnError">If true, throws exception on validation errors</param>
    public static void ValidateAppSettings(
        this IServiceProvider services,
        bool throwOnError = true)
    {
        var settingsProvider = services.GetRequiredService<IAppSettingsProvider>();
        var errors = settingsProvider.ValidateConfiguration();

        if (errors.Any())
        {
            var errorMessage = "Configuration validation failed:\n" +
                               string.Join("\n", errors.Select(e => $"  - {e.Key}: {e.Value}"));

            if (throwOnError)
            {
                throw new InvalidOperationException(errorMessage);
            }
            else
            {
                Console.WriteLine($"WARNING: {errorMessage}");
            }
        }
    }
}
