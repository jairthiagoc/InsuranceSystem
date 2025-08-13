using Microsoft.Extensions.DependencyInjection;
using InsuranceSystem.Shared.Infrastructure.Http;
using InsuranceSystem.Shared.Infrastructure.Logging;
using System.Diagnostics.CodeAnalysis;

namespace InsuranceSystem.Shared.Infrastructure.Configuration;

[ExcludeFromCodeCoverage]
public static class SharedServiceCollectionExtensions
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        // Configuração padrão do HttpClient resiliente
        services.AddResilientHttpClient("DefaultClient", options =>
        {
            options.MaxRetryAttempts = 3;
            options.RetryDelayMilliseconds = 1000;
            options.CircuitBreakerThreshold = 5;
            options.CircuitBreakerDurationMilliseconds = 30000;
            options.TimeoutMilliseconds = 30000;
            options.EnableLogging = true;
        });

        // Registra o serviço ResilientHttpClient
        services.AddScoped<IResilientHttpClient, ResilientHttpClient>();

        // Registra o serviço de logging
        services.AddScoped<ILoggingService, LoggingService>();

        return services;
    }

    public static IServiceCollection AddResilientHttpClient(
        this IServiceCollection services,
        string name,
        Action<ResilientHttpClientOptions> configureOptions)
    {
        var options = new ResilientHttpClientOptions();
        configureOptions(options);

        services.AddHttpClient(name, client =>
        {
            client.Timeout = TimeSpan.FromMilliseconds(options.TimeoutMilliseconds);
        });

        return services;
    }
} 