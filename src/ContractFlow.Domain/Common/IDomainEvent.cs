namespace ContractFlow.Domain.Common;

public interface IDomainEvent
{
    DateTimeOffset OccurredOn { get; }
}
