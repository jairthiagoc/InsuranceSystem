using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using InsuranceSystem.Shared.Infrastructure.Http;

namespace Shared.Tests;

public class ResilientHttpClientBuilderExtensionsTests
{
    [Fact]
    public void AddResilientHttpClient_WithValidOptions_ShouldRegisterHttpClientWithPolicies()
    {
        // Arrange
        var services = new ServiceCollection();
        var clientName = "TestClient";

        // Act
        var httpClientBuilder = services.AddResilientHttpClient(clientName, options =>
        {
            options.MaxRetryAttempts = 3;
            options.RetryDelayMilliseconds = 1000;
            options.CircuitBreakerThreshold = 5;
            options.CircuitBreakerDurationMilliseconds = 30000;
            options.TimeoutMilliseconds = 30000;
            options.EnableLogging = true;
            options.RetryableStatusCodes = new[] { "408", "429", "500", "502", "503", "504" };
        });

        // Assert
        httpClientBuilder.Should().NotBeNull();
        
        var serviceProvider = services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
        httpClientFactory.Should().NotBeNull();

        // Verify that the named client can be created
        var httpClient = httpClientFactory!.CreateClient(clientName);
        httpClient.Should().NotBeNull();
    }

    [Fact]
    public void AddResilientHttpClient_WithDefaultOptions_ShouldRegisterHttpClient()
    {
        // Arrange
        var services = new ServiceCollection();
        var clientName = "DefaultClient";

        // Act
        var httpClientBuilder = services.AddResilientHttpClient(clientName, options =>
        {
            // Use default options
        });

        // Assert
        httpClientBuilder.Should().NotBeNull();
        
        var serviceProvider = services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
        httpClientFactory.Should().NotBeNull();

        var httpClient = httpClientFactory!.CreateClient(clientName);
        httpClient.Should().NotBeNull();
    }

    [Fact]
    public void AddResilientHttpClient_WithCustomRetryableStatusCodes_ShouldRegisterHttpClient()
    {
        // Arrange
        var services = new ServiceCollection();
        var clientName = "CustomClient";

        // Act
        var httpClientBuilder = services.AddResilientHttpClient(clientName, options =>
        {
            options.MaxRetryAttempts = 2;
            options.RetryDelayMilliseconds = 500;
            options.CircuitBreakerThreshold = 3;
            options.CircuitBreakerDurationMilliseconds = 15000;
            options.TimeoutMilliseconds = 15000;
            options.EnableLogging = false;
            options.RetryableStatusCodes = new[] { "400", "401", "403", "404" };
        });

        // Assert
        httpClientBuilder.Should().NotBeNull();
        
        var serviceProvider = services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
        httpClientFactory.Should().NotBeNull();

        var httpClient = httpClientFactory!.CreateClient(clientName);
        httpClient.Should().NotBeNull();
    }

    [Fact]
    public void AddResilientHttpClient_WithEmptyRetryableStatusCodes_ShouldRegisterHttpClient()
    {
        // Arrange
        var services = new ServiceCollection();
        var clientName = "EmptyCodesClient";

        // Act
        var httpClientBuilder = services.AddResilientHttpClient(clientName, options =>
        {
            options.MaxRetryAttempts = 1;
            options.RetryDelayMilliseconds = 100;
            options.CircuitBreakerThreshold = 2;
            options.CircuitBreakerDurationMilliseconds = 5000;
            options.TimeoutMilliseconds = 5000;
            options.EnableLogging = true;
            options.RetryableStatusCodes = Array.Empty<string>();
        });

        // Assert
        httpClientBuilder.Should().NotBeNull();
        
        var serviceProvider = services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
        httpClientFactory.Should().NotBeNull();

        var httpClient = httpClientFactory!.CreateClient(clientName);
        httpClient.Should().NotBeNull();
    }

    [Fact]
    public void AddResilientHttpClient_WithNullRetryableStatusCodes_ShouldRegisterHttpClient()
    {
        // Arrange
        var services = new ServiceCollection();
        var clientName = "NullCodesClient";

        // Act
        var httpClientBuilder = services.AddResilientHttpClient(clientName, options =>
        {
            options.MaxRetryAttempts = 1;
            options.RetryDelayMilliseconds = 100;
            options.CircuitBreakerThreshold = 2;
            options.CircuitBreakerDurationMilliseconds = 5000;
            options.TimeoutMilliseconds = 5000;
            options.EnableLogging = false;
            options.RetryableStatusCodes = null!;
        });

        // Assert
        httpClientBuilder.Should().NotBeNull();
        
        var serviceProvider = services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
        httpClientFactory.Should().NotBeNull();

        var httpClient = httpClientFactory!.CreateClient(clientName);
        httpClient.Should().NotBeNull();
    }

    [Fact]
    public void AddResilientHttpClient_WithZeroValues_ShouldRegisterHttpClient()
    {
        // Arrange
        var services = new ServiceCollection();
        var clientName = "ZeroValuesClient";

        // Act
        var httpClientBuilder = services.AddResilientHttpClient(clientName, options =>
        {
            options.MaxRetryAttempts = 0;
            options.RetryDelayMilliseconds = 0;
            options.CircuitBreakerThreshold = 0;
            options.CircuitBreakerDurationMilliseconds = 0;
            options.TimeoutMilliseconds = 0;
            options.EnableLogging = false;
            options.RetryableStatusCodes = new[] { "500" };
        });

        // Assert
        httpClientBuilder.Should().NotBeNull();
        
        var serviceProvider = services.BuildServiceProvider();
        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
        httpClientFactory.Should().NotBeNull();

        var httpClient = httpClientFactory!.CreateClient(clientName);
        httpClient.Should().NotBeNull();
    }
} 