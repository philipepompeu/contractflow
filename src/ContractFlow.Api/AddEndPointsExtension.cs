using ContractFlow.Api.Requests;
using ContractFlow.Application.Commands;
using ContractFlow.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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


            //app.MapGroup("/approvals").MapGet("/")
            var group = app.MapGroup("/approvals");
                
                
            group.MapGet("/", async (ISender sender, CancellationToken ct, int page = 1, int size = 10) =>
            {
                var result = await sender.Send(new GetPendingApprovalsQuery(page, size), ct);
                
                return Results.Ok(result);
            });
            group.MapPut("/{id:guid}", async (ISender sender, CancellationToken ct, [FromRoute] Guid id) =>
            {
                var result = await sender.Send(new ApproveDocumentCommand(id), ct);
                
                return Results.Ok(result);
            });
            
            app.MapPost("/contracts", async (
                CreateContractRequest req,
                ISender sender,
                CancellationToken ct) =>
            {
                try
                {                    
                    var cmd = new CreateContractCommand(req.PartnerId, req.PlanId, DateOnly.FromDateTime(req.StartDate), req.Type, req.totalAmount);
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