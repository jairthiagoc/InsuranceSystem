using MassTransit;
using InsuranceSystem.Shared.Infrastructure.Messaging.Events;
using Microsoft.Extensions.Logging;

namespace ContractService.Infrastructure.Messaging;

public class ProposalStatusUpdatedConsumer : IConsumer<ProposalStatusUpdated>
{
    private readonly ILogger<ProposalStatusUpdatedConsumer> _logger;

    public ProposalStatusUpdatedConsumer(ILogger<ProposalStatusUpdatedConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProposalStatusUpdated> context)
    {
        var message = context.Message;
        
        _logger.LogInformation(
            "Proposta {ProposalId} teve status alterado para {Status} em {UpdatedAt}",
            message.ProposalId,
            message.Status,
            message.UpdatedAt
        );

        // Aqui você poderia implementar lógica específica
        // Por exemplo: enviar notificação, atualizar cache, etc.
        
        if (message.Status == "Approved")
        {
            _logger.LogInformation("Proposta {ProposalId} foi aprovada - pronto para criar contrato", message.ProposalId);
        }
        else if (message.Status == "Rejected")
        {
            _logger.LogWarning("Proposta {ProposalId} foi rejeitada. Motivo: {Reason}", 
                message.ProposalId, 
                message.RejectionReason ?? "Não informado");
        }

        await Task.CompletedTask;
    }
} 