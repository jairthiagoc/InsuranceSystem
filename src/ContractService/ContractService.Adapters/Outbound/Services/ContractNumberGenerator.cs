using ContractService.Ports.Outbound;

namespace ContractService.Adapters.Outbound.Services;

public class ContractNumberGenerator : IContractNumberGeneratorPort
{
    public async Task<string> GenerateAsync()
    {
        // Gerar número de contrato no formato: CT-YYYYMMDD-XXXX
        var date = DateTime.UtcNow;
        var random = new Random();
        var randomPart = random.Next(1000, 9999);
        
        var contractNumber = $"CT-{date:yyyyMMdd}-{randomPart:D4}";
        
        return await Task.FromResult(contractNumber);
    }
} 