using Yggdrasil.Quotation.Application.Features.Catalogs.DTOs;
using Yggdrasil.Quotation.Application.Repositories;
using Yggdrasil.Quotation.Application.Specifications;

namespace Yggdrasil.Quotation.Application.Features.Catalogs.Queries;

internal class GetInterestRatesQueryHandler(
    IInterestRateRepository interestRateRepository,
    IPaginator paginator,
    IDynamicSorter sorter
) : IQueryHandler<GetInterestRatesQuery, PagedResult<List<InterestRateDto>>>
{
    private readonly IInterestRateRepository _interestRateRepository = interestRateRepository;
    private readonly IPaginator _paginator = paginator;
    private readonly IDynamicSorter _sorter = sorter;

    public async Task<PagedResult<List<InterestRateDto>>> HandleAsync(GetInterestRatesQuery message, CancellationToken cancellationToken = default)
    {
       

        var spec = new InterestRateSpec(
            message.SearchText,
            message.IsActive,
            message.EffectiveDateFrom,
            message.EffectiveDateTo,
            message.ExpirationDateFrom, message.ExpirationDateTo,
            message.AnnualPercentageFrom, message.AnnualPercentageTo
        );

        var query = _interestRateRepository.ApplySpecification(spec);

        query = _sorter.ApplySort(query, message.SortColumn, message.SortDescending);

        return await _paginator.PaginateAsync<InterestRate, InterestRateDto>(
            query,
            message.Page,
            message.PageSize,
            cancellationToken
        );
    }
}


