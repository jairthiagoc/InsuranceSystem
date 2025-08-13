using FluentAssertions;
using InsuranceSystem.Shared.Infrastructure.Validation;

namespace ProposalService.Tests.Shared;

public class ValidationResultTests
{
    [Fact]
    public void Success_ShouldReturnValidResult()
    {
        var result = ValidationResult.Success();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        result.Warnings.Should().BeEmpty();
    }

    [Fact]
    public void Failure_ShouldReturnInvalidResultWithErrors()
    {
        var result = ValidationResult.Failure("e1", "e2");
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(new[] { "e1", "e2" });
    }

    [Fact]
    public void AddError_ShouldAppendErrorAndInvalidate()
    {
        var result = ValidationResult.Success().AddError("oops");
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("oops");
    }

    [Fact]
    public void AddWarning_ShouldAppendWarning()
    {
        var result = ValidationResult.Success().AddWarning("warn");
        result.Warnings.Should().Contain("warn");
    }

    [Fact]
    public void Merge_ShouldMergeErrorsWarningsAndValidity()
    {
        var a = ValidationResult.Success().AddWarning("w1");
        var b = ValidationResult.Failure("e1").AddWarning("w2");

        var merged = a.Merge(b);

        merged.IsValid.Should().BeFalse();
        merged.Errors.Should().Contain("e1");
        merged.Warnings.Should().Contain(new[] { "w1", "w2" });
    }
} 