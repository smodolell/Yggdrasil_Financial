namespace Yggdrasil.Catalog.Application.Specifications;

public class ProductSpec : Specification<Product>
{

    public ProductSpec(string? searchText)
    {
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            Query.Where(r => r.Name.Contains(searchText));
        }

    }
}
