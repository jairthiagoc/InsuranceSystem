using FluentAssertions;
using Moq;
using ContractService.Ports.Outbound;
using ContractService.Domain.Entities;
using ContractService.Tests.Helpers;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using ContractService.Adapters.Outbound.Repositories;

namespace ContractService.Tests.Adapters.Outbound.Repositories;

public class CachedContractRepositoryTests
{
    private readonly Mock<ContractRepository> _mockRepository;
    private readonly Mock<IDistributedCache> _mockCache;
    private readonly CachedContractRepository _cachedRepository;

    public CachedContractRepositoryTests()
    {
        _mockRepository = new Mock<ContractRepository>();
        _mockCache = new Mock<IDistributedCache>();
        _cachedRepository = new CachedContractRepository(_mockRepository.Object, _mockCache.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCacheHit_ShouldReturnCachedContract()
    {
        // Arrange
        var contractId = Guid.NewGuid();
        var contract = FakeDataGenerator.GenerateContract();
        var cachedJson = JsonSerializer.Serialize(contract);

        _mockCache
            .Setup(x => x.GetStringAsync($"contracts:{contractId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedJson);

        // Act
        var result = await _cachedRepository.GetByIdAsync(contractId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(contract.Id);
        result.ContractNumber.Should().Be(contract.ContractNumber);

        _mockCache.Verify(x => x.GetStringAsync($"contracts:{contractId}", It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(x => x.GetByIdAsync(contractId), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCacheMiss_ShouldReturnFromRepository()
    {
        // Arrange
        var contractId = Guid.NewGuid();
        var contract = FakeDataGenerator.GenerateContract();

        _mockCache
            .Setup(x => x.GetStringAsync($"contracts:{contractId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        _mockRepository
            .Setup(x => x.GetByIdAsync(contractId))
            .ReturnsAsync(contract);

        _mockCache
            .Setup(x => x.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _cachedRepository.GetByIdAsync(contractId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(contract.Id);

        _mockCache.Verify(x => x.GetStringAsync($"contracts:{contractId}", It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(x => x.GetByIdAsync(contractId), Times.Once);
        _mockCache.Verify(x => x.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRepositoryReturnsNull_ShouldReturnNull()
    {
        // Arrange
        var contractId = Guid.NewGuid();

        _mockCache
            .Setup(x => x.GetStringAsync($"contracts:{contractId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        _mockRepository
            .Setup(x => x.GetByIdAsync(contractId))
            .ReturnsAsync((Contract?)null);

        // Act
        var result = await _cachedRepository.GetByIdAsync(contractId);

        // Assert
        result.Should().BeNull();

        _mockCache.Verify(x => x.GetStringAsync($"contracts:{contractId}", It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(x => x.GetByIdAsync(contractId), Times.Once);
        _mockCache.Verify(x => x.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCacheThrowsException_ShouldFallbackToRepository()
    {
        // Arrange
        var contractId = Guid.NewGuid();
        var contract = FakeDataGenerator.GenerateContract();

        _mockCache
            .Setup(x => x.GetStringAsync($"contracts:{contractId}", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Cache error"));

        _mockRepository
            .Setup(x => x.GetByIdAsync(contractId))
            .ReturnsAsync(contract);

        // Act
        var result = await _cachedRepository.GetByIdAsync(contractId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(contract.Id);

        _mockCache.Verify(x => x.GetStringAsync($"contracts:{contractId}", It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(x => x.GetByIdAsync(contractId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCacheReturnsInvalidJson_ShouldFallbackToRepository()
    {
        // Arrange
        var contractId = Guid.NewGuid();
        var contract = FakeDataGenerator.GenerateContract();

        _mockCache
            .Setup(x => x.GetStringAsync($"contracts:{contractId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync("invalid json");

        _mockRepository
            .Setup(x => x.GetByIdAsync(contractId))
            .ReturnsAsync(contract);

        // Act
        var result = await _cachedRepository.GetByIdAsync(contractId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(contract.Id);

        _mockCache.Verify(x => x.GetStringAsync($"contracts:{contractId}", It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(x => x.GetByIdAsync(contractId), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenCacheHit_ShouldReturnCachedContracts()
    {
        // Arrange
        var contracts = FakeDataGenerator.GenerateContracts(3);
        var cachedJson = JsonSerializer.Serialize(contracts);

        _mockCache
            .Setup(x => x.GetStringAsync("contracts:all", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedJson);

        // Act
        var result = await _cachedRepository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);

        _mockCache.Verify(x => x.GetStringAsync("contracts:all", It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_WhenCacheMiss_ShouldReturnFromRepository()
    {
        // Arrange
        var contracts = FakeDataGenerator.GenerateContracts(3);

        _mockCache
            .Setup(x => x.GetStringAsync("contracts:all", It.IsAny<CancellationToken>()))
            .ReturnsAsync((string?)null);

        _mockRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(contracts);

        _mockCache
            .Setup(x => x.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _cachedRepository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);

        _mockCache.Verify(x => x.GetStringAsync("contracts:all", It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(x => x.GetAllAsync(), Times.Once);
        _mockCache.Verify(x => x.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ShouldAddToRepositoryAndInvalidateCache()
    {
        // Arrange
        var contract = FakeDataGenerator.GenerateContract();

        _mockRepository
            .Setup(x => x.AddAsync(contract))
            .ReturnsAsync(contract);

        _mockCache
            .Setup(x => x.RemoveAsync("contracts:all", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockCache
            .Setup(x => x.RemoveAsync($"contracts:{contract.Id}", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _cachedRepository.AddAsync(contract);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(contract.Id);

        _mockRepository.Verify(x => x.AddAsync(contract), Times.Once);
        _mockCache.Verify(x => x.RemoveAsync("contracts:all", It.IsAny<CancellationToken>()), Times.Once);
        _mockCache.Verify(x => x.RemoveAsync($"contracts:{contract.Id}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFromRepository()
    {
        // Arrange
        var contractId = Guid.NewGuid();

        _mockRepository
            .Setup(x => x.ExistsAsync(contractId))
            .ReturnsAsync(true);

        // Act
        var result = await _cachedRepository.ExistsAsync(contractId);

        // Assert
        result.Should().BeTrue();

        _mockRepository.Verify(x => x.ExistsAsync(contractId), Times.Once);
    }

    [Fact]
    public async Task GetByProposalIdAsync_ShouldReturnFromRepository()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var contract = FakeDataGenerator.GenerateContract();

        _mockRepository
            .Setup(x => x.GetByProposalIdAsync(proposalId))
            .ReturnsAsync(contract);

        // Act
        var result = await _cachedRepository.GetByProposalIdAsync(proposalId);

        // Assert
        result.Should().NotBeNull();
        result!.ProposalId.Should().Be(proposalId);

        _mockRepository.Verify(x => x.GetByProposalIdAsync(proposalId), Times.Once);
    }

    [Fact]
    public async Task GetByProposalIdAsync_WhenRepositoryReturnsNull_ShouldReturnNull()
    {
        // Arrange
        var proposalId = Guid.NewGuid();

        _mockRepository
            .Setup(x => x.GetByProposalIdAsync(proposalId))
            .ReturnsAsync((Contract?)null);

        // Act
        var result = await _cachedRepository.GetByProposalIdAsync(proposalId);

        // Assert
        result.Should().BeNull();

        _mockRepository.Verify(x => x.GetByProposalIdAsync(proposalId), Times.Once);
    }

    [Fact]
    public async Task AddAsync_WhenCacheThrowsException_ShouldStillAddToRepository()
    {
        // Arrange
        var contract = FakeDataGenerator.GenerateContract();

        _mockRepository
            .Setup(x => x.AddAsync(contract))
            .ReturnsAsync(contract);

        _mockCache
            .Setup(x => x.RemoveAsync("contracts:all", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Cache error"));

        // Act
        var result = await _cachedRepository.AddAsync(contract);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(contract.Id);

        _mockRepository.Verify(x => x.AddAsync(contract), Times.Once);
        _mockCache.Verify(x => x.RemoveAsync("contracts:all", It.IsAny<CancellationToken>()), Times.Once);
    }
} 