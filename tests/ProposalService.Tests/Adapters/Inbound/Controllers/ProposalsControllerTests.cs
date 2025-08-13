using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProposalService.Ports.Inbound;
using ProposalService.Ports.Inbound.Shared;
using ProposalService.Adapters.Inbound.Controllers;
using ProposalService.Domain.Enums;
using ProposalService.Tests.Helpers;

namespace ProposalService.Tests.Adapters.Inbound.Controllers;

public class ProposalsControllerTests
{
    private readonly Mock<ICreateProposalPort> _mockCreateProposalPort;
    private readonly Mock<IGetProposalsPort> _mockGetProposalsPort;
    private readonly Mock<IGetProposalByIdPort> _mockGetProposalByIdPort;
    private readonly Mock<IGetProposalsByStatusPort> _mockGetProposalsByStatusPort;
    private readonly Mock<IUpdateProposalStatusPort> _mockUpdateProposalStatusPort;
    private readonly ProposalsController _controller;

    public ProposalsControllerTests()
    {
        _mockCreateProposalPort = new Mock<ICreateProposalPort>();
        _mockGetProposalsPort = new Mock<IGetProposalsPort>();
        _mockGetProposalByIdPort = new Mock<IGetProposalByIdPort>();
        _mockGetProposalsByStatusPort = new Mock<IGetProposalsByStatusPort>();
        _mockUpdateProposalStatusPort = new Mock<IUpdateProposalStatusPort>();
        
        _controller = new ProposalsController(
            _mockCreateProposalPort.Object,
            _mockGetProposalsPort.Object,
            _mockGetProposalByIdPort.Object,
            _mockGetProposalsByStatusPort.Object,
            _mockUpdateProposalStatusPort.Object
        );
    }

    [Fact]
    public async Task CreateProposal_WithValidData_ShouldReturnCreatedResult()
    {
        // Arrange
        var request = FakeDataGenerator.GenerateCreateProposalRequest();
        var expectedResult = FakeDataGenerator.GenerateProposal();

        _mockCreateProposalPort
            .Setup(x => x.ExecuteAsync(request))
            .ReturnsAsync(new ProposalResult(
                expectedResult.Id,
                expectedResult.CustomerName,
                expectedResult.CustomerEmail,
                expectedResult.InsuranceType,
                expectedResult.CoverageAmount,
                expectedResult.PremiumAmount,
                expectedResult.Status,
                expectedResult.CreatedAt,
                expectedResult.UpdatedAt,
                expectedResult.RejectionReason
            ));

        // Act
        var result = await _controller.CreateProposal(request);

        // Assert
        result.Should().NotBeNull();
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.Value.Should().BeOfType<ProposalResult>();
        createdResult.ActionName.Should().Be(nameof(ProposalsController.GetProposal));
        createdResult.RouteValues.Should().ContainKey("id");
        createdResult.RouteValues["id"].Should().Be(expectedResult.Id);

        _mockCreateProposalPort.Verify(x => x.ExecuteAsync(request), Times.Once);
    }

    [Fact]
    public async Task CreateProposal_WhenArgumentException_ShouldReturnBadRequest()
    {
        // Arrange
        var request = FakeDataGenerator.GenerateCreateProposalRequest();
        var errorMessage = "Invalid proposal data";

        _mockCreateProposalPort
            .Setup(x => x.ExecuteAsync(request))
            .ThrowsAsync(new ArgumentException(errorMessage));

        // Act
        var result = await _controller.CreateProposal(request);

        // Assert
        result.Should().NotBeNull();
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);

        _mockCreateProposalPort.Verify(x => x.ExecuteAsync(request), Times.Once);
    }

    [Fact]
    public async Task GetProposals_ShouldReturnOkResult()
    {
        // Arrange
        var proposals = FakeDataGenerator.GenerateProposals(3);
        var proposalResults = proposals.Select(p => new ProposalResult(
            p.Id, p.CustomerName, p.CustomerEmail, p.InsuranceType,
            p.CoverageAmount, p.PremiumAmount, p.Status, p.CreatedAt, p.UpdatedAt, p.RejectionReason
        )).ToList();

        _mockGetProposalsPort
            .Setup(x => x.ExecuteAsync())
            .ReturnsAsync(proposalResults);

        // Act
        var result = await _controller.GetProposals();

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeAssignableTo<IEnumerable<ProposalResult>>();

        _mockGetProposalsPort.Verify(x => x.ExecuteAsync(), Times.Once);
    }

    [Fact]
    public async Task GetProposals_WithStatusFilter_ShouldReturnFilteredProposals()
    {
        // Arrange
        var statusFilter = "Approved";
        var proposals = FakeDataGenerator.GenerateProposals(2);
        var proposalResults = proposals.Select(p => new ProposalResult(
            p.Id, p.CustomerName, p.CustomerEmail, p.InsuranceType,
            p.CoverageAmount, p.PremiumAmount, p.Status, p.CreatedAt, p.UpdatedAt, p.RejectionReason
        )).ToList();

        _mockGetProposalsByStatusPort
            .Setup(x => x.ExecuteAsync(ProposalStatus.Approved))
            .ReturnsAsync(proposalResults);

        // Act
        var result = await _controller.GetProposals(statusFilter);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeAssignableTo<IEnumerable<ProposalResult>>();

        _mockGetProposalsByStatusPort.Verify(x => x.ExecuteAsync(ProposalStatus.Approved), Times.Once);
    }

    [Fact]
    public async Task GetProposal_WhenProposalExists_ShouldReturnOkResult()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var proposal = FakeDataGenerator.GenerateProposal();
        var proposalResult = new ProposalResult(
            proposal.Id, proposal.CustomerName, proposal.CustomerEmail, proposal.InsuranceType,
            proposal.CoverageAmount, proposal.PremiumAmount, proposal.Status, proposal.CreatedAt, proposal.UpdatedAt, proposal.RejectionReason
        );

        _mockGetProposalByIdPort
            .Setup(x => x.ExecuteAsync(proposalId))
            .ReturnsAsync(proposalResult);

        // Act
        var result = await _controller.GetProposal(proposalId);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeOfType<ProposalResult>();

        _mockGetProposalByIdPort.Verify(x => x.ExecuteAsync(proposalId), Times.Once);
    }

    [Fact]
    public async Task GetProposal_WhenProposalDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var proposalId = Guid.NewGuid();

        _mockGetProposalByIdPort
            .Setup(x => x.ExecuteAsync(proposalId))
            .ReturnsAsync((ProposalResult?)null);

        // Act
        var result = await _controller.GetProposal(proposalId);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<NotFoundResult>();

        _mockGetProposalByIdPort.Verify(x => x.ExecuteAsync(proposalId), Times.Once);
    }

    [Fact]
    public async Task UpdateProposalStatus_WithValidData_ShouldReturnOkResult()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var request = new UpdateProposalStatusRequest(proposalId, ProposalStatus.Approved);
        var proposal = FakeDataGenerator.GenerateProposal();
        var proposalResult = new ProposalResult(
            proposal.Id, proposal.CustomerName, proposal.CustomerEmail, proposal.InsuranceType,
            proposal.CoverageAmount, proposal.PremiumAmount, proposal.Status, proposal.CreatedAt, proposal.UpdatedAt, proposal.RejectionReason
        );

        _mockUpdateProposalStatusPort
            .Setup(x => x.ExecuteAsync(request))
            .ReturnsAsync(proposalResult);

        // Act
        var result = await _controller.UpdateProposalStatus(proposalId, request);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeOfType<ProposalResult>();

        _mockUpdateProposalStatusPort.Verify(x => x.ExecuteAsync(request), Times.Once);
    }

    [Fact]
    public async Task UpdateProposalStatus_WhenArgumentException_ShouldReturnBadRequest()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var request = new UpdateProposalStatusRequest(proposalId, ProposalStatus.Approved);
        var errorMessage = "Proposal not found";

        _mockUpdateProposalStatusPort
            .Setup(x => x.ExecuteAsync(request))
            .ThrowsAsync(new ArgumentException(errorMessage));

        // Act
        var result = await _controller.UpdateProposalStatus(proposalId, request);

        // Assert
        result.Should().NotBeNull();
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequestResult.Value.Should().Be(errorMessage);

        _mockUpdateProposalStatusPort.Verify(x => x.ExecuteAsync(request), Times.Once);
    }
} 