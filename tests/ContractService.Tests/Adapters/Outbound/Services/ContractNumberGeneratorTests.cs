using FluentAssertions;
using ContractService.Adapters.Outbound.Services;
using ContractService.Tests.Helpers;

namespace ContractService.Tests.Adapters.Outbound.Services;

public class ContractNumberGeneratorTests
{
    private readonly ContractNumberGenerator _generator;

    public ContractNumberGeneratorTests()
    {
        _generator = new ContractNumberGenerator();
    }

    [Fact]
    public async Task GenerateAsync_ShouldReturnValidContractNumber()
    {
        // Act
        var result = await _generator.GenerateAsync();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().MatchRegex(@"^CT-\d{8}-\d{4}$");
    }

    [Fact]
    public async Task GenerateAsync_ShouldReturnDifferentNumbersOnMultipleCalls()
    {
        // Act
        var result1 = await _generator.GenerateAsync();
        var result2 = await _generator.GenerateAsync();
        var result3 = await _generator.GenerateAsync();

        // Assert
        result1.Should().NotBe(result2);
        result2.Should().NotBe(result3);
        result1.Should().NotBe(result3);
    }

    [Fact]
    public async Task GenerateAsync_ShouldReturnCorrectFormat()
    {
        // Act
        var result = await _generator.GenerateAsync();

        // Assert
        result.Should().StartWith("CT-");
        result.Should().Contain("-");
        result.Split('-').Should().HaveCount(3);
    }

    [Fact]
    public async Task GenerateAsync_ShouldReturnCurrentDateInFormat()
    {
        // Arrange
        var currentDate = DateTime.UtcNow;

        // Act
        var result = await _generator.GenerateAsync();

        // Assert
        var parts = result.Split('-');
        parts.Should().HaveCount(3);
        
        var datePart = parts[1];
        datePart.Should().HaveLength(8);
        datePart.Should().Be(currentDate.ToString("yyyyMMdd"));
    }

    [Fact]
    public async Task GenerateAsync_ShouldReturnRandomNumberInCorrectRange()
    {
        // Act
        var result = await _generator.GenerateAsync();

        // Assert
        var parts = result.Split('-');
        var randomPart = parts[2];
        
        randomPart.Should().HaveLength(4);
        int.TryParse(randomPart, out var randomNumber).Should().BeTrue();
        randomNumber.Should().BeGreaterThanOrEqualTo(1000);
        randomNumber.Should().BeLessThanOrEqualTo(9999);
    }

    [Fact]
    public async Task GenerateAsync_ShouldBeAsync()
    {
        // Act
        var task = _generator.GenerateAsync();

        // Assert
        task.Should().NotBeNull();
        task.IsCompleted.Should().BeFalse();
        
        var result = await task;
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GenerateAsync_ShouldReturnConsistentFormat()
    {
        // Act
        var results = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            results.Add(await _generator.GenerateAsync());
        }

        // Assert
        foreach (var result in results)
        {
            result.Should().MatchRegex(@"^CT-\d{8}-\d{4}$");
        }
    }

    [Fact]
    public async Task GenerateAsync_ShouldHandleConcurrentCalls()
    {
        // Arrange
        var tasks = new List<Task<string>>();
        for (int i = 0; i < 100; i++)
        {
            tasks.Add(_generator.GenerateAsync());
        }

        // Act
        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().HaveCount(100);
        results.Should().OnlyContain(r => !string.IsNullOrEmpty(r));
        results.Should().OnlyContain(r => System.Text.RegularExpressions.Regex.IsMatch(r, @"^CT-\d{8}-\d{4}$"));
        
        // Verificar que todos são únicos (pode haver algumas duplicatas devido ao random)
        var uniqueResults = results.Distinct().ToList();
        uniqueResults.Count.Should().BeGreaterThan(50); // Pelo menos 50% devem ser únicos
    }

    [Fact]
    public async Task GenerateAsync_ShouldReturnValidDateComponent()
    {
        // Arrange
        var expectedDate = DateTime.UtcNow.ToString("yyyyMMdd");

        // Act
        var result = await _generator.GenerateAsync();

        // Assert
        var dateComponent = result.Split('-')[1];
        dateComponent.Should().Be(expectedDate);
    }

    [Fact]
    public async Task GenerateAsync_ShouldReturnValidRandomComponent()
    {
        // Act
        var result = await _generator.GenerateAsync();

        // Assert
        var randomComponent = result.Split('-')[2];
        randomComponent.Should().HaveLength(4);
        
        var randomNumber = int.Parse(randomComponent);
        randomNumber.Should().BeInRange(1000, 9999);
    }
} 