using FluentAssertions;
using InsuranceSystem.Shared.Infrastructure.Messaging.Events;
using ProposalService.Tests.Helpers;

namespace ProposalService.Tests.Shared.Events;

public class ProposalCreatedTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateEvent()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var customerName = "João Silva";
        var customerEmail = "joao@email.com";
        var insuranceType = "Auto";
        var coverageAmount = 50000m;
        var premiumAmount = 1200m;
        var status = "UnderReview";
        var createdAt = DateTime.UtcNow;

        // Act
        var @event = new ProposalCreated(
            proposalId,
            customerName,
            customerEmail,
            insuranceType,
            coverageAmount,
            premiumAmount,
            status,
            createdAt
        );

        // Assert
        @event.ProposalId.Should().Be(proposalId);
        @event.CustomerName.Should().Be(customerName);
        @event.CustomerEmail.Should().Be(customerEmail);
        @event.InsuranceType.Should().Be(insuranceType);
        @event.CoverageAmount.Should().Be(coverageAmount);
        @event.PremiumAmount.Should().Be(premiumAmount);
        @event.Status.Should().Be(status);
        @event.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void Constructor_WithEmptyStrings_ShouldCreateEvent()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var customerName = "";
        var customerEmail = "";
        var insuranceType = "";
        var coverageAmount = 0m;
        var premiumAmount = 0m;
        var status = "";
        var createdAt = DateTime.UtcNow;

        // Act
        var @event = new ProposalCreated(
            proposalId,
            customerName,
            customerEmail,
            insuranceType,
            coverageAmount,
            premiumAmount,
            status,
            createdAt
        );

        // Assert
        @event.ProposalId.Should().Be(proposalId);
        @event.CustomerName.Should().Be(customerName);
        @event.CustomerEmail.Should().Be(customerEmail);
        @event.InsuranceType.Should().Be(insuranceType);
        @event.CoverageAmount.Should().Be(coverageAmount);
        @event.PremiumAmount.Should().Be(premiumAmount);
        @event.Status.Should().Be(status);
        @event.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void Constructor_WithNullStrings_ShouldCreateEvent()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        string? customerName = null;
        string? customerEmail = null;
        string? insuranceType = null;
        var coverageAmount = 0m;
        var premiumAmount = 0m;
        string? status = null;
        var createdAt = DateTime.UtcNow;

        // Act
        var @event = new ProposalCreated(
            proposalId,
            customerName!,
            customerEmail!,
            insuranceType!,
            coverageAmount,
            premiumAmount,
            status!,
            createdAt
        );

        // Assert
        @event.ProposalId.Should().Be(proposalId);
        @event.CustomerName.Should().BeNull();
        @event.CustomerEmail.Should().BeNull();
        @event.InsuranceType.Should().BeNull();
        @event.CoverageAmount.Should().Be(coverageAmount);
        @event.PremiumAmount.Should().Be(premiumAmount);
        @event.Status.Should().BeNull();
        @event.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void Constructor_WithDifferentStatuses_ShouldCreateEvent()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var statuses = new[] { "UnderReview", "Approved", "Rejected" };

        foreach (var status in statuses)
        {
            // Act
            var @event = new ProposalCreated(
                proposalId,
                "Test",
                "test@email.com",
                "Auto",
                1000m,
                100m,
                status,
                DateTime.UtcNow
            );

            // Assert
            @event.Status.Should().Be(status);
        }
    }

    [Fact]
    public void Constructor_WithDifferentInsuranceTypes_ShouldCreateEvent()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var insuranceTypes = new[] { "Auto", "Home", "Life", "Health", "Travel" };

        foreach (var insuranceType in insuranceTypes)
        {
            // Act
            var @event = new ProposalCreated(
                proposalId,
                "Test",
                "test@email.com",
                insuranceType,
                1000m,
                100m,
                "UnderReview",
                DateTime.UtcNow
            );

            // Assert
            @event.InsuranceType.Should().Be(insuranceType);
        }
    }

    [Fact]
    public void Constructor_WithDifferentAmounts_ShouldCreateEvent()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var amounts = new[] { 0m, 100m, 1000m, 10000m, 100000m, 999999.99m };

        foreach (var amount in amounts)
        {
            // Act
            var @event = new ProposalCreated(
                proposalId,
                "Test",
                "test@email.com",
                "Auto",
                amount,
                amount * 0.1m,
                "UnderReview",
                DateTime.UtcNow
            );

            // Assert
            @event.CoverageAmount.Should().Be(amount);
            @event.PremiumAmount.Should().Be(amount * 0.1m);
        }
    }

    [Fact]
    public void Constructor_WithDifferentDates_ShouldCreateEvent()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var dates = new[]
        {
            DateTime.MinValue,
            DateTime.MaxValue,
            DateTime.UtcNow,
            DateTime.Now,
            new DateTime(2024, 1, 1),
            new DateTime(2024, 12, 31, 23, 59, 59)
        };

        foreach (var date in dates)
        {
            // Act
            var @event = new ProposalCreated(
                proposalId,
                "Test",
                "test@email.com",
                "Auto",
                1000m,
                100m,
                "UnderReview",
                date
            );

            // Assert
            @event.CreatedAt.Should().Be(date);
        }
    }

    [Fact]
    public void Constructor_WithSpecialCharacters_ShouldCreateEvent()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var customerName = "João Silva & Maria Santos";
        var customerEmail = "joão.maria@empresa-123.com";
        var insuranceType = "Auto & Home";
        var status = "Under Review";

        // Act
        var @event = new ProposalCreated(
            proposalId,
            customerName,
            customerEmail,
            insuranceType,
            50000m,
            1200m,
            status,
            DateTime.UtcNow
        );

        // Assert
        @event.CustomerName.Should().Be(customerName);
        @event.CustomerEmail.Should().Be(customerEmail);
        @event.InsuranceType.Should().Be(insuranceType);
        @event.Status.Should().Be(status);
    }

    [Fact]
    public void Constructor_WithLongStrings_ShouldCreateEvent()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var customerName = new string('A', 1000);
        var customerEmail = new string('B', 500) + "@email.com";
        var insuranceType = new string('C', 200);
        var status = new string('D', 100);

        // Act
        var @event = new ProposalCreated(
            proposalId,
            customerName,
            customerEmail,
            insuranceType,
            50000m,
            1200m,
            status,
            DateTime.UtcNow
        );

        // Assert
        @event.CustomerName.Should().Be(customerName);
        @event.CustomerEmail.Should().Be(customerEmail);
        @event.InsuranceType.Should().Be(insuranceType);
        @event.Status.Should().Be(status);
    }

    [Fact]
    public void Constructor_WithZeroGuid_ShouldCreateEvent()
    {
        // Arrange
        var proposalId = Guid.Empty;

        // Act
        var @event = new ProposalCreated(
            proposalId,
            "Test",
            "test@email.com",
            "Auto",
            1000m,
            100m,
            "UnderReview",
            DateTime.UtcNow
        );

        // Assert
        @event.ProposalId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void Constructor_WithNegativeAmounts_ShouldCreateEvent()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var negativeAmounts = new[] { -100m, -1000m, -999999.99m };

        foreach (var amount in negativeAmounts)
        {
            // Act
            var @event = new ProposalCreated(
                proposalId,
                "Test",
                "test@email.com",
                "Auto",
                amount,
                amount,
                "UnderReview",
                DateTime.UtcNow
            );

            // Assert
            @event.CoverageAmount.Should().Be(amount);
            @event.PremiumAmount.Should().Be(amount);
        }
    }
} 