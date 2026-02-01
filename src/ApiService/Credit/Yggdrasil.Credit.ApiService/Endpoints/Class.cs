namespace Yggdrasil.Credit.ApiService.Endpoints;

// DTOs rápidos para la prueba
public record ProductDto(int Id, string Name, decimal Price);
public record CreateProductRequest(string Name, decimal Price);
