using ContractFlow.Domain.Common;

namespace ContractFlow.Domain.Models;

public sealed record DocumentApprovedEvent(Guid DocumentId): IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } =  DateTimeOffset.UtcNow;
}
