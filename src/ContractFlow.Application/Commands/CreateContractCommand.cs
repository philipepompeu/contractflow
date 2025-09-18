using ContractFlow.Domain.Models;
using MediatR;

namespace ContractFlow.Application.Commands;

public sealed record CreateContractCommand(Guid PartnerId, Guid PlanId, DateOnly StartDate, ContractType Type, decimal totalAmount) : IRequest<CreateContractResult>
{    
}

public sealed record CreateContractResult(Guid ContractId);