using FluentAssertions;
using ProposalService.Domain.Exceptions;

namespace ProposalService.Tests.Domain;

public class InvalidProposalStatusExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_ShouldCreateException()
    {
        // Arrange
        var message = "Invalid status transition";

        // Act
        var exception = new InvalidProposalStatusException(message);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_ShouldCreateException()
    {
        // Arrange
        var message = "Invalid status transition";
        var innerException = new InvalidOperationException("Inner error");

        // Act
        var exception = new InvalidProposalStatusException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }

    [Fact]
    public void Constructor_WithNullMessage_ShouldCreateException()
    {
        // Act
        var exception = new InvalidProposalStatusException(null!);

        // Assert
        exception.Message.Should().NotBeNull(); // .NET gera mensagem padrão
        exception.InnerException.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithNullMessageAndInnerException_ShouldCreateException()
    {
        // Arrange
        var innerException = new InvalidOperationException("Inner error");

        // Act
        var exception = new InvalidProposalStatusException(null!, innerException);

        // Assert
        exception.Message.Should().NotBeNull(); // .NET gera mensagem padrão
        exception.InnerException.Should().Be(innerException);
    }
} 