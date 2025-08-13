using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using ContractService.Ports.Outbound;
using ContractService.Adapters.Outbound.Services;
using InsuranceSystem.Shared.Infrastructure.Http;
using ContractService.Tests.Helpers;

namespace ContractService.Tests.Adapters.Outbound.Services;

public class ProposalServiceClientTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<ResilientHttpClient>> _mockLogger;
    private readonly IResilientHttpClient _resilientHttpClient;
    private readonly ProposalServiceClient _service;

    public ProposalServiceClientTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("https://localhost:7001")
        };
        
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration
            .Setup(x => x["ProposalService:BaseUrl"])
            .Returns("https://localhost:7001");

        _mockLogger = new Mock<ILogger<ResilientHttpClient>>();
        _resilientHttpClient = new ResilientHttpClient(_httpClient, _mockLogger.Object);
        
        _service = new ProposalServiceClient(_resilientHttpClient, _mockConfiguration.Object);
    }

    [Fact]
    public async Task GetProposalAsync_WhenProposalExists_ShouldReturnProposalInfo()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var expectedProposalInfo = new
        {
            id = proposalId,
            customerName = "João Silva",
            customerEmail = "joao@email.com",
            insuranceType = "Auto",
            coverageAmount = 50000m,
            premiumAmount = 1200m,
            status = "Approved"
        };

        var jsonResponse = JsonSerializer.Serialize(expectedProposalInfo);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _service.GetProposalAsync(proposalId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(proposalId);
        result.CustomerName.Should().Be("João Silva");
        result.CustomerEmail.Should().Be("joao@email.com");
        result.InsuranceType.Should().Be("Auto");
        result.CoverageAmount.Should().Be(50000m);
        result.PremiumAmount.Should().Be(1200m);
        result.Status.Should().Be("Approved");
    }

    [Fact]
    public async Task GetProposalAsync_WhenProposalExistsWithNumericStatus_ShouldConvertStatusCorrectly()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var expectedProposalInfo = new
        {
            id = proposalId,
            customerName = "Maria Santos",
            customerEmail = "maria@email.com",
            insuranceType = "Home",
            coverageAmount = 100000m,
            premiumAmount = 2000m,
            status = 1 // Approved
        };

        var jsonResponse = JsonSerializer.Serialize(expectedProposalInfo);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _service.GetProposalAsync(proposalId);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be("Approved");
    }

    [Fact]
    public async Task GetProposalAsync_WhenProposalExistsWithRejectedStatus_ShouldConvertStatusCorrectly()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var expectedProposalInfo = new
        {
            id = proposalId,
            customerName = "Pedro Costa",
            customerEmail = "pedro@email.com",
            insuranceType = "Life",
            coverageAmount = 200000m,
            premiumAmount = 3000m,
            status = 2 // Rejected
        };

        var jsonResponse = JsonSerializer.Serialize(expectedProposalInfo);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _service.GetProposalAsync(proposalId);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be("Rejected");
    }

    [Fact]
    public async Task GetProposalAsync_WhenProposalExistsWithUnderReviewStatus_ShouldConvertStatusCorrectly()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var expectedProposalInfo = new
        {
            id = proposalId,
            customerName = "Ana Silva",
            customerEmail = "ana@email.com",
            insuranceType = "Health",
            coverageAmount = 50000m,
            premiumAmount = 800m,
            status = 0 // UnderReview
        };

        var jsonResponse = JsonSerializer.Serialize(expectedProposalInfo);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _service.GetProposalAsync(proposalId);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be("UnderReview");
    }

    [Fact]
    public async Task GetProposalAsync_WhenProposalExistsWithUnknownStatus_ShouldConvertStatusCorrectly()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var expectedProposalInfo = new
        {
            id = proposalId,
            customerName = "Carlos Lima",
            customerEmail = "carlos@email.com",
            insuranceType = "Travel",
            coverageAmount = 25000m,
            premiumAmount = 500m,
            status = 99 // Unknown
        };

        var jsonResponse = JsonSerializer.Serialize(expectedProposalInfo);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _service.GetProposalAsync(proposalId);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be("Unknown");
    }

    [Fact]
    public async Task GetProposalAsync_WhenProposalDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var httpResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(httpResponse);

        // Act
        var result = await _service.GetProposalAsync(proposalId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetProposalAsync_WhenHttpClientThrowsException_ShouldThrowHttpRequestException()
    {
        // Arrange
        var proposalId = Guid.NewGuid();

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act & Assert
        var action = () => _service.GetProposalAsync(proposalId);
        await action.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("Network error");
    }

    [Fact]
    public async Task GetProposalAsync_WhenResponseIsNotJson_ShouldThrowJsonException()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("Invalid JSON")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(httpResponse);

        // Act & Assert
        var action = () => _service.GetProposalAsync(proposalId);
        await action.Should().ThrowAsync<JsonException>();
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    [InlineData(HttpStatusCode.GatewayTimeout)]
    public async Task GetProposalAsync_WhenResponseIsError_ShouldThrowHttpRequestException(HttpStatusCode statusCode)
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var httpResponse = new HttpResponseMessage(statusCode);

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(httpResponse);

        // Act & Assert
        var action = () => _service.GetProposalAsync(proposalId);
        await action.Should().ThrowAsync<HttpRequestException>()
            .WithMessage($"HTTP {(int)statusCode}: {statusCode}");
    }

    [Fact]
    public async Task GetProposalAsync_WhenConfigurationIsNull_ShouldUseDefaultUrl()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(x => x["ProposalService:BaseUrl"]).Returns((string?)null);
        
        var service = new ProposalServiceClient(_resilientHttpClient, mockConfig.Object);

        var expectedProposalInfo = new
        {
            id = proposalId,
            customerName = "Test User",
            customerEmail = "test@email.com",
            insuranceType = "Auto",
            coverageAmount = 50000m,
            premiumAmount = 1200m,
            status = "Approved"
        };

        var jsonResponse = JsonSerializer.Serialize(expectedProposalInfo);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(httpResponse);

        // Act
        var result = await service.GetProposalAsync(proposalId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(proposalId);
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
} 