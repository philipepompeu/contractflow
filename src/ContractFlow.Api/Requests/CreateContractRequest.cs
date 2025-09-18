using ContractFlow.Domain.Models;

namespace ContractFlow.Api.Requests
{
    public record CreateContractRequest(Guid PartnerId, Guid PlanId, DateTime StartDate, ContractType Type, decimal totalAmount);
}