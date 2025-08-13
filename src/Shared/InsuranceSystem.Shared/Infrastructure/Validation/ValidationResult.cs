namespace InsuranceSystem.Shared.Infrastructure.Validation;

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();

    public static ValidationResult Success()
    {
        return new ValidationResult { IsValid = true };
    }

    public static ValidationResult Failure(params string[] errors)
    {
        return new ValidationResult
        {
            IsValid = false,
            Errors = errors.ToList()
        };
    }

    public ValidationResult AddError(string error)
    {
        Errors.Add(error);
        IsValid = false;
        return this;
    }

    public ValidationResult AddWarning(string warning)
    {
        Warnings.Add(warning);
        return this;
    }

    public ValidationResult Merge(ValidationResult other)
    {
        if (!other.IsValid)
        {
            IsValid = false;
        }
        
        Errors.AddRange(other.Errors);
        Warnings.AddRange(other.Warnings);
        
        return this;
    }
} 