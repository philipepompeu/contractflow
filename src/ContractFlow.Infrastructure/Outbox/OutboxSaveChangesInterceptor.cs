using System.Text.Json;
using ContractFlow.Domain.Common;
using ContractFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ContractFlow.Infrastructure.Outbox;

public sealed class OutboxSaveChangesInterceptor : SaveChangesInterceptor
{
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false
    };

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        var ctx = (ContractFlowDbContext?)eventData.Context;
        if (ctx is null) return base.SavingChangesAsync(eventData, result, ct);

        // Captura entidades com eventos
        var entriesWithEvents = ctx.ChangeTracker
            .Entries()
            .Where(e => e.Entity is IHasDomainEvents aggregate && aggregate.DomainEvents.Count > 0)
            .ToList();

        if (entriesWithEvents.Count == 0)
            return base.SavingChangesAsync(eventData, result, ct);

        foreach (var entry in entriesWithEvents)
        {
            var agg = (IHasDomainEvents)entry.Entity;
            foreach (var evt in agg.DomainEvents)
            {
                // serializa o evento de domínio e empilha no outbox
                var type = evt.GetType();
                var eventType = type.AssemblyQualifiedName!;               // para re-hidratar no dispatcher
                var eventName = type.Name;                                  // nome lógico
                var payload = JsonSerializer.Serialize(evt, type, _json);
                var occurred = evt.OccurredOn;

                ctx.OutboxMessages.Add(OutboxMessage.Create(eventType, eventName, payload, occurred));
            }

            // muito importante: limpa os eventos do agregado (idempotência de persitência)
            agg.ClearDomainEvents();
        }

        return base.SavingChangesAsync(eventData, result, ct);
    }
}
