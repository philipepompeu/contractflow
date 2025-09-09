namespace ContractFlow.Domain.Models;

public sealed class SaleContract : Contract
{
    public Guid CustomerId { get; private set; }
    protected SaleContract()
    {
        Type = ContractType.Sale;
    }
    
    public static class Factories
    {

        public static SaleContract Create(Guid customerId, Guid planId, DateOnly startDate, DateTimeOffset? now = null)
        {
            if (customerId == Guid.Empty) throw new ArgumentException("CustomerId inválido.");
            if (planId == Guid.Empty) throw new ArgumentException("PlanId inválido.");            
            if (startDate < DateOnly.FromDateTime(DateTime.UtcNow.Date))
                throw new InvalidOperationException("StartDate não pode ser no passado.");

            var saleContract = new SaleContract
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                PlanId = planId,
                StartDate = startDate,
                CreatedAt = now ?? DateTimeOffset.UtcNow,
                TotalPrice = 1m
            };

            saleContract.RaiseCreated();
            return saleContract;
        }
    }
}