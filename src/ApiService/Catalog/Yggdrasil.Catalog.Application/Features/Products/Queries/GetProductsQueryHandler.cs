using Yggdrasil.Catalog.Application.Features.Products.DTOs;


namespace Yggdrasil.Catalog.Application.Features.Products.Queries;

internal class GetProductsQueryHandler(
    IProductRepository productRepository,
    IPaginator paginator,
    IDynamicSorter sorter
) : IQueryHandler<GetProductsQuery, PagedResult<List<ProductDto>>>
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IPaginator _paginator = paginator;
    private readonly IDynamicSorter _sorter = sorter;

    public async Task<PagedResult<List<ProductDto>>> HandleAsync(GetProductsQuery message, CancellationToken cancellationToken = default)
    {

        var spec = new ProductSpec(
            message.SearchText            
        );

        var query = _productRepository.ApplySpecification(spec);

        query = _sorter.ApplySort(query, message.SortColumn, message.SortDescending);

        return await _paginator.PaginateAsync<Product, ProductDto>(
            query,
            message.Page,
            message.PageSize,
            cancellationToken
        );
    }
}
