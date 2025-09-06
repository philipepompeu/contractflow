namespace ContractFlow.Domain.Models;

public enum ContractStatus { Draft = 0, Active = 1 }
public enum ContractType { Purchase = 1, Sale = 2 }

public abstract class Contract
{

    public Guid Id { get; init; }
    public Guid PlanId { get; init; }
    public DateOnly StartDate { get; init; }
    public ContractStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; init; }
    public ContractType Type { get; init; }
    public Money TotalPrice { get; set; }

    // Construtor privado para EF
    protected Contract() { }

    public void Activate()
    {
        if (Status == ContractStatus.Active)
            throw new InvalidOperationException("Contrato já está ativo.");
        Status = ContractStatus.Active;
    }
        
}