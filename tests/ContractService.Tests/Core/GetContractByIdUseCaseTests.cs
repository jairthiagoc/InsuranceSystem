using FluentAssertions;
using Moq;
using ContractService.Ports.Inbound;
using ContractService.Ports.Inbound.Shared;
using ContractService.Core.UseCases;
using ContractService.Domain.Entities;
using ContractService.Ports.Outbound;

namespace ContractService.Tests.Core;

public class GetContractByIdUseCaseTests
{
    private readonly Mock<IContractRepositoryPort> _mockContractRepository;
    private readonly GetContractByIdUseCase _useCase;

    public GetContractByIdUseCaseTests()
    {
        _mockContractRepository = new Mock<IContractRepositoryPort>();
        _useCase = new GetContractByIdUseCase(_mockContractRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenContractExists_ShouldReturnContract()
    {
        // Arrange
        var contractId = Guid.NewGuid();
        var proposalId = Guid.NewGuid();
        var contract = new Contract(proposalId, "CTR-2024-001", 1200m);

        _mockContractRepository
            .Setup(x => x.GetByIdAsync(contractId))
            .ReturnsAsync(contract);

        // Act
        var result = await _useCase.ExecuteAsync(contractId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(contract.Id);
        result.ProposalId.Should().Be(proposalId);
        result.ContractNumber.Should().Be("CTR-2024-001");
        result.PremiumAmount.Should().Be(1200m);

        _mockContractRepository.Verify(x => x.GetByIdAsync(contractId), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenContractDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var contractId = Guid.NewGuid();

        _mockContractRepository
            .Setup(x => x.GetByIdAsync(contractId))
            .ReturnsAsync((Contract?)null);

        // Act
        var result = await _useCase.ExecuteAsync(contractId);

        // Assert
        result.Should().BeNull();

        _mockContractRepository.Verify(x => x.GetByIdAsync(contractId), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var contractId = Guid.NewGuid();
        var exception = new InvalidOperationException("Database connection failed");

        _mockContractRepository
            .Setup(x => x.GetByIdAsync(contractId))
            .ThrowsAsync(exception);

        // Act & Assert
        var action = () => _useCase.ExecuteAsync(contractId);
        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("Database connection failed");

        _mockContractRepository.Verify(x => x.GetByIdAsync(contractId), Times.Once);
    }
} 