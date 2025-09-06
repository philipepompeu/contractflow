namespace ContractFlow.Domain.Models;
public sealed class PurchaseContract : Contract
{
    public Guid SupplierId { get; private set; }
    private PurchaseContract()
    { 
        Type = ContractType.Purchase;
    }
}