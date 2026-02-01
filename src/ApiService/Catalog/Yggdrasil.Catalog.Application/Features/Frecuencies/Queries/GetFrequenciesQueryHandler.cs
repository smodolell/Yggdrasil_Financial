using Yggdrasil.Catalog.Application.Features.Frecuencies.DTOs;
using Yggdrasil.Catalog.Application.Interfaces;
using Yggdrasil.Catalog.Application.Specifications;

namespace Yggdrasil.Catalog.Application.Features.Frecuencies.Queries;

internal class GetFrequenciesQueryHandler : IQueryHandler<GetFrequenciesQuery, PagedResult<List<FrequencyListItemDto>>>
{
    private readonly IFrequencyRepository _frequencyRepository;
    private readonly IPaginator _paginator;
    private readonly IDynamicSorter _sorter;

    public GetFrequenciesQueryHandler(IFrequencyRepository frequencyRepository, IPaginator paginator, IDynamicSorter sorter)
    {
        _frequencyRepository = frequencyRepository;
        _paginator = paginator;
        _sorter = sorter;
    }
    public async Task<PagedResult<List<FrequencyListItemDto>>> HandleAsync(GetFrequenciesQuery message, CancellationToken cancellationToken = default)
    {
        var spec = new FrequencySpec(message.SearchText,null);

        var query = _frequencyRepository.ApplySpecification(spec);

        query = _sorter.ApplySort(query, message.SortColumn, message.SortDescending);

        return await _paginator.PaginateAsync<PaymentFrequency, FrequencyListItemDto>(
            query,
            message.Page,
            message.PageSize,
            cancellationToken
        );
    }
}
