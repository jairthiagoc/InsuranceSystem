using Bogus;
using ContractService.Domain.Entities;
using ContractService.Ports.Inbound;
using ContractService.Ports.Inbound.Shared;

namespace ContractService.Tests.Helpers;

public static class FakeDataGenerator
{
    private static readonly Faker _faker = new Faker("pt_BR");

    public static Faker<Contract> ContractFaker => new Faker<Contract>("pt_BR")
        .CustomInstantiator(f => new Contract(
            f.Random.Guid(),
            f.Random.Replace("CTR-####-###"),
            f.Random.Decimal(100, 10000)
        ));

    public static Faker<CreateContractRequest> CreateContractRequestFaker => new Faker<CreateContractRequest>("pt_BR")
        .CustomInstantiator(f => new CreateContractRequest(
            f.Random.Guid()
        ));

    // Métodos utilitários
    public static Contract GenerateContract() => ContractFaker.Generate();
    public static IEnumerable<Contract> GenerateContracts(int count = 5) => ContractFaker.Generate(count);
    
    public static CreateContractRequest GenerateCreateContractRequest() => CreateContractRequestFaker.Generate();

    // Geradores específicos para cenários de teste
    public static Contract GenerateContractWithSpecificProposalId(Guid proposalId)
    {
        var contract = GenerateContract();
        // Usar reflection para definir o ProposalId
        var proposalIdProperty = typeof(Contract).GetProperty("ProposalId");
        proposalIdProperty?.SetValue(contract, proposalId);
        return contract;
    }

    public static Contract GenerateContractWithSpecificNumber(string contractNumber)
    {
        var contract = GenerateContract();
        // Usar reflection para definir o ContractNumber
        var contractNumberProperty = typeof(Contract).GetProperty("ContractNumber");
        contractNumberProperty?.SetValue(contract, contractNumber);
        return contract;
    }

    public static CreateContractRequest GenerateCreateContractRequestWithProposalId(Guid proposalId)
    {
        return new CreateContractRequest(proposalId);
    }
} 