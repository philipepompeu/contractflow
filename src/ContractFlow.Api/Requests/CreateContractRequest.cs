namespace ContractFlow.Api.Requests
{
    public record CreateContractRequest(Guid CustomerId, Guid PlanId, DateTime StartDate);
}