namespace ContractService.Domain.Entities;

public class Contract
{
    public Guid Id { get; private set; }
    public Guid ProposalId { get; private set; }
    public DateTime ContractDate { get; private set; }
    public string ContractNumber { get; private set; }
    public decimal PremiumAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Contract() { } // Para EF Core

    public Contract(Guid proposalId, string contractNumber, decimal premiumAmount)
    {
        Id = Guid.NewGuid();
        ProposalId = proposalId;
        ContractNumber = !string.IsNullOrWhiteSpace(contractNumber) ? contractNumber : throw new ArgumentException("Contract number cannot be null or empty");
        PremiumAmount = premiumAmount > 0 ? premiumAmount : throw new ArgumentException("Premium amount must be greater than zero");
        ContractDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }
} 