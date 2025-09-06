using MediatR;

namespace ContractFlow.Application.Commands;

public sealed record CreateContractCommand(Guid CustomerId, Guid PlanId, DateOnly StartDate) : IRequest<CreateContractResult>
{    
}

public sealed record CreateContractResult(Guid ContractId);