using ContractFlow.Application.Abstractions;
using ContractFlow.Application.Commands;
using ContractFlow.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContractFlow.Application.Handlers;

public sealed class ApproveDocumentHandler: IRequestHandler<ApproveDocumentCommand, ApproveDocumentResult>
{

    private readonly ILogger<ApproveDocumentHandler> _logger;
    private readonly IApprovalDocumentRepository _repo;
    
    public ApproveDocumentHandler(ILogger<ApproveDocumentHandler> logger,  IApprovalDocumentRepository repo)
    {
        _logger = logger;
        _repo = repo;
    }
    public async Task<ApproveDocumentResult> Handle(ApproveDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = _repo.GetById(request.DocumentId, cancellationToken);
        
        if (document is not ApprovalDocument)
            throw new InvalidOperationException("Approval document not found");
            
        if(document.Status == ApprovalStatus.Approved)
            throw new InvalidOperationException("Document already approved");
            
        await _repo.ApproveDocument(document, cancellationToken);

        return new ApproveDocumentResult(document.Id, document.ContractId, document.ApprovedAt?.DateTime ?? DateTime.UtcNow);
    }
}