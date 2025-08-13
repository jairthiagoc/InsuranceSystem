using FluentAssertions;
using InsuranceSystem.Shared.Infrastructure.Configuration;
using InsuranceSystem.Shared.Infrastructure.Http;
using InsuranceSystem.Shared.Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace ContractService.Tests.Infrastructure;

public class SharedServiceCollectionExtensionsTests
{
    [Fact]
    public void AddSharedServices_ShouldRegisterLoggingAndHttpClient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddSharedServices();
        var provider = services.BuildServiceProvider();

        // Assert
        provider.GetService<ILoggingService>().Should().NotBeNull();
        provider.GetService<IResilientHttpClient>().Should().NotBeNull();
    }
} 