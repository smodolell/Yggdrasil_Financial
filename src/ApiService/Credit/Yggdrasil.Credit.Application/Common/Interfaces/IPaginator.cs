using Ardalis.Result;

namespace Yggdrasil.Credit.Application.Common.Interfaces;

public interface IPaginator
{
    Task<PagedResult<List<TDestination>>> PaginateAsync<T, TDestination>(
        IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
        where T : class
        where TDestination : class;

    PagedResult<List<T>> CreatePagedResult<T>(
        List<T> items,
        int pageNumber,
        int pageSize,
        long totalCount)
        where T : class;
}
