namespace ContractService.Ports.Outbound;

public interface IContractNumberGeneratorPort
{
    Task<string> GenerateAsync();
} 