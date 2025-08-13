using FluentAssertions;
using Moq;
using ProposalService.Ports.Inbound;
using ProposalService.Ports.Inbound.Shared;
using ProposalService.Core.UseCases;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Enums;
using ProposalService.Ports.Outbound;
using InsuranceSystem.Shared.Infrastructure.Messaging.Events;
using ProposalService.Tests.Helpers;

namespace ProposalService.Tests.Core;

public class UpdateProposalStatusUseCaseTests
{
    private readonly Mock<IProposalRepositoryPort> _mockProposalRepository;
    private readonly Mock<IEventPublisherPort> _mockEventPublisher;
    private readonly UpdateProposalStatusUseCase _useCase;

    public UpdateProposalStatusUseCaseTests()
    {
        _mockProposalRepository = new Mock<IProposalRepositoryPort>();
        _mockEventPublisher = new Mock<IEventPublisherPort>();
        
        _useCase = new UpdateProposalStatusUseCase(
            _mockProposalRepository.Object,
            _mockEventPublisher.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldUpdateProposalStatus()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var request = new UpdateProposalStatusRequest(proposalId, ProposalStatus.Approved, null);
        var existingProposal = FakeDataGenerator.GenerateProposal();

        _mockProposalRepository
            .Setup(x => x.GetByIdAsync(proposalId))
            .ReturnsAsync(existingProposal);

        _mockProposalRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Proposal>()))
            .ReturnsAsync((Proposal p) => p);

        _mockEventPublisher
            .Setup(x => x.PublishAsync(It.IsAny<ProposalStatusUpdated>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(existingProposal.Id);
        result.Status.Should().Be(ProposalStatus.Approved);

        _mockProposalRepository.Verify(x => x.GetByIdAsync(proposalId), Times.Once);
        _mockProposalRepository.Verify(x => x.UpdateAsync(It.IsAny<Proposal>()), Times.Once);
        _mockEventPublisher.Verify(x => x.PublishAsync(It.IsAny<ProposalStatusUpdated>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithRejectionReason_ShouldUpdateProposalStatus()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var rejectionReason = "Documentação insuficiente";
        var request = new UpdateProposalStatusRequest(proposalId, ProposalStatus.Rejected, rejectionReason);
        var existingProposal = FakeDataGenerator.GenerateProposal();

        _mockProposalRepository
            .Setup(x => x.GetByIdAsync(proposalId))
            .ReturnsAsync(existingProposal);

        _mockProposalRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Proposal>()))
            .ReturnsAsync((Proposal p) => p);

        _mockEventPublisher
            .Setup(x => x.PublishAsync(It.IsAny<ProposalStatusUpdated>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(existingProposal.Id);
        result.Status.Should().Be(ProposalStatus.Rejected);
        result.RejectionReason.Should().Be(rejectionReason);

        _mockProposalRepository.Verify(x => x.GetByIdAsync(proposalId), Times.Once);
        _mockProposalRepository.Verify(x => x.UpdateAsync(It.IsAny<Proposal>()), Times.Once);
        _mockEventPublisher.Verify(x => x.PublishAsync(It.IsAny<ProposalStatusUpdated>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenProposalNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var request = new UpdateProposalStatusRequest(proposalId, ProposalStatus.Approved, null);

        _mockProposalRepository
            .Setup(x => x.GetByIdAsync(proposalId))
            .ReturnsAsync((Proposal?)null);

        // Act & Assert
        var action = () => _useCase.ExecuteAsync(request);
        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage($"Proposal with ID {proposalId} not found");

        _mockProposalRepository.Verify(x => x.GetByIdAsync(proposalId), Times.Once);
        _mockProposalRepository.Verify(x => x.UpdateAsync(It.IsAny<Proposal>()), Times.Never);
        _mockEventPublisher.Verify(x => x.PublishAsync(It.IsAny<ProposalStatusUpdated>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithUnderReviewStatus_ShouldUpdateProposalStatus()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var request = new UpdateProposalStatusRequest(proposalId, ProposalStatus.UnderReview, null);
        var existingProposal = FakeDataGenerator.GenerateProposal();

        _mockProposalRepository
            .Setup(x => x.GetByIdAsync(proposalId))
            .ReturnsAsync(existingProposal);

        _mockProposalRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Proposal>()))
            .ReturnsAsync((Proposal p) => p);

        _mockEventPublisher
            .Setup(x => x.PublishAsync(It.IsAny<ProposalStatusUpdated>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(existingProposal.Id);
        result.Status.Should().Be(ProposalStatus.UnderReview);

        _mockProposalRepository.Verify(x => x.GetByIdAsync(proposalId), Times.Once);
        _mockProposalRepository.Verify(x => x.UpdateAsync(It.IsAny<Proposal>()), Times.Once);
        _mockEventPublisher.Verify(x => x.PublishAsync(It.IsAny<ProposalStatusUpdated>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyRejectionReason_ShouldThrowArgumentException()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var request = new UpdateProposalStatusRequest(proposalId, ProposalStatus.Rejected, "");
        var existingProposal = FakeDataGenerator.GenerateProposal();

        _mockProposalRepository
            .Setup(x => x.GetByIdAsync(proposalId))
            .ReturnsAsync(existingProposal);

        // Act & Assert
        var action = () => _useCase.ExecuteAsync(request);
        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Rejection reason is required");

        _mockProposalRepository.Verify(x => x.GetByIdAsync(proposalId), Times.Once);
        _mockProposalRepository.Verify(x => x.UpdateAsync(It.IsAny<Proposal>()), Times.Never);
        _mockEventPublisher.Verify(x => x.PublishAsync(It.IsAny<ProposalStatusUpdated>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WithLongRejectionReason_ShouldUpdateProposalStatus()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var rejectionReason = new string('A', 1000);
        var request = new UpdateProposalStatusRequest(proposalId, ProposalStatus.Rejected, rejectionReason);
        var existingProposal = FakeDataGenerator.GenerateProposal();

        _mockProposalRepository
            .Setup(x => x.GetByIdAsync(proposalId))
            .ReturnsAsync(existingProposal);

        _mockProposalRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Proposal>()))
            .ReturnsAsync((Proposal p) => p);

        _mockEventPublisher
            .Setup(x => x.PublishAsync(It.IsAny<ProposalStatusUpdated>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(existingProposal.Id);
        result.Status.Should().Be(ProposalStatus.Rejected);
        result.RejectionReason.Should().Be(rejectionReason);

        _mockProposalRepository.Verify(x => x.GetByIdAsync(proposalId), Times.Once);
        _mockProposalRepository.Verify(x => x.UpdateAsync(It.IsAny<Proposal>()), Times.Once);
        _mockEventPublisher.Verify(x => x.PublishAsync(It.IsAny<ProposalStatusUpdated>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithSpecialCharactersInRejectionReason_ShouldUpdateProposalStatus()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var rejectionReason = "Documentação com caracteres especiais: @#$%&*()_+-=[]{}|;':\",./<>?";
        var request = new UpdateProposalStatusRequest(proposalId, ProposalStatus.Rejected, rejectionReason);
        var existingProposal = FakeDataGenerator.GenerateProposal();

        _mockProposalRepository
            .Setup(x => x.GetByIdAsync(proposalId))
            .ReturnsAsync(existingProposal);

        _mockProposalRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Proposal>()))
            .ReturnsAsync((Proposal p) => p);

        _mockEventPublisher
            .Setup(x => x.PublishAsync(It.IsAny<ProposalStatusUpdated>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(existingProposal.Id);
        result.Status.Should().Be(ProposalStatus.Rejected);
        result.RejectionReason.Should().Be(rejectionReason);

        _mockProposalRepository.Verify(x => x.GetByIdAsync(proposalId), Times.Once);
        _mockProposalRepository.Verify(x => x.UpdateAsync(It.IsAny<Proposal>()), Times.Once);
        _mockEventPublisher.Verify(x => x.PublishAsync(It.IsAny<ProposalStatusUpdated>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var request = new UpdateProposalStatusRequest(proposalId, ProposalStatus.Approved, null);

        _mockProposalRepository
            .Setup(x => x.GetByIdAsync(proposalId))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        var action = () => _useCase.ExecuteAsync(request);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Database error");

        _mockProposalRepository.Verify(x => x.GetByIdAsync(proposalId), Times.Once);
        _mockProposalRepository.Verify(x => x.UpdateAsync(It.IsAny<Proposal>()), Times.Never);
        _mockEventPublisher.Verify(x => x.PublishAsync(It.IsAny<ProposalStatusUpdated>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEventPublisherThrowsException_ShouldPropagateException()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var request = new UpdateProposalStatusRequest(proposalId, ProposalStatus.Approved, null);
        var existingProposal = FakeDataGenerator.GenerateProposal();

        _mockProposalRepository
            .Setup(x => x.GetByIdAsync(proposalId))
            .ReturnsAsync(existingProposal);

        _mockProposalRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Proposal>()))
            .ReturnsAsync((Proposal p) => p);

        _mockEventPublisher
            .Setup(x => x.PublishAsync(It.IsAny<ProposalStatusUpdated>()))
            .ThrowsAsync(new InvalidOperationException("Event publishing failed"));

        // Act & Assert
        var action = () => _useCase.ExecuteAsync(request);
        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Event publishing failed");

        _mockProposalRepository.Verify(x => x.GetByIdAsync(proposalId), Times.Once);
        _mockProposalRepository.Verify(x => x.UpdateAsync(It.IsAny<Proposal>()), Times.Once);
        _mockEventPublisher.Verify(x => x.PublishAsync(It.IsAny<ProposalStatusUpdated>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithZeroGuid_ShouldThrowArgumentException()
    {
        // Arrange
        var proposalId = Guid.Empty;
        var request = new UpdateProposalStatusRequest(proposalId, ProposalStatus.Approved, null);

        _mockProposalRepository
            .Setup(x => x.GetByIdAsync(proposalId))
            .ReturnsAsync((Proposal?)null);

        // Act & Assert
        var action = () => _useCase.ExecuteAsync(request);
        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage($"Proposal with ID {proposalId} not found");

        _mockProposalRepository.Verify(x => x.GetByIdAsync(proposalId), Times.Once);
        _mockProposalRepository.Verify(x => x.UpdateAsync(It.IsAny<Proposal>()), Times.Never);
        _mockEventPublisher.Verify(x => x.PublishAsync(It.IsAny<ProposalStatusUpdated>()), Times.Never);
    }

    [Theory]
    [InlineData(ProposalStatus.UnderReview)]
    [InlineData(ProposalStatus.Approved)]
    [InlineData(ProposalStatus.Rejected)]
    public async Task ExecuteAsync_WithAllStatuses_ShouldUpdateProposalStatus(ProposalStatus status)
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var request = new UpdateProposalStatusRequest(proposalId, status, null);
        var existingProposal = FakeDataGenerator.GenerateProposal();

        _mockProposalRepository
            .Setup(x => x.GetByIdAsync(proposalId))
            .ReturnsAsync(existingProposal);

        _mockProposalRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Proposal>()))
            .ReturnsAsync((Proposal p) => p);

        _mockEventPublisher
            .Setup(x => x.PublishAsync(It.IsAny<ProposalStatusUpdated>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(existingProposal.Id);
        result.Status.Should().Be(status);

        _mockProposalRepository.Verify(x => x.GetByIdAsync(proposalId), Times.Once);
        _mockProposalRepository.Verify(x => x.UpdateAsync(It.IsAny<Proposal>()), Times.Once);
        _mockEventPublisher.Verify(x => x.PublishAsync(It.IsAny<ProposalStatusUpdated>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldPublishCorrectEvent()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var request = new UpdateProposalStatusRequest(proposalId, ProposalStatus.Approved, null);
        var existingProposal = FakeDataGenerator.GenerateProposal();

        _mockProposalRepository
            .Setup(x => x.GetByIdAsync(proposalId))
            .ReturnsAsync(existingProposal);

        _mockProposalRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Proposal>()))
            .ReturnsAsync((Proposal p) => p);

        ProposalStatusUpdated? publishedEvent = null;
        _mockEventPublisher
            .Setup(x => x.PublishAsync(It.IsAny<ProposalStatusUpdated>()))
            .Callback<ProposalStatusUpdated>(e => publishedEvent = e)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        publishedEvent.Should().NotBeNull();
        publishedEvent!.ProposalId.Should().Be(existingProposal.Id);
        publishedEvent.Status.Should().Be("Approved");
        publishedEvent.RejectionReason.Should().BeNull();
        publishedEvent.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        _mockEventPublisher.Verify(x => x.PublishAsync(It.IsAny<ProposalStatusUpdated>()), Times.Once);
    }
} 