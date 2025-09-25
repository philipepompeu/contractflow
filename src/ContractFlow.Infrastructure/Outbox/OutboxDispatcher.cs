using System.Diagnostics;
using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ContractFlow.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace ContractFlow.Infrastructure.Outbox;

public sealed class OutboxDispatcher(
    IServiceProvider sp,
    IOptions<OutboxOptions> options,
    ILogger<OutboxDispatcher> log) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = sp;
    private readonly OutboxOptions _opt = options.Value;
    private readonly ILogger<OutboxDispatcher> _log = log;

    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);
    private static readonly ActivitySource Source = new("ContractFlow.Outbox");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation("OutboxDispatcher iniciado: batch={Batch}, interval={Interval}s",
            _opt.BatchSize, _opt.IntervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ContractFlowDbContext>();
                var publisher = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                var now = DateTimeOffset.UtcNow;

                // Lê um lote de mensagens pendentes (prontas para reprocesso)
                var batch = await db.OutboxMessages
                    .Where(x => x.Status == OutboxStatus.Pending &&
                                (x.NextAttemptAt == null || x.NextAttemptAt <= now))
                    .OrderBy(x => x.OccurredOn)
                    .Take(_opt.BatchSize)
                    .ToListAsync(stoppingToken);

                if (batch.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(_opt.IntervalSeconds), stoppingToken);
                    continue;
                }

                // Marcar como Processing para evitar pegas múltiplas (single instance já resolve)
                foreach (var msg in batch) msg.MarkProcessing();
                    await db.SaveChangesAsync(stoppingToken);

                foreach (var msg in batch)
                {
                    try
                    {
                        var type = ResolveType(msg.EventType);
                        if (type is null)
                            throw new InvalidOperationException($"Tipo não resolvido: {msg.EventType}");

                        var payload = JsonSerializer.Deserialize(msg.Payload, type, _json)
                                      ?? throw new InvalidOperationException("Payload inválido");

                        using (Serilog.Context.LogContext.PushProperty("correlation_id", msg.CorrelationId))
                        {
                            _log.LogInformation("Lendo mensagem: {message}", msg.Payload);

                            await publisher.Publish(payload, sendContext =>
                            {
                                sendContext.Headers.Set("CorrelationId", msg.CorrelationId);
                            },stoppingToken);

                            msg.MarkSent();
                            _log.LogInformation("Outbox {Id} publicado ({EventName})", msg.Id, msg.EventName);
                        }
                    }
                    catch (Exception ex)
                    {
                        // backoff simples + contagem de tentativas
                        msg.MarkFailed(ex.Message, TimeSpan.FromSeconds(_opt.BackoffSeconds));
                        _log.LogWarning(ex, "Falha ao publicar outbox {Id} ({EventName}), tentativa={Attempts}",
                            msg.Id, msg.EventName, msg.Attempts);

                        if (msg.Attempts >= _opt.MaxAttempts)
                        {
                            _log.LogError("Outbox {Id} excedeu MaxAttempts e permanece como Failed", msg.Id);
                        }
                    }

                    await db.SaveChangesAsync(stoppingToken);
                }
            }
            catch (TaskCanceledException) { /* shutting down */ }
            catch (Exception ex)
            {
                _log.LogError(ex, "Erro no loop do OutboxDispatcher");
                // pausa curta para evitar loop quente em erro sistemático
                await Task.Delay(TimeSpan.FromSeconds(Math.Max(3, _opt.IntervalSeconds)), stoppingToken);
            }
        }

        _log.LogInformation("OutboxDispatcher finalizado.");
    }
    
    private static Type? ResolveType(string typeNameOrAqn)
    {
        // 1) Tenta resolver direto (suporta AQN "Namespace.Tipo, Assembly, Version=...")
        var t = Type.GetType(typeNameOrAqn, throwOnError: false);
        if (t is not null) return t;

        // 2) Se veio só FullName ou o AQN não resolveu, varre assemblies carregados
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            t = asm.GetType(typeNameOrAqn, throwOnError: false);
            if (t is not null) return t;
        }
        return null;
    }
}
