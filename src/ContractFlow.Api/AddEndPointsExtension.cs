using ContractFlow.Api.Requests;
using ContractFlow.Application.Commands;
using MediatR;

namespace ContractFlow.Api
{
    public static class AddEndPointsExtension
    {
        public static WebApplication AddEndPoints(this WebApplication app)
        {
            app.MapGet("/", () => Results.Ok(new
            {
                service = "contractflow",
                version = "0.1.0",
                status = "ok"
            }));


            app.MapPost("/contracts", async (
                CreateContractRequest req,
                ISender sender,
                CancellationToken ct) =>
            {
                try
                {                    
                    var cmd = new CreateContractCommand(req.CustomerId, req.PlanId, DateOnly.FromDateTime(req.StartDate));
                    var result = await sender.Send(cmd, ct);

                    return Results.Created($"/contracts/{result.ContractId}", result);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(new { error = ex.Message });
                }
            });



            return app;
        }
    }
}