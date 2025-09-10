using ContractFlow.Api;
using ContractFlow.Application.Commands;
using ContractFlow.Infrastructure;
using MediatR;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration)
       .Enrich.FromLogContext()
       .WriteTo.Console();
});
builder.WebHost.ConfigureKestrel(o => o.AddServerHeader = false);

builder.Services.AddInfrastructurePersistence(builder.Configuration);
builder.Services.AddMediatR(typeof(CreateContractCommand).Assembly);
builder.Services.AddInfrastructureMessaging(builder.Configuration);

builder.Services.AddOutboxDispatcher(builder.Configuration);

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
    };
});


var app = builder.Build();

app.AddEndPoints();

app.UseSerilogRequestLogging();
//app.UseExceptionHandler(e =>);

app.Run();
