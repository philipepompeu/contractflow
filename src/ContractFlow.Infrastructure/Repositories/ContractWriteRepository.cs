using ContractFlow.Application.Contracts.Abstractions;
using ContractFlow.Domain.Models;
using ContractFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ContractFlow.Infrastructure.Repositories;

public sealed class ContractWriteRepository(ContractFlowDbContext db) : IContractWriteRepository
{
    public async Task AddAsync(Contract contract, CancellationToken ct)
    {
        await db.Contracts.AddAsync(contract, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task<Contract?> GetById(Guid id, CancellationToken ct)
    {
        return await db.Contracts.FirstAsync(con => con.Id == id, ct);
    }

    public async Task UpdateAsync(Contract contract, CancellationToken ct)
    {
        if (contract != null)
            await db.SaveChangesAsync(ct);
    }
}
