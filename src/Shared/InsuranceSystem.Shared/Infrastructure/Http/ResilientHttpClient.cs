using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace InsuranceSystem.Shared.Infrastructure.Http;

public class ResilientHttpClient : IResilientHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ResilientHttpClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ResilientHttpClient(HttpClient httpClient, ILogger<ResilientHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Making GET request to {RequestUri}", requestUri);
        
        try
        {
            var response = await _httpClient.GetAsync(requestUri, cancellationToken);
            _logger.LogInformation("GET request to {RequestUri} completed with status {StatusCode}", 
                requestUri, response.StatusCode);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GET request to {RequestUri} failed", requestUri);
            throw;
        }
    }

    public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Making POST request to {RequestUri}", requestUri);
        
        try
        {
            var response = await _httpClient.PostAsync(requestUri, content, cancellationToken);
            _logger.LogInformation("POST request to {RequestUri} completed with status {StatusCode}", 
                requestUri, response.StatusCode);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "POST request to {RequestUri} failed", requestUri);
            throw;
        }
    }

    public async Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Making PUT request to {RequestUri}", requestUri);
        
        try
        {
            var response = await _httpClient.PutAsync(requestUri, content, cancellationToken);
            _logger.LogInformation("PUT request to {RequestUri} completed with status {StatusCode}", 
                requestUri, response.StatusCode);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PUT request to {RequestUri} failed", requestUri);
            throw;
        }
    }

    public async Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Making DELETE request to {RequestUri}", requestUri);
        
        try
        {
            var response = await _httpClient.DeleteAsync(requestUri, cancellationToken);
            _logger.LogInformation("DELETE request to {RequestUri} completed with status {StatusCode}", 
                requestUri, response.StatusCode);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DELETE request to {RequestUri} failed", requestUri);
            throw;
        }
    }

    public async Task<T?> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default)
    {
        var response = await GetAsync(requestUri, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("GET request to {RequestUri} returned status {StatusCode}", 
                requestUri, response.StatusCode);
            return default;
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<T>(content, _jsonOptions);
    }

    public async Task<T?> PostAsync<T>(string requestUri, object data, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await PostAsync(requestUri, content, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("POST request to {RequestUri} returned status {StatusCode}", 
                requestUri, response.StatusCode);
            return default;
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
    }
} 