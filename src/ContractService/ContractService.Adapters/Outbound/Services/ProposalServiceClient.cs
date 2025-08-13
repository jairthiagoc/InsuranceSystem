using ContractService.Ports.Outbound;
using Microsoft.Extensions.Configuration;
using InsuranceSystem.Shared.Infrastructure.Http;

namespace ContractService.Adapters.Outbound.Services;

public class ProposalServiceClient : IProposalServicePort
{
    private readonly IResilientHttpClient _httpClient;
    private readonly string _proposalServiceUrl;

    public ProposalServiceClient(IResilientHttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _proposalServiceUrl = configuration["ProposalService:BaseUrl"] ?? "https://localhost:7001";
    }

    public async Task<ProposalInfo?> GetProposalAsync(Guid proposalId)
    {
        var response = await _httpClient.GetAsync($"{_proposalServiceUrl}/api/proposals/{proposalId}");
        
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"HTTP {(int)response.StatusCode}: {response.StatusCode}");
        }

        var content = await response.Content.ReadAsStringAsync();
        
        // Deserializar para um objeto dinâmico primeiro para converter o status
        var jsonDoc = System.Text.Json.JsonDocument.Parse(content);
        var root = jsonDoc.RootElement;
        
        // Agora o status já vem como string, mas vamos garantir compatibilidade
        string statusString;
        if (root.GetProperty("status").ValueKind == System.Text.Json.JsonValueKind.String)
        {
            statusString = root.GetProperty("status").GetString()!;
        }
        else
        {
            // Fallback para o formato antigo (número)
            var statusValue = root.GetProperty("status").GetInt32();
            statusString = statusValue switch
            {
                0 => "UnderReview",  // Em Análise
                1 => "Approved",     // Aprovada
                2 => "Rejected",     // Rejeitada
                _ => "Unknown"
            };
        }
        
        var proposal = new ProposalInfo(
            root.GetProperty("id").GetGuid(),
            root.GetProperty("customerName").GetString()!,
            root.GetProperty("customerEmail").GetString()!,
            root.GetProperty("insuranceType").GetString()!,
            root.GetProperty("coverageAmount").GetDecimal(),
            root.GetProperty("premiumAmount").GetDecimal(),
            statusString
        );

        return proposal;
    }
} 