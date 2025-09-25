using Serilog.Context;
using System.Diagnostics;

namespace ContractFlow.Api.Observability;

public class CorrelationMiddleware: IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Já haverá Activity em ASP.NET Core 8, mas garantimos fallback
        var traceId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");

        // Ecoa para o cliente e enriquece log
        context.Response.Headers["X-Correlation-ID"] = traceId;
        using (LogContext.PushProperty("correlation_id", traceId))
        {
            await next(context);
        }
    }
}