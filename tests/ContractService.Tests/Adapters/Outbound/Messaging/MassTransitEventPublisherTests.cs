using FluentAssertions;
using MassTransit;
using Moq;
using ContractService.Adapters.Outbound.Messaging;
using InsuranceSystem.Shared.Infrastructure.Messaging.Events;
using ContractService.Tests.Helpers;

namespace ContractService.Tests.Adapters.Outbound.Messaging;

public class MassTransitEventPublisherTests
{
    private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;
    private readonly MassTransitEventPublisher _publisher;

    public MassTransitEventPublisherTests()
    {
        _mockPublishEndpoint = new Mock<IPublishEndpoint>();
        _publisher = new MassTransitEventPublisher(_mockPublishEndpoint.Object);
    }

    [Fact]
    public async Task PublishAsync_WithValidEvent_ShouldCallPublishEndpoint()
    {
        // Arrange
        var @event = new ContractCreated(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "CT-20241201-0001",
            1200m,
            DateTime.UtcNow,
            DateTime.UtcNow
        );

        _mockPublishEndpoint
            .Setup(x => x.Publish(@event, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _publisher.PublishAsync(@event);

        // Assert
        _mockPublishEndpoint.Verify(
            x => x.Publish(@event, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task PublishAsync_WithNullEvent_ShouldCallPublishEndpoint()
    {
        // Arrange
        ContractCreated? @event = null;

        _mockPublishEndpoint
            .Setup(x => x.Publish(@event, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _publisher.PublishAsync(@event);

        // Assert
        _mockPublishEndpoint.Verify(
            x => x.Publish(@event, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task PublishAsync_WithDifferentEventTypes_ShouldCallPublishEndpoint()
    {
        // Arrange
        var contractCreatedEvent = new ContractCreated(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "CT-20241201-0001",
            1200m,
            DateTime.UtcNow,
            DateTime.UtcNow
        );

        var proposalStatusUpdatedEvent = new ProposalStatusUpdated(
            Guid.NewGuid(),
            "Approved",
            null,
            DateTime.UtcNow
        );

        _mockPublishEndpoint
            .Setup(x => x.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _publisher.PublishAsync(contractCreatedEvent);
        await _publisher.PublishAsync(proposalStatusUpdatedEvent);

        // Assert
        _mockPublishEndpoint.Verify(
            x => x.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
    }

    [Fact]
    public async Task PublishAsync_WhenPublishEndpointThrowsException_ShouldPropagateException()
    {
        // Arrange
        var @event = new ContractCreated(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "CT-20241201-0001",
            1200m,
            DateTime.UtcNow,
            DateTime.UtcNow
        );

        var expectedException = new InvalidOperationException("Publish failed");
        _mockPublishEndpoint
            .Setup(x => x.Publish(@event, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var action = () => _publisher.PublishAsync(@event);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Publish failed");
    }

    [Fact]
    public async Task PublishAsync_WithDefaultCancellationToken_ShouldUseDefaultToken()
    {
        // Arrange
        var @event = new ContractCreated(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "CT-20241201-0001",
            1200m,
            DateTime.UtcNow,
            DateTime.UtcNow
        );

        _mockPublishEndpoint
            .Setup(x => x.Publish(@event, default(CancellationToken)))
            .Returns(Task.CompletedTask);

        // Act
        await _publisher.PublishAsync(@event);

        // Assert
        _mockPublishEndpoint.Verify(
            x => x.Publish(@event, default(CancellationToken)),
            Times.Once
        );
    }

    [Fact]
    public async Task PublishAsync_WithComplexEvent_ShouldCallPublishEndpoint()
    {
        // Arrange
        var @event = new ContractCreated(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "CT-20241201-9999",
            9999.99m,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddHours(-2)
        );

        _mockPublishEndpoint
            .Setup(x => x.Publish(@event, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _publisher.PublishAsync(@event);

        // Assert
        _mockPublishEndpoint.Verify(
            x => x.Publish(@event, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task PublishAsync_WithMultipleEvents_ShouldCallPublishEndpointForEach()
    {
        // Arrange
        var events = new List<ContractCreated>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), "CT-20241201-0001", 1000m, DateTime.UtcNow, DateTime.UtcNow),
            new(Guid.NewGuid(), Guid.NewGuid(), "CT-20241201-0002", 2000m, DateTime.UtcNow, DateTime.UtcNow),
            new(Guid.NewGuid(), Guid.NewGuid(), "CT-20241201-0003", 3000m, DateTime.UtcNow, DateTime.UtcNow)
        };

        _mockPublishEndpoint
            .Setup(x => x.Publish(It.IsAny<ContractCreated>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        foreach (var @event in events)
        {
            await _publisher.PublishAsync(@event);
        }

        // Assert
        _mockPublishEndpoint.Verify(
            x => x.Publish(It.IsAny<ContractCreated>(), It.IsAny<CancellationToken>()),
            Times.Exactly(3)
        );
    }

    [Fact]
    public async Task PublishAsync_ShouldCompleteSuccessfully()
    {
        // Arrange
        var @event = new ContractCreated(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "CT-20241201-0001",
            1200m,
            DateTime.UtcNow,
            DateTime.UtcNow
        );

        _mockPublishEndpoint
            .Setup(x => x.Publish(@event, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        var action = () => _publisher.PublishAsync(@event);
        await action.Should().NotThrowAsync();
    }
} 