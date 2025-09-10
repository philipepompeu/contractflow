namespace ContractFlow.Domain.Models;
public sealed class PurchaseContract : Contract
{
    public Guid SupplierId { get; private set; }
    private PurchaseContract(Guid id): base(id)
    { 
        Type = ContractType.Purchase;
    }
}