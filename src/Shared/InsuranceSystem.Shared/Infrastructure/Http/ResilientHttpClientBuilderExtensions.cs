using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;

namespace InsuranceSystem.Shared.Infrastructure.Http;

public static class ResilientHttpClientBuilderExtensions
{
    public static IHttpClientBuilder AddResilientHttpClient(
        this IServiceCollection services,
        string name,
        Action<ResilientHttpClientOptions> configureOptions)
    {
        var options = new ResilientHttpClientOptions();
        configureOptions(options);

        return services
            .AddHttpClient(name)
            .AddPolicyHandler(GetRetryPolicy(options))
            .AddPolicyHandler(GetCircuitBreakerPolicy(options))
            .AddPolicyHandler(GetTimeoutPolicy(options));
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(ResilientHttpClientOptions options)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => options.RetryableStatusCodes.Contains(((int)msg.StatusCode).ToString()))
            .WaitAndRetryAsync(
                options.MaxRetryAttempts,
                retryAttempt => TimeSpan.FromMilliseconds(options.RetryDelayMilliseconds * Math.Pow(2, retryAttempt - 1)));
    }

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(ResilientHttpClientOptions options)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: options.CircuitBreakerThreshold,
                durationOfBreak: TimeSpan.FromMilliseconds(options.CircuitBreakerDurationMilliseconds));
    }

    private static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(ResilientHttpClientOptions options)
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(
            TimeSpan.FromMilliseconds(options.TimeoutMilliseconds),
            TimeoutStrategy.Optimistic);
    }
} 