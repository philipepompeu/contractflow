using ContractFlow.Application.Contracts.Abstractions;
using ContractFlow.Application.Commands;
using ContractFlow.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContractFlow.Application.Contracts.Handlers;

public sealed class CreateContractHandler : IRequestHandler<CreateContractCommand, CreateContractResult>
{
    private readonly IContractWriteRepository _repo;
    private readonly ILogger<CreateContractHandler> _logger;

    public CreateContractHandler(IContractWriteRepository repo, ILogger<CreateContractHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<CreateContractResult> Handle(CreateContractCommand cmd, CancellationToken ct)
    {        
        var contract = SaleContract.Factories.Create(cmd.CustomerId, cmd.PlanId, cmd.StartDate);

        await _repo.AddAsync(contract, ct);

        _logger.LogInformation("Contract {ContractId} created for customer {CustomerId} with plan {PlanId} starting at {StartDate}",
            contract.Id, contract.CustomerId, contract.PlanId, contract.StartDate);

        return new CreateContractResult(contract.Id);
    }
}
