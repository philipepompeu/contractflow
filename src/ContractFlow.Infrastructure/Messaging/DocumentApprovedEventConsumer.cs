using ContractFlow.Application.Abstractions;
using ContractFlow.Application.Contracts.Abstractions;
using ContractFlow.Domain.Models;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ContractFlow.Infrastructure.Messaging;

public class DocumentApprovedEventConsumer(IContractWriteRepository _contractRepository, IApprovalDocumentRepository _documentRepository, ILogger<DocumentApprovedEventConsumer> _logger):IConsumer<DocumentApprovedEvent>
{
    public async Task Consume(ConsumeContext<DocumentApprovedEvent> context)
    {
        _logger.LogInformation("DocumentApprovedDomainEvent consumer started");

        ApprovalDocument? document = _documentRepository.GetById(context.Message.DocumentId, CancellationToken.None);
        
        if (document == null)   
            throw new Exception("Document not found"); 
        
        var contract = await _contractRepository.GetById(document.ContractId, CancellationToken.None);
        
        if(contract == null)
            throw new Exception("Contract not found");
        
        contract.Activate();

        await _contractRepository.UpdateAsync(contract, CancellationToken.None);
        
        _logger.LogInformation("DocumentApprovedDomainEvent consumer finished");
    }
}