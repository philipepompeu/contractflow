
namespace ContractFlow.Domain.Models;
public enum ApprovalStatus { Pending = 0, Approved = 1 }

public class ApprovalDocument : Entity<Guid>
{
    public Guid ContractId { get; private set; }
    public ApprovalStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
    private ApprovalDocument(Guid id, Guid contractId) : base(id)
    {
        Status = ApprovalStatus.Pending;
        ContractId = contractId;
    }

    public static ApprovalDocument Create(Guid contractId)
    {
        var document = new ApprovalDocument(Guid.NewGuid(), contractId);

        return document;
    }
}