namespace InsuranceSystem.Shared.Infrastructure.Validation;

public interface IValidator<in T>
{
    ValidationResult Validate(T item);
    Task<ValidationResult> ValidateAsync(T item);
} 