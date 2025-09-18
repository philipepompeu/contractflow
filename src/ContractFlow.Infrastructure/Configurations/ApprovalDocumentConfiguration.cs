using ContractFlow.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractFlow.Infrastructure.Persistence.Configurations;

public class ApprovalDocumentConfiguration: IEntityTypeConfiguration<ApprovalDocument>
{

    public void Configure(EntityTypeBuilder<ApprovalDocument> builder)
    {
        builder.ToTable("approval_documents");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.ContractId).HasColumnName("contract_id").IsRequired();
        builder.Property(x => x.Status).HasColumnName("status");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.Property(x => x.ApprovedAt).HasColumnName("approved_at");
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>();

    }
}