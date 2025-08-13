using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ContractService.Ports.Inbound;
using ContractService.Ports.Inbound.Shared;
using ContractService.Adapters.Inbound.Controllers;

namespace ContractService.Tests.Adapters.Inbound.Controllers;

public class ContractsControllerTests
{
    private readonly Mock<ICreateContractPort> _mockCreateContractPort;
    private readonly Mock<IGetContractsPort> _mockGetContractsPort;
    private readonly Mock<IGetContractByIdPort> _mockGetContractByIdPort;
    private readonly Mock<IGetContractByProposalIdPort> _mockGetContractByProposalIdPort;
    private readonly ContractsController _controller;

    public ContractsControllerTests()
    {
        _mockCreateContractPort = new Mock<ICreateContractPort>();
        _mockGetContractsPort = new Mock<IGetContractsPort>();
        _mockGetContractByIdPort = new Mock<IGetContractByIdPort>();
        _mockGetContractByProposalIdPort = new Mock<IGetContractByProposalIdPort>();
        
        _controller = new ContractsController(
            _mockCreateContractPort.Object,
            _mockGetContractsPort.Object,
            _mockGetContractByIdPort.Object,
            _mockGetContractByProposalIdPort.Object
        );
    }

    [Fact]
    public async Task CreateContract_WithValidData_ShouldReturnCreatedResult()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var contractId = Guid.NewGuid();
        var request = new CreateContractRequest(proposalId);
        
        var expectedResult = new ContractResult(
            contractId,
            proposalId,
            DateTime.UtcNow,
            "CTR-2024-001",
            1200m,
            DateTime.UtcNow
        );

        _mockCreateContractPort
            .Setup(x => x.ExecuteAsync(request))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.CreateContract(request);

        // Assert
        result.Should().NotBeNull();
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.Value.Should().BeOfType<ContractResult>();
        createdResult.ActionName.Should().Be(nameof(ContractsController.GetContract));
        createdResult.RouteValues.Should().ContainKey("id");
        createdResult.RouteValues["id"].Should().Be(contractId);

        _mockCreateContractPort.Verify(x => x.ExecuteAsync(request), Times.Once);
    }

    [Fact]
    public async Task CreateContract_WhenArgumentException_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateContractRequest(Guid.NewGuid());
        var errorMessage = "Proposal not found";

        _mockCreateContractPort
            .Setup(x => x.ExecuteAsync(request))
            .ThrowsAsync(new ArgumentException(errorMessage));

        // Act
        var result = await _controller.CreateContract(request);

        // Assert
        result.Should().NotBeNull();
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);

        _mockCreateContractPort.Verify(x => x.ExecuteAsync(request), Times.Once);
    }

    [Fact]
    public async Task CreateContract_WhenInvalidOperationException_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateContractRequest(Guid.NewGuid());
        var errorMessage = "Contract already exists";

        _mockCreateContractPort
            .Setup(x => x.ExecuteAsync(request))
            .ThrowsAsync(new InvalidOperationException(errorMessage));

        // Act
        var result = await _controller.CreateContract(request);

        // Assert
        result.Should().NotBeNull();
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);

        _mockCreateContractPort.Verify(x => x.ExecuteAsync(request), Times.Once);
    }

    [Fact]
    public async Task GetContracts_ShouldReturnOkResult()
    {
        // Arrange
        var contracts = new List<ContractResult>
        {
            new(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, "CTR-2024-001", 1200m, DateTime.UtcNow),
            new(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, "CTR-2024-002", 1500m, DateTime.UtcNow)
        };

        _mockGetContractsPort
            .Setup(x => x.ExecuteAsync())
            .ReturnsAsync(contracts);

        // Act
        var result = await _controller.GetContracts();

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeOfType<List<ContractResult>>();

        _mockGetContractsPort.Verify(x => x.ExecuteAsync(), Times.Once);
    }

    [Fact]
    public async Task GetContract_WhenContractExists_ShouldReturnOkResult()
    {
        // Arrange
        var contractId = Guid.NewGuid();
        var contract = new ContractResult(
            contractId,
            Guid.NewGuid(),
            DateTime.UtcNow,
            "CTR-2024-001",
            1200m,
            DateTime.UtcNow
        );

        _mockGetContractByIdPort
            .Setup(x => x.ExecuteAsync(contractId))
            .ReturnsAsync(contract);

        // Act
        var result = await _controller.GetContract(contractId);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeOfType<ContractResult>();

        _mockGetContractByIdPort.Verify(x => x.ExecuteAsync(contractId), Times.Once);
    }

    [Fact]
    public async Task GetContract_WhenContractDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var contractId = Guid.NewGuid();

        _mockGetContractByIdPort
            .Setup(x => x.ExecuteAsync(contractId))
            .ReturnsAsync((ContractResult?)null);

        // Act
        var result = await _controller.GetContract(contractId);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<NotFoundResult>();

        _mockGetContractByIdPort.Verify(x => x.ExecuteAsync(contractId), Times.Once);
    }

    [Fact]
    public async Task GetContractByProposalId_WhenContractExists_ShouldReturnOkResult()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var contract = new ContractResult(
            Guid.NewGuid(),
            proposalId,
            DateTime.UtcNow,
            "CTR-2024-001",
            1200m,
            DateTime.UtcNow
        );

        _mockGetContractByProposalIdPort
            .Setup(x => x.ExecuteAsync(proposalId))
            .ReturnsAsync(contract);

        // Act
        var result = await _controller.GetContractByProposalId(proposalId);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeOfType<ContractResult>();

        _mockGetContractByProposalIdPort.Verify(x => x.ExecuteAsync(proposalId), Times.Once);
    }

    [Fact]
    public async Task GetContractByProposalId_WhenContractDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var proposalId = Guid.NewGuid();

        _mockGetContractByProposalIdPort
            .Setup(x => x.ExecuteAsync(proposalId))
            .ReturnsAsync((ContractResult?)null);

        // Act
        var result = await _controller.GetContractByProposalId(proposalId);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<NotFoundResult>();

        _mockGetContractByProposalIdPort.Verify(x => x.ExecuteAsync(proposalId), Times.Once);
    }
} 