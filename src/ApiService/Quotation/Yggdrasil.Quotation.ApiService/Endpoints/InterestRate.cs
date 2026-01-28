using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Threading;
using Yggdrasil.Quotation.ApiService.Infrastructure;
using Yggdrasil.Quotation.Application.Features.Catalogs.Commands;
using Yggdrasil.Quotation.Application.Features.Catalogs.DTOs;
using Yggdrasil.Quotation.Application.Features.Catalogs.Queries;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Yggdrasil.Quotation.ApiService.Endpoints;

public class InterestRate : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost("/", CreateInterestRate)
            .WithName("CreateInterestRate");

        groupBuilder.MapGet("/", GetInterestRates)
            .Produces<PagedResult<List<InterestRateDto>>>(StatusCodes.Status200OK);
    }

    [EndpointSummary("Crear tasa de interés")]
    [EndpointDescription("Crea una nueva tasa de interés con los datos proporcionados")]
    public async Task<IResult> CreateInterestRate(
        ICommandMediator commandMediator,
        CreateInterestRateCommand command
    )
    {
        var result = await commandMediator.SendAsync(command);
        return result.ToMinimalApiResult();
    }


    [EndpointSummary("Obtener tasas de interés")]
    [EndpointDescription("Obtiene un listado paginado y filtrado de las tasas registradas")]
    public async Task<IResult> GetInterestRates(
       IQueryMediator queryMediator,
       IValidator<GetInterestRatesQuery> validator,
      [AsParameters] GetInterestRatesQuery query
    )
    {
        //var validatorResult = await validator.ValidateAsync(query);

        //if (!validatorResult.IsValid)
        //{
        //    return Result.Invalid(validatorResult.AsErrors()).ToMinimalApiResult();
        //}
        var result = await queryMediator.QueryAsync(query);

        if (result.IsSuccess)
        {
            return Results.Ok(result);
        }

        return result.ToMinimalApiResult();
    }

}
