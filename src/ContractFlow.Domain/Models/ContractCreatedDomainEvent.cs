using ContractFlow.Domain.Common;

namespace ContractFlow.Domain.Models;

public sealed record ContractCreatedDomainEvent(Guid ContractId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}