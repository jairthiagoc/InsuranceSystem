using FluentAssertions;
using Moq;
using ContractService.Ports.Inbound;
using ContractService.Ports.Inbound.Shared;
using ContractService.Core.UseCases;
using ContractService.Domain.Entities;
using ContractService.Ports.Outbound;

namespace ContractService.Tests.Core;

public class GetContractsUseCaseTests
{
    private readonly Mock<IContractRepositoryPort> _mockContractRepository;
    private readonly GetContractsUseCase _useCase;

    public GetContractsUseCaseTests()
    {
        _mockContractRepository = new Mock<IContractRepositoryPort>();
        _useCase = new GetContractsUseCase(_mockContractRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenContractsExist_ShouldReturnAllContracts()
    {
        // Arrange
        var contract1 = new Contract(Guid.NewGuid(), "CTR-2024-001", 1200m);
        var contract2 = new Contract(Guid.NewGuid(), "CTR-2024-002", 1500m);
        var contract3 = new Contract(Guid.NewGuid(), "CTR-2024-003", 800m);

        var contracts = new List<Contract> { contract1, contract2, contract3 };

        _mockContractRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(contracts);

        // Act
        var result = await _useCase.ExecuteAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);

        var resultList = result.ToList();
        resultList[0].Id.Should().Be(contract1.Id);
        resultList[0].ContractNumber.Should().Be("CTR-2024-001");
        resultList[0].PremiumAmount.Should().Be(1200m);

        resultList[1].Id.Should().Be(contract2.Id);
        resultList[1].ContractNumber.Should().Be("CTR-2024-002");
        resultList[1].PremiumAmount.Should().Be(1500m);

        resultList[2].Id.Should().Be(contract3.Id);
        resultList[2].ContractNumber.Should().Be("CTR-2024-003");
        resultList[2].PremiumAmount.Should().Be(800m);

        _mockContractRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoContractsExist_ShouldReturnEmptyList()
    {
        // Arrange
        _mockContractRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Contract>());

        // Act
        var result = await _useCase.ExecuteAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _mockContractRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepositoryThrowsException_ShouldPropagateException()
    {
        // Arrange
        var exception = new InvalidOperationException("Database connection failed");
        _mockContractRepository
            .Setup(x => x.GetAllAsync())
            .ThrowsAsync(exception);

        // Act & Assert
        var action = () => _useCase.ExecuteAsync();
        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("Database connection failed");

        _mockContractRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }
} 