
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Yggdrasil.Credit.ApiService.Infrastructure;

namespace Yggdrasil.Credit.ApiService.Endpoints;



public class Products : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet("/", GetProducts)
            .WithSummary("Obtiene la lista de productos dummy")
            .WithDescription("Retorna una lista estática de productos para probar la interfaz de Scalar")
            .Produces<List<ProductDto>>(StatusCodes.Status200OK);

        groupBuilder.MapPost("/", CreateProduct)
            .WithSummary("Crea un producto de prueba")
            .WithDescription("Recibe un objeto y retorna un ID falso 201")
            .Produces<int>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    // Método GET Dummy
    public Ok<List<ProductDto>> GetProducts()
    {
        var list = new List<ProductDto>
        {
            new(1, "Producto Alpha", 99.99m),
            new(2, "Producto Beta", 150.00m)
        };
        return TypedResults.Ok(list);
    }

    // Método POST Dummy
    public Created<int> CreateProduct([FromBody] CreateProductRequest request)
    {
        // Simulamos que guardamos algo
        var fakeId = new Random().Next(1, 1000);
        return TypedResults.Created($"/products/{fakeId}", fakeId);
    }
}

// DTOs rápidos para la prueba
public record ProductDto(int Id, string Name, decimal Price);
public record CreateProductRequest(string Name, decimal Price);
