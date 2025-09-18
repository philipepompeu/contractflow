using ContractFlow.Application.Contracts.Abstractions;
using ContractFlow.Application.Commands;
using ContractFlow.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContractFlow.Application.Handlers;

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

        Contract? contract = default;

        switch(cmd.Type)
        {
            case ContractType.Purchase:

                contract = PurchaseContract.Builder.New()
                            .WithSupplierId(cmd.PartnerId)
                            .WithPlanId(cmd.PlanId)
                            .WithStartDate(cmd.StartDate)
                            .WithTotalPrice(cmd.totalAmount)
                            .Build();
                            
                await _repo.AddAsync(contract, ct);
                break;
            case ContractType.Sale:
                contract = SaleContract.Factories.Create(cmd.PartnerId, cmd.PlanId, cmd.StartDate);
                await _repo.AddAsync(contract, ct);
                break;
            default:
                throw new ArgumentException("Invalid contract type.");
        }
        
        if (contract is Contract)
        {
            _logger.LogInformation("Contract {ContractId} created with plan {PlanId} starting at {StartDate}",
                contract.Id, contract.PlanId, contract.StartDate);

            return new CreateContractResult(contract.Id);
        }
        else
            throw new InvalidOperationException("Failed to create contract.");

    }
}
