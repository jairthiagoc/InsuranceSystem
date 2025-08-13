using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using ContractService.Infrastructure.Messaging;
using InsuranceSystem.Shared.Infrastructure.Messaging.Events;
using ContractService.Tests.Helpers;

namespace ContractService.Tests.Infrastructure.Messaging;

public class ProposalStatusUpdatedConsumerTests
{
    private readonly Mock<ILogger<ProposalStatusUpdatedConsumer>> _mockLogger;
    private readonly ProposalStatusUpdatedConsumer _consumer;

    public ProposalStatusUpdatedConsumerTests()
    {
        _mockLogger = new Mock<ILogger<ProposalStatusUpdatedConsumer>>();
        _consumer = new ProposalStatusUpdatedConsumer(_mockLogger.Object);
    }

    [Fact]
    public async Task Consume_WhenProposalIsApproved_ShouldLogApprovalMessage()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var message = new ProposalStatusUpdated(
            proposalId,
            "Approved",
            null,
            DateTime.UtcNow
        );

        var context = Mock.Of<ConsumeContext<ProposalStatusUpdated>>(c =>
            c.Message == message);

        // Act
        await _consumer.Consume(context);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("foi aprovada")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task Consume_WhenProposalIsRejected_ShouldLogRejectionMessage()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var rejectionReason = "Documentação insuficiente";
        var message = new ProposalStatusUpdated(
            proposalId,
            "Rejected",
            rejectionReason,
            DateTime.UtcNow
        );

        var context = Mock.Of<ConsumeContext<ProposalStatusUpdated>>(c =>
            c.Message == message);

        // Act
        await _consumer.Consume(context);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("foi rejeitada")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task Consume_WhenProposalIsRejectedWithoutReason_ShouldLogDefaultReason()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var message = new ProposalStatusUpdated(
            proposalId,
            "Rejected",
            null,
            DateTime.UtcNow
        );

        var context = Mock.Of<ConsumeContext<ProposalStatusUpdated>>(c =>
            c.Message == message);

        // Act
        await _consumer.Consume(context);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Não informado")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task Consume_WhenProposalStatusIsUnderReview_ShouldLogStatusChange()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var message = new ProposalStatusUpdated(
            proposalId,
            "UnderReview",
            null,
            DateTime.UtcNow
        );

        var context = Mock.Of<ConsumeContext<ProposalStatusUpdated>>(c =>
            c.Message == message);

        // Act
        await _consumer.Consume(context);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("teve status alterado")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task Consume_WhenProposalStatusIsUnknown_ShouldLogStatusChange()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var message = new ProposalStatusUpdated(
            proposalId,
            "Unknown",
            null,
            DateTime.UtcNow
        );

        var context = Mock.Of<ConsumeContext<ProposalStatusUpdated>>(c =>
            c.Message == message);

        // Act
        await _consumer.Consume(context);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("teve status alterado")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    [Fact]
    public async Task Consume_ShouldCompleteSuccessfully()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var message = new ProposalStatusUpdated(
            proposalId,
            "Approved",
            null,
            DateTime.UtcNow
        );

        var context = Mock.Of<ConsumeContext<ProposalStatusUpdated>>(c =>
            c.Message == message);

        // Act & Assert
        var action = () => _consumer.Consume(context);
        await action.Should().NotThrowAsync();
    }

    [Theory]
    [InlineData("Approved")]
    [InlineData("Rejected")]
    [InlineData("UnderReview")]
    [InlineData("Unknown")]
    public async Task Consume_WithDifferentStatuses_ShouldLogAppropriateMessages(string status)
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var message = new ProposalStatusUpdated(
            proposalId,
            status,
            status == "Rejected" ? "Test reason" : null,
            DateTime.UtcNow
        );

        var context = Mock.Of<ConsumeContext<ProposalStatusUpdated>>(c =>
            c.Message == message);

        // Act
        await _consumer.Consume(context);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.AtLeast(1)
        );
    }
} 