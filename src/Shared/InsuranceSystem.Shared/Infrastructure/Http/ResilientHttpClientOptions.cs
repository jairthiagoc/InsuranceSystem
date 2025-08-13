namespace InsuranceSystem.Shared.Infrastructure.Http;

public class ResilientHttpClientOptions
{
    public int MaxRetryAttempts { get; set; } = 3;
    public int RetryDelayMilliseconds { get; set; } = 1000;
    public int CircuitBreakerThreshold { get; set; } = 5;
    public int CircuitBreakerDurationMilliseconds { get; set; } = 30000;
    public int TimeoutMilliseconds { get; set; } = 30000;
    public bool EnableLogging { get; set; } = true;
    public string[] RetryableStatusCodes { get; set; } = { "408", "429", "500", "502", "503", "504" };
} 