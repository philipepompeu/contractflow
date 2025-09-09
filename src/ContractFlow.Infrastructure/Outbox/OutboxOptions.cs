namespace ContractFlow.Infrastructure.Outbox;

public sealed class OutboxOptions
{
    public int BatchSize { get; set; } = 100;      // quantas mensagens por varredura
    public int IntervalSeconds { get; set; } = 5;  // intervalo entre varreduras
    public int MaxAttempts { get; set; } = 5;      // tentativas antes de falhar
    public int BackoffSeconds { get; set; } = 30;  // tempo até próxima tentativa ao falhar
}
