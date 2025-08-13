using FluentAssertions;
using Moq;
using ProposalService.Ports.Inbound;
using ProposalService.Ports.Inbound.Shared;
using ProposalService.Core.UseCases;
using ProposalService.Domain.Entities;
using ProposalService.Ports.Outbound;
using ProposalService.Tests.Helpers;

namespace ProposalService.Tests.Core;

public class GetProposalsUseCaseTests
{
    private readonly Mock<IProposalRepositoryPort> _mockProposalRepository;
    private readonly GetProposalsUseCase _useCase;

    public GetProposalsUseCaseTests()
    {
        _mockProposalRepository = new Mock<IProposalRepositoryPort>();
        _useCase = new GetProposalsUseCase(_mockProposalRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenProposalsExist_ShouldReturnAllProposals()
    {
        // Arrange
        var proposals = FakeDataGenerator.GenerateProposals(3);

        _mockProposalRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(proposals);

        // Act
        var result = await _useCase.ExecuteAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);

        _mockProposalRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoProposalsExist_ShouldReturnEmptyList()
    {
        // Arrange
        _mockProposalRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Proposal>());

        // Act
        var result = await _useCase.ExecuteAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _mockProposalRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var exception = new InvalidOperationException("Database connection failed");
        _mockProposalRepository
            .Setup(x => x.GetAllAsync())
            .ThrowsAsync(exception);

        // Act & Assert
        var action = () => _useCase.ExecuteAsync();
        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("Database connection failed");

        _mockProposalRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }
} 