using FluentAssertions;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Enums;
using ProposalService.Domain.Exceptions;
using ProposalService.Tests.Helpers;

namespace ProposalService.Tests.Domain;

public class ProposalTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateProposal()
    {
        // Arrange & Act
        var proposal = FakeDataGenerator.GenerateProposal();

        // Assert
        proposal.Id.Should().NotBeEmpty();
        proposal.CustomerName.Should().NotBeNullOrEmpty();
        proposal.CustomerEmail.Should().NotBeNullOrEmpty();
        proposal.InsuranceType.Should().NotBeNullOrEmpty();
        proposal.CoverageAmount.Should().BeGreaterThan(0);
        proposal.PremiumAmount.Should().BeGreaterThan(0);
        proposal.Status.Should().Be(ProposalStatus.UnderReview);
        proposal.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        proposal.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        proposal.RejectionReason.Should().BeNull();
    }

    [Theory]
    [InlineData("", "email@test.com", "Auto", 50000, 1200)]
    [InlineData("João", "", "Auto", 50000, 1200)]
    [InlineData("João", "email@test.com", "", 50000, 1200)]
    public void Constructor_WithInvalidData_ShouldThrowArgumentException(
        string customerName, string customerEmail, string insuranceType, decimal coverageAmount, decimal premiumAmount)
    {
        // Act & Assert
        var action = () => new Proposal(customerName, customerEmail, insuranceType, coverageAmount, premiumAmount);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Constructor_WithInvalidAmounts_ShouldThrowArgumentException(decimal amount)
    {
        // Act & Assert
        var action = () => new Proposal("João", "joao@email.com", "Auto", amount, 1200m);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Approve_WhenUnderReview_ShouldChangeStatusToApproved()
    {
        // Arrange
        var proposal = FakeDataGenerator.GenerateProposal();

        // Act
        proposal.Approve();

        // Assert
        proposal.Status.Should().Be(ProposalStatus.Approved);
        proposal.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Approve_WhenNotUnderReview_ShouldThrowException()
    {
        // Arrange
        var proposal = FakeDataGenerator.GenerateProposal();
        proposal.Approve(); // Aprova primeiro

        // Act & Assert
        var action = () => proposal.Approve();
        action.Should().Throw<InvalidProposalStatusException>();
    }

    [Fact]
    public void Reject_WhenUnderReview_ShouldChangeStatusToRejected()
    {
        // Arrange
        var proposal = FakeDataGenerator.GenerateProposal();
        var rejectionReason = "Documentação incompleta";

        // Act
        proposal.Reject(rejectionReason);

        // Assert
        proposal.Status.Should().Be(ProposalStatus.Rejected);
        proposal.RejectionReason.Should().Be(rejectionReason);
        proposal.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Reject_WhenNotUnderReview_ShouldThrowException()
    {
        // Arrange
        var proposal = FakeDataGenerator.GenerateProposal();
        proposal.Approve(); // Aprova primeiro

        // Act & Assert
        var action = () => proposal.Reject("Motivo");
        action.Should().Throw<InvalidProposalStatusException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Reject_WithInvalidReason_ShouldThrowException(string reason)
    {
        // Arrange
        var proposal = FakeDataGenerator.GenerateProposal();

        // Act & Assert
        var action = () => proposal.Reject(reason);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CanBeContracted_WhenApproved_ShouldReturnTrue()
    {
        // Arrange
        var proposal = FakeDataGenerator.GenerateProposal();
        proposal.Approve();

        // Act & Assert
        proposal.CanBeContracted.Should().BeTrue();
    }

    [Theory]
    [InlineData(ProposalStatus.UnderReview)]
    [InlineData(ProposalStatus.Rejected)]
    public void CanBeContracted_WhenNotApproved_ShouldReturnFalse(ProposalStatus status)
    {
        // Arrange
        var proposal = FakeDataGenerator.GenerateProposal();
        
        switch (status)
        {
            case ProposalStatus.UnderReview:
                // Já está em UnderReview por padrão
                break;
            case ProposalStatus.Approved:
                proposal.Approve();
                break;
            case ProposalStatus.Rejected:
                proposal.Reject("Motivo");
                break;
        }

        // Act & Assert
        proposal.CanBeContracted.Should().BeFalse();
    }

    [Fact]
    public void UpdateDetails_WithValidData_ShouldUpdateProposal()
    {
        // Arrange
        var proposal = FakeDataGenerator.GenerateProposal();
        var originalUpdatedAt = proposal.UpdatedAt;
        var newData = FakeDataGenerator.GenerateProposal();

        // Act
        proposal.UpdateDetails(
            newData.CustomerName,
            newData.CustomerEmail,
            newData.InsuranceType,
            newData.CoverageAmount,
            newData.PremiumAmount
        );

        // Assert
        proposal.CustomerName.Should().Be(newData.CustomerName);
        proposal.CustomerEmail.Should().Be(newData.CustomerEmail);
        proposal.InsuranceType.Should().Be(newData.InsuranceType);
        proposal.CoverageAmount.Should().Be(newData.CoverageAmount);
        proposal.PremiumAmount.Should().Be(newData.PremiumAmount);
        proposal.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void UpdateStatus_WithApprovedStatus_ShouldCallApprove()
    {
        // Arrange
        var proposal = FakeDataGenerator.GenerateProposal();

        // Act
        proposal.UpdateStatus(ProposalStatus.Approved);

        // Assert
        proposal.Status.Should().Be(ProposalStatus.Approved);
    }

    [Fact]
    public void UpdateStatus_WithRejectedStatus_ShouldCallReject()
    {
        // Arrange
        var proposal = FakeDataGenerator.GenerateProposal();
        var rejectionReason = "Documentação insuficiente";

        // Act
        proposal.UpdateStatus(ProposalStatus.Rejected, rejectionReason);

        // Assert
        proposal.Status.Should().Be(ProposalStatus.Rejected);
        proposal.RejectionReason.Should().Be(rejectionReason);
    }

    [Fact]
    public void UpdateStatus_WithOtherStatus_ShouldUpdateStatus()
    {
        // Arrange
        var proposal = FakeDataGenerator.GenerateProposal();
        var originalUpdatedAt = proposal.UpdatedAt;

        // Act
        proposal.UpdateStatus(ProposalStatus.UnderReview);

        // Assert
        proposal.Status.Should().Be(ProposalStatus.UnderReview);
        proposal.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }
} 