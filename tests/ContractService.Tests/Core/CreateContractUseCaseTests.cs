using FluentAssertions;
using Moq;
using ContractService.Ports.Inbound;
using ContractService.Ports.Inbound.Shared;
using ContractService.Core.UseCases;
using ContractService.Domain.Entities;
using ContractService.Ports.Outbound;
using InsuranceSystem.Shared.Infrastructure.Messaging.Events;

namespace ContractService.Tests.Core;

public class CreateContractUseCaseTests
{
    private readonly Mock<IContractRepositoryPort> _mockContractRepository;
    private readonly Mock<IProposalServicePort> _mockProposalService;
    private readonly Mock<IContractNumberGeneratorPort> _mockContractNumberGenerator;
    private readonly Mock<IEventPublisherPort> _mockEventPublisher;
    private readonly CreateContractUseCase _useCase;

    public CreateContractUseCaseTests()
    {
        _mockContractRepository = new Mock<IContractRepositoryPort>();
        _mockProposalService = new Mock<IProposalServicePort>();
        _mockContractNumberGenerator = new Mock<IContractNumberGeneratorPort>();
        _mockEventPublisher = new Mock<IEventPublisherPort>();
        
        _useCase = new CreateContractUseCase(
            _mockContractRepository.Object, 
            _mockProposalService.Object, 
            _mockContractNumberGenerator.Object,
            _mockEventPublisher.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidData_ShouldCreateContract()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var contractNumber = "CTR-2024-001";
        var premiumAmount = 1200m;

        var request = new CreateContractRequest(proposalId);

        var proposalInfo = new ProposalInfo(
            proposalId,
            "João Silva",
            "joao@email.com",
            "Auto",
            50000m,
            1200m,
            "Approved"
        );

        var expectedContract = new Contract(proposalId, contractNumber, premiumAmount);

        _mockProposalService
            .Setup(x => x.GetProposalAsync(proposalId))
            .ReturnsAsync(proposalInfo);

        _mockContractRepository
            .Setup(x => x.GetByProposalIdAsync(proposalId))
            .ReturnsAsync((Contract?)null);

        _mockContractNumberGenerator
            .Setup(x => x.GenerateAsync())
            .ReturnsAsync(contractNumber);

        _mockContractRepository
            .Setup(x => x.AddAsync(It.IsAny<Contract>()))
            .ReturnsAsync(expectedContract);

        _mockEventPublisher
            .Setup(x => x.PublishAsync(It.IsAny<object>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.ProposalId.Should().Be(proposalId);
        result.ContractNumber.Should().Be(contractNumber);
        result.PremiumAmount.Should().Be(premiumAmount);
        result.ContractDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        _mockProposalService.Verify(x => x.GetProposalAsync(proposalId), Times.Once);
        _mockContractRepository.Verify(x => x.GetByProposalIdAsync(proposalId), Times.Once);
        _mockContractNumberGenerator.Verify(x => x.GenerateAsync(), Times.Once);
        _mockContractRepository.Verify(x => x.AddAsync(It.IsAny<Contract>()), Times.Once);
        _mockEventPublisher.Verify(x => x.PublishAsync(It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenProposalNotFound_ShouldThrowArgumentException()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var request = new CreateContractRequest(proposalId);

        _mockProposalService
            .Setup(x => x.GetProposalAsync(proposalId))
            .ReturnsAsync((ProposalInfo?)null);

        // Act & Assert
        var action = () => _useCase.ExecuteAsync(request);
        await action.Should().ThrowAsync<ArgumentException>().WithMessage("*Proposal not found*");

        _mockProposalService.Verify(x => x.GetProposalAsync(proposalId), Times.Once);
        _mockContractRepository.Verify(x => x.GetByProposalIdAsync(It.IsAny<Guid>()), Times.Never);
        _mockContractRepository.Verify(x => x.AddAsync(It.IsAny<Contract>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenProposalNotApproved_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var request = new CreateContractRequest(proposalId);

        var proposalInfo = new ProposalInfo(
            proposalId,
            "João Silva",
            "joao@email.com",
            "Auto",
            50000m,
            1200m,
            "UnderReview"
        );

        _mockProposalService
            .Setup(x => x.GetProposalAsync(proposalId))
            .ReturnsAsync(proposalInfo);

        // Act & Assert
        var action = () => _useCase.ExecuteAsync(request);
        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Cannot contract proposal*");

        _mockProposalService.Verify(x => x.GetProposalAsync(proposalId), Times.Once);
        _mockContractRepository.Verify(x => x.GetByProposalIdAsync(It.IsAny<Guid>()), Times.Never);
        _mockContractRepository.Verify(x => x.AddAsync(It.IsAny<Contract>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenContractAlreadyExists_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var request = new CreateContractRequest(proposalId);

        var proposalInfo = new ProposalInfo(
            proposalId,
            "João Silva",
            "joao@email.com",
            "Auto",
            50000m,
            1200m,
            "Approved"
        );

        var existingContract = new Contract(proposalId, "CTR-2024-001", 1200m);

        _mockProposalService
            .Setup(x => x.GetProposalAsync(proposalId))
            .ReturnsAsync(proposalInfo);

        _mockContractRepository
            .Setup(x => x.GetByProposalIdAsync(proposalId))
            .ReturnsAsync(existingContract);

        // Act & Assert
        var action = () => _useCase.ExecuteAsync(request);
        await action.Should().ThrowAsync<InvalidOperationException>().WithMessage("*Contract already exists*");

        _mockProposalService.Verify(x => x.GetProposalAsync(proposalId), Times.Once);
        _mockContractRepository.Verify(x => x.GetByProposalIdAsync(proposalId), Times.Once);
        _mockContractRepository.Verify(x => x.AddAsync(It.IsAny<Contract>()), Times.Never);
    }
} 