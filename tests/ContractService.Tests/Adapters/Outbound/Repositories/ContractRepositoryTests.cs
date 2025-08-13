using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ContractService.Domain.Entities;
using ContractService.Ports.Outbound;
using ContractService.Infrastructure.Data;
using ContractService.Adapters.Outbound.Repositories;

namespace ContractService.Tests.Adapters.Outbound.Repositories;

public class ContractRepositoryTests
{
    private readonly DbContextOptions<ContractDbContext> _options;
    private readonly ContractDbContext _context;
    private readonly IContractRepositoryPort _repository;

    public ContractRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<ContractDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ContractDbContext(_options);
        _repository = new ContractRepository(_context);
    }

    [Fact]
    public async Task AddAsync_WithValidContract_ShouldAddToDatabase()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var contract = new Contract(proposalId, "CTR-2024-001", 1200m);

        // Act
        var result = await _repository.AddAsync(contract);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.ProposalId.Should().Be(proposalId);
        result.ContractNumber.Should().Be("CTR-2024-001");
        result.PremiumAmount.Should().Be(1200m);

        // Verificar se foi salvo no banco
        var savedContract = await _context.Contracts.FindAsync(result.Id);
        savedContract.Should().NotBeNull();
        savedContract!.ProposalId.Should().Be(proposalId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenContractExists_ShouldReturnContract()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var contract = new Contract(proposalId, "CTR-2024-001", 1200m);
        var addedContract = await _repository.AddAsync(contract);

        // Act
        var result = await _repository.GetByIdAsync(addedContract.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(addedContract.Id);
        result.ProposalId.Should().Be(proposalId);
        result.ContractNumber.Should().Be("CTR-2024-001");
        result.PremiumAmount.Should().Be(1200m);
    }

    [Fact]
    public async Task GetByIdAsync_WhenContractDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_WhenContractsExist_ShouldReturnAllContracts()
    {
        // Arrange
        var contract1 = await _repository.AddAsync(new Contract(Guid.NewGuid(), "CTR-2024-001", 1200m));
        var contract2 = await _repository.AddAsync(new Contract(Guid.NewGuid(), "CTR-2024-002", 1500m));
        var contract3 = await _repository.AddAsync(new Contract(Guid.NewGuid(), "CTR-2024-003", 800m));

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(c => c.Id == contract1.Id);
        result.Should().Contain(c => c.Id == contract2.Id);
        result.Should().Contain(c => c.Id == contract3.Id);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoContractsExist_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByProposalIdAsync_WhenContractExists_ShouldReturnContract()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var contract = new Contract(proposalId, "CTR-2024-001", 1200m);
        await _repository.AddAsync(contract);

        // Act
        var result = await _repository.GetByProposalIdAsync(proposalId);

        // Assert
        result.Should().NotBeNull();
        result!.ProposalId.Should().Be(proposalId);
        result.ContractNumber.Should().Be("CTR-2024-001");
        result.PremiumAmount.Should().Be(1200m);
    }

    [Fact]
    public async Task GetByProposalIdAsync_WhenContractDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentProposalId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByProposalIdAsync(nonExistentProposalId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_WhenContractExists_ShouldReturnTrue()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var contract = new Contract(proposalId, "CTR-2024-001", 1200m);
        var addedContract = await _repository.AddAsync(contract);

        // Act
        var result = await _repository.ExistsAsync(addedContract.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenContractDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.ExistsAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
} 