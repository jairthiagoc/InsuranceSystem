using FluentAssertions;
using Moq;
using ProposalService.Ports.Inbound;
using ProposalService.Ports.Inbound.Shared;
using ProposalService.Core.UseCases;
using ProposalService.Domain.Entities;
using ProposalService.Ports.Outbound;
using ProposalService.Tests.Helpers;

namespace ProposalService.Tests.Core;

public class GetProposalByIdUseCaseTests
{
    private readonly Mock<IProposalRepositoryPort> _mockProposalRepository;
    private readonly GetProposalByIdUseCase _useCase;

    public GetProposalByIdUseCaseTests()
    {
        _mockProposalRepository = new Mock<IProposalRepositoryPort>();
        _useCase = new GetProposalByIdUseCase(_mockProposalRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenProposalExists_ShouldReturnProposal()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var proposal = FakeDataGenerator.GenerateProposal();

        _mockProposalRepository
            .Setup(x => x.GetByIdAsync(proposalId))
            .ReturnsAsync(proposal);

        // Act
        var result = await _useCase.ExecuteAsync(proposalId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(proposal.Id);
        result.CustomerName.Should().Be(proposal.CustomerName);
        result.CustomerEmail.Should().Be(proposal.CustomerEmail);
        result.InsuranceType.Should().Be(proposal.InsuranceType);
        result.CoverageAmount.Should().Be(proposal.CoverageAmount);
        result.PremiumAmount.Should().Be(proposal.PremiumAmount);
        result.Status.Should().Be(proposal.Status);

        _mockProposalRepository.Verify(x => x.GetByIdAsync(proposalId), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenProposalDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var proposalId = Guid.NewGuid();

        _mockProposalRepository
            .Setup(x => x.GetByIdAsync(proposalId))
            .ReturnsAsync((Proposal?)null);

        // Act
        var result = await _useCase.ExecuteAsync(proposalId);

        // Assert
        result.Should().BeNull();

        _mockProposalRepository.Verify(x => x.GetByIdAsync(proposalId), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var exception = new InvalidOperationException("Database connection failed");

        _mockProposalRepository
            .Setup(x => x.GetByIdAsync(proposalId))
            .ThrowsAsync(exception);

        // Act & Assert
        var action = () => _useCase.ExecuteAsync(proposalId);
        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("Database connection failed");

        _mockProposalRepository.Verify(x => x.GetByIdAsync(proposalId), Times.Once);
    }
} 