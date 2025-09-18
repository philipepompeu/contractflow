using ContractFlow.Application.Abstractions;
using ContractFlow.Application.Queries;
using ContractFlow.Domain.Models;
using ContractFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ContractFlow.Infrastructure.Repositories;

public class ApprovalDocumentRepository(ContractFlowDbContext db):IApprovalDocumentRepository
{
    public async Task AddAsync(ApprovalDocument document, CancellationToken ct)
    {
        await db.Documents.AddAsync(document, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task<(IReadOnlyList<PendingApprovalDto> items, int total)> GetPendingDocuments(int page, int size, CancellationToken ct)
    {
        var query = db.Documents.AsNoTracking().Where(d => d.Status == ApprovalStatus.Pending);
        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .Select(x => new PendingApprovalDto(x.Id, x.ContractId, x.CreatedAt))
            .ToListAsync();
        
        return (items, total);
    }

    
}