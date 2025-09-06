using ContractFlow.Application.Contracts.Abstractions;
using ContractFlow.Domain.Models;
using ContractFlow.Infrastructure.Persistence;

namespace ContractFlow.Infrastructure.Repositories;

public sealed class ContractWriteRepository(ContractFlowDbContext db) : IContractWriteRepository
{
    public async Task AddAsync(Contract contract, CancellationToken ct)
    {
        await db.Contracts.AddAsync(contract, ct);
        await db.SaveChangesAsync(ct);
    }
}
