using ContractFlow.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractFlow.Infrastructure.Persistence.Configurations;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> b)
    {
        b.ToTable("outbox_messages");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasColumnName("id");
        b.Property(x => x.EventType).HasColumnName("event_type").IsRequired().HasMaxLength(512);
        b.Property(x => x.EventName).HasColumnName("event_name").IsRequired().HasMaxLength(128);
        b.Property(x => x.Payload).HasColumnName("payload").HasColumnType("jsonb").IsRequired();

        b.Property(x => x.OccurredOn).HasColumnName("occurred_on").IsRequired();
        b.Property(x => x.Status).HasColumnName("status").HasConversion<int>().IsRequired();
        b.Property(x => x.Attempts).HasColumnName("attempts").IsRequired();
        b.Property(x => x.LastError).HasColumnName("last_error");
        b.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        b.Property(x => x.ProcessedAt).HasColumnName("processed_at");
        b.Property(x => x.NextAttemptAt).HasColumnName("next_attempt_at");
        b.Property(x => x.CorrelationId).HasColumnName("correlation_id");

        b.HasIndex(x => new { x.Status, x.NextAttemptAt }).HasDatabaseName("ix_outbox_status_nextattempt");
        b.HasIndex(x => x.OccurredOn).HasDatabaseName("ix_outbox_occurred");
    }
}
