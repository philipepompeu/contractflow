namespace ContractFlow.Domain.Models;

public sealed class PurchaseContract : Contract
{
    public Guid SupplierId { get; private set; }
    private PurchaseContract(Guid id) : base(id)
    {
        Type = ContractType.Purchase;
    }

    public static class Factories
    {

        public static PurchaseContract Create(Guid supplierId, Guid planId, DateOnly startDate, DateTimeOffset? now = null)
        {
            if (supplierId == Guid.Empty) throw new ArgumentException("SupplierId inválido.");
            if (planId == Guid.Empty) throw new ArgumentException("PlanId inválido.");
            if (startDate < DateOnly.FromDateTime(DateTime.UtcNow.Date))
                throw new InvalidOperationException("StartDate não pode ser no passado.");

            var purchaseContract = new PurchaseContract(Guid.NewGuid())
            {
                SupplierId = supplierId,
                PlanId = planId,
                StartDate = startDate,
                CreatedAt = now ?? DateTimeOffset.UtcNow,
                TotalPrice = 1m
            };

            purchaseContract.RaiseCreated();
            return purchaseContract;
        }
    }
    public class Builder
    {
        private Guid _supplierId;
        private Guid _planId;
        private DateOnly _startDate;
        private DateTimeOffset? _now;
        
        private Money? _totalPrice = 1m;

        private Builder() { }

        public static Builder New() => new Builder();
        public Builder WithSupplierId(Guid supplierId)
        {
            _supplierId = supplierId;
            return this;
        }        

        public Builder WithPlanId(Guid planId)
        {
            _planId = planId;
            return this;
        }

        public Builder WithStartDate(DateOnly startDate)
        {
            _startDate = startDate;
            return this;
        }

        public Builder WithNow(DateTimeOffset now)
        {
            _now = now;
            return this;
        }
        
        public Builder WithTotalPrice(decimal totalPrice)
        {
            _totalPrice = totalPrice;
            return this;
        }

        public PurchaseContract Build()
        {

            var purchaseContract = new PurchaseContract(Guid.NewGuid())
            {
                SupplierId = _supplierId,
                PlanId = _planId,
                StartDate = _startDate,
                CreatedAt = _now ?? DateTimeOffset.UtcNow,
                TotalPrice = _totalPrice
            };
            purchaseContract.RaiseCreated();

            return purchaseContract;

        }
    }
}