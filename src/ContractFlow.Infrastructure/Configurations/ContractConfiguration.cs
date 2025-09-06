using ContractFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractFlow.Infrastructure.Persistence.Configurations;

public sealed class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> b)
    {
        b.ToTable("contracts");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id");

        b.Property(x => x.PlanId)
            .HasColumnName("plan_id")
            .IsRequired();

        b.Property(x => x.StartDate)
            .HasColumnName("start_date")
            .IsRequired();
        // Npgsql mapeia DateOnly -> DATE automaticamente

        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()     // enum como int
            .IsRequired();

        b.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        b.Property(x => x.Type)
            .HasColumnName("contract_type")
            .HasConversion<int>()
            .IsRequired();

        b.HasDiscriminator(x => x.Type)
            .HasValue<SaleContract>(ContractType.Sale)
            .HasValue<PurchaseContract>(ContractType.Purchase);
            
        b.OwnsOne(x => x.TotalPrice, owned =>
        {
            owned.Property(p => p.Amount)
                .HasColumnName("total_amount")
                .HasPrecision(18, 2)           // dinheiro com precisão
                .IsRequired();

            owned.Property(p => p.Currency)
                .HasColumnName("total_currency")
                .HasMaxLength(3)
                .IsRequired();
        });

    }
    
    // 2) Config da derivada: propriedades específicas do SaleContract
    public sealed class SaleContractConfiguration : IEntityTypeConfiguration<SaleContract>
    {
        public void Configure(EntityTypeBuilder<SaleContract> b)
        {
            b.Property(x => x.CustomerId).HasColumnName("customer_id");
            b.HasIndex(x => x.CustomerId).HasDatabaseName("ix_contracts_customer");
        }
    }

    // 3) Config da derivada: propriedades específicas do PurchaseContract
    public sealed class PurchaseContractConfiguration : IEntityTypeConfiguration<PurchaseContract>
    {
        public void Configure(EntityTypeBuilder<PurchaseContract> b)
        {
            b.Property(x => x.SupplierId).HasColumnName("supplier_id");
            b.HasIndex(x => x.SupplierId).HasDatabaseName("ix_contracts_supplier");
        }
    }
}
