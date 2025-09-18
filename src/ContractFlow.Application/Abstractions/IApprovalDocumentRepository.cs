using ContractFlow.Application.Queries;
using ContractFlow.Domain.Models;

namespace ContractFlow.Application.Abstractions;

public interface IApprovalDocumentRepository
{
    Task AddAsync(ApprovalDocument document, CancellationToken ct);
    Task<(IReadOnlyList<PendingApprovalDto> items, int total)> GetPendingDocuments(int page, int size,CancellationToken ct);
}