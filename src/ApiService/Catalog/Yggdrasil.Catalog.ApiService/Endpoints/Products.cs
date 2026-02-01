using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using LiteBus.Commands.Abstractions;
using LiteBus.Queries.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Yggdrasil.Catalog.ApiService.Infrastructure;
using Yggdrasil.Catalog.Application.Features.Products.Commands;
using Yggdrasil.Catalog.Application.Features.Products.DTOs;
using Yggdrasil.Catalog.Application.Features.Products.Queries;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Yggdrasil.Catalog.ApiService.Endpoints;

public class Products : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        var group = groupBuilder.MapGroup("/api/products")
            .WithTags("Products");

        group.MapPost("/", CreateProduct)
            .WithName("CreateProduct")
            .WithSummary("Crear Producto")
            .WithDescription("Crea una nuevo Producto")
            .Accepts<CreateProductCommand>("application/json")
            .Produces<int>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

        group.MapGet("/", GetProducts)
         .WithName("GetProducts")
         .WithSummary("Obtener Productos")
         .WithDescription("Obtiene un listado paginado y filtrado de las productos registradas")
         .Produces<PagedResult<List<ProductDto>>>(StatusCodes.Status200OK)
         .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest);


    }

    public async Task<IResult> CreateProduct(
    [FromServices] ICommandMediator commandMediator,
    [FromBody] CreateProductCommand command)
    {
        var result = await commandMediator.SendAsync(command);
        return result.ToMinimalApiResult();
    }

    public async Task<IResult> GetProducts(
        [FromServices] IQueryMediator queryMediator,
        [FromServices] IValidator<GetProductsQuery> validator,
        [AsParameters] GetProductsQuery query)
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
}
