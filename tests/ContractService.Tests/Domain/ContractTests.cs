using FluentAssertions;
using ContractService.Domain.Entities;
using ContractService.Tests.Helpers;

namespace ContractService.Tests.Domain;

public class ContractTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateContract()
    {
        // Arrange
        var contract = FakeDataGenerator.GenerateContract();

        // Act
        var newContract = new Contract(contract.ProposalId, contract.ContractNumber, contract.PremiumAmount);

        // Assert
        newContract.ProposalId.Should().Be(contract.ProposalId);
        newContract.ContractNumber.Should().Be(contract.ContractNumber);
        newContract.PremiumAmount.Should().Be(contract.PremiumAmount);
        newContract.ContractDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        newContract.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidContractNumber_ShouldThrowArgumentException(string contractNumber)
    {
        // Arrange
        var contract = FakeDataGenerator.GenerateContract();

        // Act & Assert
        var action = () => new Contract(contract.ProposalId, contractNumber, contract.PremiumAmount);
        action.Should().Throw<ArgumentException>().WithMessage("*Contract number cannot be null or empty*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Constructor_WithInvalidPremiumAmount_ShouldThrowArgumentException(decimal premiumAmount)
    {
        // Arrange
        var contract = FakeDataGenerator.GenerateContract();

        // Act & Assert
        var action = () => new Contract(contract.ProposalId, contract.ContractNumber, premiumAmount);
        action.Should().Throw<ArgumentException>().WithMessage("*Premium amount must be greater than zero*");
    }

    [Fact]
    public void Constructor_WithValidPremiumAmount_ShouldAcceptPositiveValues()
    {
        // Arrange
        var contract = FakeDataGenerator.GenerateContract();
        var premiumAmount = 0.01m;

        // Act
        var newContract = new Contract(contract.ProposalId, contract.ContractNumber, premiumAmount);

        // Assert
        newContract.PremiumAmount.Should().Be(premiumAmount);
    }

    [Fact]
    public void Constructor_WithValidContractNumber_ShouldAcceptVariousFormats()
    {
        // Arrange
        var contract = FakeDataGenerator.GenerateContract();
        var validContractNumbers = new[]
        {
            "CTR-2024-001",
            "CONTRACT-001",
            "INS-12345",
            "A",
            "123456789012345678901234567890123456789012345678901234567890" // 60 chars
        };

        foreach (var contractNumber in validContractNumbers)
        {
            // Act
            var newContract = new Contract(contract.ProposalId, contractNumber, contract.PremiumAmount);

            // Assert
            newContract.ContractNumber.Should().Be(contractNumber);
        }
    }

    [Fact]
    public void Constructor_WithValidProposalId_ShouldAcceptAnyValidGuid()
    {
        // Arrange
        var contract = FakeDataGenerator.GenerateContract();

        // Act
        var newContract = new Contract(contract.ProposalId, contract.ContractNumber, contract.PremiumAmount);

        // Assert
        newContract.ProposalId.Should().Be(contract.ProposalId);
        newContract.ProposalId.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_WithValidData_ShouldSetDefaultValues()
    {
        // Arrange
        var contract = FakeDataGenerator.GenerateContract();

        // Act
        var newContract = new Contract(contract.ProposalId, contract.ContractNumber, contract.PremiumAmount);

        // Assert
        newContract.Id.Should().NotBeEmpty();
        newContract.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_ShouldGenerateUniqueIds()
    {
        // Arrange
        var contract = FakeDataGenerator.GenerateContract();

        // Act
        var contract1 = new Contract(contract.ProposalId, contract.ContractNumber, contract.PremiumAmount);
        var contract2 = new Contract(contract.ProposalId, "CTR-2024-002", contract.PremiumAmount);

        // Assert
        contract1.Id.Should().NotBe(contract2.Id);
    }
} 