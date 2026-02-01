namespace Yggdrasil.Catalog.Application.Features.Products.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool Active { get; set; }
}
