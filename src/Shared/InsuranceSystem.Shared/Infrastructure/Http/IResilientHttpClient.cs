namespace InsuranceSystem.Shared.Infrastructure.Http;

public interface IResilientHttpClient
{
    Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken = default);
    Task<T?> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default);
    Task<T?> PostAsync<T>(string requestUri, object data, CancellationToken cancellationToken = default);
} 