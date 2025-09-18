using ContractFlow.Application.Abstractions;
using ContractFlow.Application.Queries;
using MediatR;

namespace ContractFlow.Application.Handlers;

public sealed class GetPendingApprovalsHandler(IApprovalDocumentRepository repository)
    : IRequestHandler<GetPendingApprovalsQuery, PageResult<PendingApprovalDto>>
{

    public async Task<PageResult<PendingApprovalDto>> Handle(GetPendingApprovalsQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await repository.GetPendingDocuments(request.Page, request.Size, cancellationToken);
        
        var totalPages = (int)Math.Ceiling(total / (double)request.Size);
        
        return new PageResult<PendingApprovalDto>(items, request.Page, request.Size, total, totalPages);
    }
}