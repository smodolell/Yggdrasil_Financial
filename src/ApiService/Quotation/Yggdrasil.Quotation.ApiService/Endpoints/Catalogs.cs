using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Yggdrasil.Quotation.ApiService.Infrastructure;
using Yggdrasil.Quotation.Application.Features.Catalogs.Commands;
using Yggdrasil.Quotation.Application.Features.Catalogs.DTOs;
using Yggdrasil.Quotation.Application.Features.Catalogs.Queries;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Yggdrasil.Quotation.ApiService.Endpoints;

public class Catalogs : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        var group = groupBuilder.MapGroup("/api/catalogs")
            .WithTags("Catalogs");

        // Interest Rate Endpoints
        group.MapPost("/interest-rate", CreateInterestRate)
            .WithName("CreateInterestRate")
            .WithSummary("Crear tasa de interés")
            .WithDescription("Crea una nueva tasa de interés con los datos proporcionados")
            .Accepts<CreateInterestRateCommand>("application/json")
            .Produces<int>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

        group.MapGet("/interest-rate", GetInterestRates)
            .WithName("GetInterestRates")
            .WithSummary("Obtener tasas de interés")
            .WithDescription("Obtiene un listado paginado y filtrado de las tasas registradas")
            .Produces<PagedResult<List<InterestRateDto>>>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest);

        // Payment Term Endpoints
        group.MapPost("/payment-term", CreatePaymentTerm)
            .WithName("CreatePaymentTerm")
            .WithSummary("Crear término de pago")
            .WithDescription("Permite registrar términos de pago validados por el negocio")
            .Accepts<CreatePaymentTermCommand>("application/json")
            .Produces<int>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

        group.MapGet("/payment-term", GetAllPaymentTerm)
            .WithName("GetAllPaymentTerm")
            .WithSummary("Obtener todos los términos de pago")
            .WithDescription("Retorna una lista paginada de términos de pago")
            .Produces<PagedResult<List<PaymentTermDto>>>(StatusCodes.Status200OK)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest);

        group.MapPut("/payment-term/{id}/toggle-status", TogglePaymentTermStatus)
            .WithName("TogglePaymentTermStatus")
            .WithSummary("Activar/desactivar término de pago")
            .WithDescription("Cambia el estado activo/inactivo de un término de pago")
            .Accepts<TogglePaymentTermStatusCommand>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
    }

    #region Interest Rate

    public async Task<IResult> CreateInterestRate(
        [FromServices] ICommandMediator commandMediator,
        [FromBody] CreateInterestRateCommand command)
    {
        var result = await commandMediator.SendAsync(command);
        return result.ToMinimalApiResult();
    }

    public async Task<IResult> GetInterestRates(
        [FromServices] IQueryMediator queryMediator,
        [FromServices] IValidator<GetInterestRatesQuery> validator,
        [AsParameters] GetInterestRatesQuery query)
    {
        var validationResult = await validator.ValidateAsync(query);

        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors())
                .ToMinimalApiResult();
        }

        var result = await queryMediator.QueryAsync(query);
        return result.ToMinimalApiResult();
    }

    #endregion

    #region Payment Term

    public async Task<IResult> CreatePaymentTerm(
        [FromServices] ICommandMediator commandMediator,
        [FromBody] CreatePaymentTermCommand command)
    {
        var result = await commandMediator.SendAsync(command);
        return result.ToMinimalApiResult();
    }

    public async Task<IResult> GetAllPaymentTerm(
        [FromServices] IQueryMediator queryMediator
    )
    {
        var result = await queryMediator.QueryAsync(new GetAllPaymentTermQuery());
        return result.ToMinimalApiResult();
    }

    public async Task<IResult> TogglePaymentTermStatus(
        [FromServices] ICommandMediator commandMediator,
        int id,
        [FromBody] TogglePaymentTermStatusCommand command)
    {
        if (id != command.PaymentTermId)
        {
            return Results.BadRequest("El ID de la ruta no coincide con el ID del comando");
        }

        var result = await commandMediator.SendAsync(command);
        return result.ToMinimalApiResult();
    }

    #endregion
}