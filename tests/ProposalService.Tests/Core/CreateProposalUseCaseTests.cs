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

public class CreateProposalUseCaseTests
{
    private readonly Mock<IProposalRepositoryPort> _mockProposalRepository;
    private readonly Mock<IEventPublisherPort> _mockEventPublisher;
    private readonly CreateProposalUseCase _useCase;

    public CreateProposalUseCaseTests()
    {
        _mockProposalRepository = new Mock<IProposalRepositoryPort>();
        _mockEventPublisher = new Mock<IEventPublisherPort>();
        _useCase = new CreateProposalUseCase(_mockProposalRepository.Object, _mockEventPublisher.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateProposal()
    {
        // Arrange
        var request = FakeDataGenerator.GenerateCreateProposalRequest();

        var expectedProposal = new Proposal(
            request.CustomerName,
            request.CustomerEmail,
            request.InsuranceType,
            request.CoverageAmount,
            request.PremiumAmount
        );

        _mockProposalRepository
            .Setup(x => x.AddAsync(It.IsAny<Proposal>()))
            .ReturnsAsync(expectedProposal);

        _mockEventPublisher
            .Setup(x => x.PublishAsync(It.IsAny<object>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.CustomerName.Should().Be(request.CustomerName);
        result.CustomerEmail.Should().Be(request.CustomerEmail);
        result.InsuranceType.Should().Be(request.InsuranceType);
        result.CoverageAmount.Should().Be(request.CoverageAmount);
        result.PremiumAmount.Should().Be(request.PremiumAmount);
        result.Status.Should().Be(ProposalStatus.UnderReview);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        _mockProposalRepository.Verify(x => x.AddAsync(It.IsAny<Proposal>()), Times.Once);
        _mockEventPublisher.Verify(x => x.PublishAsync(It.IsAny<object>()), Times.Once);
    }

    [Theory]
    [InlineData("", "email@test.com", "Auto", 50000, 1200)]
    [InlineData("João", "", "Auto", 50000, 1200)]
    [InlineData("João", "email@test.com", "", 50000, 1200)]
    public async Task ExecuteAsync_WithInvalidData_ShouldThrowArgumentException(
        string customerName, string customerEmail, string insuranceType, decimal coverageAmount, decimal premiumAmount)
    {
        // Arrange
        var request = new CreateProposalRequest(customerName, customerEmail, insuranceType, coverageAmount, premiumAmount);

        // Act & Assert
        var action = () => _useCase.ExecuteAsync(request);
        await action.Should().ThrowAsync<ArgumentException>();

        _mockProposalRepository.Verify(x => x.AddAsync(It.IsAny<Proposal>()), Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public async Task ExecuteAsync_WithInvalidAmounts_ShouldThrowArgumentException(decimal amount)
    {
        // Arrange
        var request = new CreateProposalRequest("João", "joao@email.com", "Auto", amount, 1200m);

        // Act & Assert
        var action = () => _useCase.ExecuteAsync(request);
        await action.Should().ThrowAsync<ArgumentException>();

        _mockProposalRepository.Verify(x => x.AddAsync(It.IsAny<Proposal>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var request = FakeDataGenerator.GenerateCreateProposalRequest();
        var exception = new InvalidOperationException("Database connection failed");

        _mockProposalRepository
            .Setup(x => x.AddAsync(It.IsAny<Proposal>()))
            .ThrowsAsync(exception);

        // Act & Assert
        var action = () => _useCase.ExecuteAsync(request);
        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("Database connection failed");

        _mockProposalRepository.Verify(x => x.AddAsync(It.IsAny<Proposal>()), Times.Once);
        _mockEventPublisher.Verify(x => x.PublishAsync(It.IsAny<object>()), Times.Never);
    }
} 