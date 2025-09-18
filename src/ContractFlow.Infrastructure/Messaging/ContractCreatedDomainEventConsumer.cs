using ContractFlow.Application.Abstractions;
using ContractFlow.Domain.Models;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ContractFlow.Infrastructure.Messaging;

public class ContractCreatedDomainEventConsumer(
    IApprovalDocumentRepository repository, 
    ILogger<ContractCreatedDomainEventConsumer> log): IConsumer<ContractCreatedDomainEvent>
{
    public async Task Consume(ConsumeContext<ContractCreatedDomainEvent> context)
    {
        log.LogInformation("ContractCreatedDomainEvent consumer begin");
        
        var contractId = context.Message.ContractId;
        var document = ApprovalDocument.Create(contractId);
        
        await repository.AddAsync(document, CancellationToken.None);
        log.LogInformation("ContractCreatedDomainEvent consumer end");
    }
}