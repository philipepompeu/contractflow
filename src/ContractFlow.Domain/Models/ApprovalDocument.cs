
using ContractFlow.Domain.Common;

namespace ContractFlow.Domain.Models;
public enum ApprovalStatus { Pending = 0, Approved = 1 }

public class ApprovalDocument : Entity<Guid>, IHasDomainEvents
{
    public Guid ContractId { get; private set; }
    public ApprovalStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ApprovedAt { get; private set; }
    private ApprovalDocument(Guid id, Guid contractId) : base(id)
    {
        Status = ApprovalStatus.Pending;
        ContractId = contractId;
    }

    public void Approve()
    {
        Status = ApprovalStatus.Approved;
        ApprovedAt = DateTimeOffset.UtcNow;
        RaiseApproved();
    } 

    public static ApprovalDocument Create(Guid contractId)
    {
        var document = new ApprovalDocument(Guid.NewGuid(), contractId);

        return document;
    }

    private readonly List<IDomainEvent> _domainEvents = new(); 
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    public void ClearDomainEvents() => _domainEvents.Clear();

    private void RaiseApproved() => _domainEvents.Add(new DocumentApprovedEvent(Id));
}