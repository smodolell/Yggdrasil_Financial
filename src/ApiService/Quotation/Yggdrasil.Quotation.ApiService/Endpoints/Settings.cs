using Ardalis.Result.AspNetCore;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Yggdrasil.Quotation.ApiService.Infrastructure;
using Yggdrasil.Quotation.Application.Features.Settings.Commands.CreatePlan;
using Yggdrasil.Quotation.Application.Features.Settings.Commands.UpdatePlan;
using Yggdrasil.Quotation.Application.Features.Settings.DTOs;
using Yggdrasil.Quotation.Application.Features.Settings.Queries;
using Yggdrasil.Quotation.Application.Features.Settings.Queries.GetPlanById;

namespace Yggdrasil.Quotation.ApiService.Endpoints;

public class Settings : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        var group = groupBuilder.MapGroup("/api/settings")
           .WithTags("Settings");

        group.MapGet("/plan/{id}", GetPlanById)
            .WithName("GetPlanById")
            .WithSummary("Obtiene un plan por su ID")
            .WithDescription("Retorna los detalles de un plan específico incluyendo sus términos de pago")
            .Produces<PlanViewDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPost("/plan", CreatePlan)
            .WithName("CreatePlan")
            .WithSummary("Crea un nuevo plan")
            .WithDescription("Crea un nuevo plan con sus términos de pago asociados")
            .Accepts<PlanEditDto>("application/json")
            .Produces<int>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

        group.MapPut("/plan/{id}", UpdatePlan)
            .WithName("UpdatePlan")
            .WithSummary("Actualiza un plan existente")
            .WithDescription("Actualiza los datos de un plan y sus términos de pago")
            .Accepts<PlanEditDto>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
    }

    public async Task<IResult> CreatePlan(
        [FromServices] ICommandMediator commandMediator,
        [FromBody] PlanEditDto model)
    {
        var result = await commandMediator.SendAsync(new CreatePlanCommand(model));
        return result.ToMinimalApiResult();
    }

    public async Task<IResult> UpdatePlan(
        [FromServices] ICommandMediator commandMediator,
        int id,
        [FromBody] PlanEditDto model)
    {
        var result = await commandMediator.SendAsync(new UpdatePlanCommand(id,model));
        return result.ToMinimalApiResult();
    }

    public async Task<IResult> GetPlanById(
        [FromServices] IQueryMediator queryMediator,
        int id)
    {
        var result = await queryMediator.QueryAsync(new GetPlanByIdQuery(id));
        return result.ToMinimalApiResult();
    }
}