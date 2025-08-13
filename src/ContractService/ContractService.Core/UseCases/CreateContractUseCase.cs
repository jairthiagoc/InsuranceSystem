using ContractService.Domain.Entities;
using ContractService.Ports.Inbound;
using ContractService.Ports.Inbound.Shared;
using ContractService.Ports.Outbound;
using InsuranceSystem.Shared.Infrastructure.Messaging.Events;

namespace ContractService.Core.UseCases;

public class CreateContractUseCase : ICreateContractPort
{
    private readonly IContractRepositoryPort _contractRepository;
    private readonly IProposalServicePort _proposalService;
    private readonly IContractNumberGeneratorPort _contractNumberGenerator;
    private readonly IEventPublisherPort _eventPublisher;

    public CreateContractUseCase(
        IContractRepositoryPort contractRepository, 
        IProposalServicePort proposalService, 
        IContractNumberGeneratorPort contractNumberGenerator, 
        IEventPublisherPort eventPublisher)
    {
        _contractRepository = contractRepository;
        _proposalService = proposalService;
        _contractNumberGenerator = contractNumberGenerator;
        _eventPublisher = eventPublisher;
    }

    public async Task<ContractResult> ExecuteAsync(CreateContractRequest request)
    {
        // Verificar se a proposta existe e está aprovada
        var proposal = await _proposalService.GetProposalAsync(request.ProposalId);
        
        if (proposal == null)
            throw new ArgumentException("Proposal not found");

        if (proposal.Status != "Approved")
            throw new InvalidOperationException($"Cannot contract proposal. Current status: {proposal.Status}");

        // Verificar se já existe contrato para esta proposta
        var existingContract = await _contractRepository.GetByProposalIdAsync(request.ProposalId);
        if (existingContract != null)
            throw new InvalidOperationException("Contract already exists for this proposal");

        // Gerar número de contrato automaticamente
        var contractNumber = await _contractNumberGenerator.GenerateAsync();
        
        // Obter premium amount da proposta
        var premiumAmount = proposal.PremiumAmount;

        var contract = new Contract(request.ProposalId, contractNumber, premiumAmount);
        var createdContract = await _contractRepository.AddAsync(contract);

        await _eventPublisher.PublishAsync(new ContractCreated(
            createdContract.Id,
            createdContract.ProposalId,
            createdContract.ContractNumber,
            createdContract.PremiumAmount,
            createdContract.ContractDate,
            createdContract.CreatedAt
        ));

        return new ContractResult(
            createdContract.Id,
            createdContract.ProposalId,
            createdContract.ContractDate,
            createdContract.ContractNumber,
            createdContract.PremiumAmount,
            createdContract.CreatedAt
        );
    }
} 