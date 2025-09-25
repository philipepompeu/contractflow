namespace ContractFlow.Infrastructure.Outbox;

public enum OutboxStatus { Pending = 0, Processing = 1, Sent = 2, Failed = 3 }

public sealed class OutboxMessage
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string EventType { get; private set; } = default!;           // AssemblyQualifiedName do evento
    public string EventName { get; private set; } = default!;           // Nome lógico (ex.: ContractCreated)
    public string Payload { get; private set; } = default!;             // JSON
    public DateTimeOffset OccurredOn { get; private set; }
    public OutboxStatus Status { get; private set; } = OutboxStatus.Pending;
    public int Attempts { get; private set; } = 0;
    public string? LastError { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ProcessedAt { get; private set; }
    public DateTimeOffset? NextAttemptAt { get; private set; }
    public string? CorrelationId { get; private set; } 

    private OutboxMessage() { }

    public static OutboxMessage Create(string eventType, string eventName, string payload, DateTimeOffset occurredOn, string? correlationId)
        => new()
        {
            EventType = eventType,
            EventName = eventName,
            Payload = payload,
            OccurredOn = occurredOn,
            CorrelationId = correlationId
        };

    // Métodos de controle (usaremos no HostedService depois)    
    public void MarkSent() { Status = OutboxStatus.Sent; ProcessedAt = DateTimeOffset.UtcNow; }
    public void MarkFailed(string error, TimeSpan backoff)
    {
        Status = OutboxStatus.Failed; // ou voltar para Pending, se preferir retry simples
        Attempts++;
        LastError = error;
        NextAttemptAt = DateTimeOffset.UtcNow.Add(backoff);
    }

    public void MarkProcessing() => Status = OutboxStatus.Processing;
}
