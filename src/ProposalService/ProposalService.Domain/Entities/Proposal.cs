using ProposalService.Domain.Enums;
using ProposalService.Domain.Exceptions;

namespace ProposalService.Domain.Entities;

public class Proposal
{
    public Guid Id { get; private set; }
    public string CustomerName { get; private set; }
    public string CustomerEmail { get; private set; }
    public string InsuranceType { get; private set; }
    public decimal CoverageAmount { get; private set; }
    public decimal PremiumAmount { get; private set; }
    public ProposalStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public string? RejectionReason { get; private set; }

    private Proposal() { } // Para EF Core

    public Proposal(string customerName, string customerEmail, string insuranceType, decimal coverageAmount, decimal premiumAmount)
    {
        Id = Guid.NewGuid();
        CustomerName = !string.IsNullOrWhiteSpace(customerName) ? customerName : throw new ArgumentException("Customer name cannot be null or empty");
        CustomerEmail = !string.IsNullOrWhiteSpace(customerEmail) ? customerEmail : throw new ArgumentException("Customer email cannot be null or empty");
        InsuranceType = !string.IsNullOrWhiteSpace(insuranceType) ? insuranceType : throw new ArgumentException("Insurance type cannot be null or empty");
        CoverageAmount = coverageAmount > 0 ? coverageAmount : throw new ArgumentException("Coverage amount must be greater than zero");
        PremiumAmount = premiumAmount > 0 ? premiumAmount : throw new ArgumentException("Premium amount must be greater than zero");
        Status = ProposalStatus.UnderReview; // Propostas sempre começam em análise
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve()
    {
        if (Status != ProposalStatus.UnderReview)
            throw new InvalidProposalStatusException($"Cannot approve proposal. Current status: {Status}");

        Status = ProposalStatus.Approved;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(string reason)
    {
        if (Status != ProposalStatus.UnderReview)
            throw new InvalidProposalStatusException($"Cannot reject proposal. Current status: {Status}");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Rejection reason is required");

        Status = ProposalStatus.Rejected;
        RejectionReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(ProposalStatus status, string? rejectionReason = null)
    {
        switch (status)
        {
            case ProposalStatus.Approved:
                Approve();
                break;
            case ProposalStatus.Rejected:
                Reject(rejectionReason ?? "No reason provided");
                break;
            default:
                Status = status;
                UpdatedAt = DateTime.UtcNow;
                break;
        }
    }

    public bool CanBeContracted => Status == ProposalStatus.Approved;

    public void UpdateDetails(string customerName, string customerEmail, string insuranceType, decimal coverageAmount, decimal premiumAmount)
    {
        CustomerName = !string.IsNullOrWhiteSpace(customerName) ? customerName : throw new ArgumentException("Customer name cannot be null or empty");
        CustomerEmail = !string.IsNullOrWhiteSpace(customerEmail) ? customerEmail : throw new ArgumentException("Customer email cannot be null or empty");
        InsuranceType = !string.IsNullOrWhiteSpace(insuranceType) ? insuranceType : throw new ArgumentException("Insurance type cannot be null or empty");
        CoverageAmount = coverageAmount > 0 ? coverageAmount : throw new ArgumentException("Coverage amount must be greater than zero");
        PremiumAmount = premiumAmount > 0 ? premiumAmount : throw new ArgumentException("Premium amount must be greater than zero");
        UpdatedAt = DateTime.UtcNow;
    }
} 