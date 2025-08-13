using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Enums;
using ProposalService.Ports.Outbound;
using ProposalService.Infrastructure.Data;
using ProposalService.Adapters.Outbound.Repositories;
using ProposalService.Tests.Helpers;

namespace ProposalService.Tests.Adapters.Outbound.Repositories;

public class ProposalRepositoryTests
{
    private readonly DbContextOptions<ProposalDbContext> _options;
    private readonly ProposalDbContext _context;
    private readonly IProposalRepositoryPort _repository;

    public ProposalRepositoryTests()
    {
        _options = new DbContextOptionsBuilder<ProposalDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ProposalDbContext(_options);
        _repository = new ProposalRepository(_context);
    }

    [Fact]
    public async Task AddAsync_WithValidProposal_ShouldAddToDatabase()
    {
        // Arrange
        var proposal = FakeDataGenerator.GenerateProposal();

        // Act
        var result = await _repository.AddAsync(proposal);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CustomerName.Should().Be(proposal.CustomerName);
        result.CustomerEmail.Should().Be(proposal.CustomerEmail);
        result.InsuranceType.Should().Be(proposal.InsuranceType);
        result.CoverageAmount.Should().Be(proposal.CoverageAmount);
        result.PremiumAmount.Should().Be(proposal.PremiumAmount);
        result.Status.Should().Be(ProposalStatus.UnderReview);

        // Verificar se foi salvo no banco
        var savedProposal = await _context.Proposals.FindAsync(result.Id);
        savedProposal.Should().NotBeNull();
        savedProposal!.CustomerName.Should().Be(proposal.CustomerName);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProposalExists_ShouldReturnProposal()
    {
        // Arrange
        var proposal = FakeDataGenerator.GenerateProposal();
        var addedProposal = await _repository.AddAsync(proposal);

        // Act
        var result = await _repository.GetByIdAsync(addedProposal.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(addedProposal.Id);
        result.CustomerName.Should().Be(proposal.CustomerName);
        result.CustomerEmail.Should().Be(proposal.CustomerEmail);
        result.InsuranceType.Should().Be(proposal.InsuranceType);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProposalDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_WhenProposalsExist_ShouldReturnAllProposals()
    {
        // Arrange
        var proposal1 = await _repository.AddAsync(FakeDataGenerator.GenerateProposal());
        var proposal2 = await _repository.AddAsync(FakeDataGenerator.GenerateProposal());
        var proposal3 = await _repository.AddAsync(FakeDataGenerator.GenerateProposal());

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(p => p.Id == proposal1.Id);
        result.Should().Contain(p => p.Id == proposal2.Id);
        result.Should().Contain(p => p.Id == proposal3.Id);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoProposalsExist_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByStatusAsync_WhenProposalsExist_ShouldReturnFilteredProposals()
    {
        // Arrange
        var proposal1 = FakeDataGenerator.GenerateProposal();
        proposal1.Approve();
        var proposal2 = FakeDataGenerator.GenerateProposal();
        proposal2.Approve();
        var proposal3 = FakeDataGenerator.GenerateProposal(); // UnderReview

        await _repository.AddAsync(proposal1);
        await _repository.AddAsync(proposal2);
        await _repository.AddAsync(proposal3);

        // Act
        var result = await _repository.GetByStatusAsync(ProposalStatus.Approved);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().OnlyContain(p => p.Status == ProposalStatus.Approved);
    }

    [Fact]
    public async Task UpdateAsync_WithValidProposal_ShouldUpdateInDatabase()
    {
        // Arrange
        var proposal = FakeDataGenerator.GenerateProposal();
        var addedProposal = await _repository.AddAsync(proposal);
        
        addedProposal.Approve();

        // Act
        var result = await _repository.UpdateAsync(addedProposal);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ProposalStatus.Approved);

        // Verificar se foi atualizado no banco
        var updatedProposal = await _context.Proposals.FindAsync(result.Id);
        updatedProposal.Should().NotBeNull();
        updatedProposal!.Status.Should().Be(ProposalStatus.Approved);
    }

    [Fact]
    public async Task ExistsAsync_WhenProposalExists_ShouldReturnTrue()
    {
        // Arrange
        var proposal = FakeDataGenerator.GenerateProposal();
        var addedProposal = await _repository.AddAsync(proposal);

        // Act
        var result = await _repository.ExistsAsync(addedProposal.Id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenProposalDoesNotExist_ShouldReturnFalse()
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