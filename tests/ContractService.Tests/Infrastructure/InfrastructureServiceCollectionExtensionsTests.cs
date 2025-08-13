using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ContractService.Adapters.DependencyInjection;
using ContractService.Ports.Outbound;
using ContractService.Ports.Inbound;
using ContractService.Core.UseCases;
using ContractService.Adapters.Outbound.Repositories;
using ContractService.Adapters.Outbound.Services;
using ContractService.Adapters.Outbound.Messaging;
using ContractService.Adapters.Inbound.Controllers;
using ContractService.Tests.Helpers;

namespace ContractService.Tests.Infrastructure;

public class InfrastructureServiceCollectionExtensionsTests
{
    [Fact]
    public void AddInfrastructure_ShouldRegisterRequiredServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Database=testdb;Trusted_Connection=true;",
                ["ProposalService:BaseUrl"] = "https://localhost:7001",
                ["Redis:ConnectionString"] = "localhost:6379"
            })
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Act
        services.AddInfrastructure(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Verificar se os serviços principais estão registrados
        serviceProvider.GetService<IContractRepositoryPort>().Should().NotBeNull();
        serviceProvider.GetService<IProposalServicePort>().Should().NotBeNull();
        serviceProvider.GetService<IEventPublisherPort>().Should().NotBeNull();
        serviceProvider.GetService<IContractNumberGeneratorPort>().Should().NotBeNull();
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterUseCases()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Database=testdb;Trusted_Connection=true;",
                ["ProposalService:BaseUrl"] = "https://localhost:7001",
                ["Redis:ConnectionString"] = "localhost:6379"
            })
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Act
        services.AddInfrastructure(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Verificar se os use cases estão registrados
        serviceProvider.GetService<ICreateContractPort>().Should().NotBeNull();
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterAdapters()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Database=testdb;Trusted_Connection=true;",
                ["ProposalService:BaseUrl"] = "https://localhost:7001",
                ["Redis:ConnectionString"] = "localhost:6379"
            })
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Act
        services.AddInfrastructure(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Verificar se os adapters estão registrados
        serviceProvider.GetService<ContractsController>().Should().NotBeNull();
    }

    [Fact]
    public void AddInfrastructure_WithNullConfiguration_ShouldThrowArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        var action = () => services.AddInfrastructure(null!);
        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("configuration");
    }

    [Fact]
    public void AddInfrastructure_WithMissingConnectionString_ShouldRegisterServicesWithDefaults()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Act
        services.AddInfrastructure(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Verificar se os serviços ainda são registrados mesmo sem connection string
        serviceProvider.GetService<IContractRepositoryPort>().Should().NotBeNull();
        serviceProvider.GetService<IProposalServicePort>().Should().NotBeNull();
    }

    [Fact]
    public void AddInfrastructure_ShouldRegisterScopedServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Database=testdb;Trusted_Connection=true;",
                ["ProposalService:BaseUrl"] = "https://localhost:7001",
                ["Redis:ConnectionString"] = "localhost:6379"
            })
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Act
        services.AddInfrastructure(configuration);

        // Assert
        var serviceProvider = services.BuildServiceProvider();

        // Verificar se os serviços são scoped
        using var scope1 = serviceProvider.CreateScope();
        using var scope2 = serviceProvider.CreateScope();

        var service1 = scope1.ServiceProvider.GetService<IContractRepositoryPort>();
        var service2 = scope2.ServiceProvider.GetService<IContractRepositoryPort>();

        service1.Should().NotBeNull();
        service2.Should().NotBeNull();
        service1.Should().NotBeSameAs(service2);
    }
} 