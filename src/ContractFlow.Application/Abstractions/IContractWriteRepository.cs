using ContractFlow.Domain.Models;

namespace ContractFlow.Application.Contracts.Abstractions;

public interface IContractWriteRepository
{
    Task AddAsync(Contract contract, CancellationToken ct);
}
