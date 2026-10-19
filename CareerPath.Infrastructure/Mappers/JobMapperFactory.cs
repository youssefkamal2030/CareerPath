using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CareerPath.Infrastructure.Mappers;

/// <summary>
/// Factory for creating and managing job source mappers
/// Supports registering mappers for different job sources (Jooble, Indeed, LinkedIn, etc.)
/// </summary>
public class JobMapperFactory : IJobMapperFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<JobMapperFactory> _logger;

    public JobMapperFactory(
        IServiceProvider serviceProvider,
        ILogger<JobMapperFactory> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public IJobSourceMapper<TSource> GetMapper<TSource>(string providerName) where TSource : class
    {
        try
        {
            var mapper = _serviceProvider.GetService<IJobSourceMapper<TSource>>();
            
            if (mapper == null)
            {
                _logger.LogError("No mapper registered for provider {ProviderName} with source type {SourceType}", 
                    providerName, typeof(TSource).Name);
                throw new InvalidOperationException(
                    $"No mapper registered for provider '{providerName}' with source type '{typeof(TSource).Name}'");
            }

            _logger.LogDebug("Retrieved mapper for provider {ProviderName}", providerName);
            return mapper;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting mapper for provider {ProviderName}", providerName);
            throw;
        }
    }

    public bool HasMapper<TSource>(string providerName) where TSource : class
    {
        var mapper = _serviceProvider.GetService<IJobSourceMapper<TSource>>();
        return mapper != null && mapper.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Interface for the job mapper factory
/// </summary>
public interface IJobMapperFactory
{
    /// <summary>
    /// Gets a mapper for the specified provider and source type
    /// </summary>
    /// <typeparam name="TSource">The external API source type</typeparam>
    /// <param name="providerName">Name of the provider (e.g., "Jooble")</param>
    /// <returns>Mapper instance</returns>
    IJobSourceMapper<TSource> GetMapper<TSource>(string providerName) where TSource : class;

    /// <summary>
    /// Checks if a mapper exists for the specified provider and source type
    /// </summary>
    /// <typeparam name="TSource">The external API source type</typeparam>
    /// <param name="providerName">Name of the provider</param>
    /// <returns>True if mapper exists</returns>
    bool HasMapper<TSource>(string providerName) where TSource : class;
}
