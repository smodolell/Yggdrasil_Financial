using Ardalis.Result;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Yggdrasil.Credit.Application.Common.Interfaces;

namespace Yggdrasil.Credit.Infrastructure.Common.Services;

public sealed class Paginator : IPaginator
{
    public Paginator()
    {
        // Constructor vacío ya que Mapster no requiere una instancia de IMapper
    }

    // Implementación explícita para garantizar coincidencia de restricciones
    async Task<PagedResult<List<TDestination>>> IPaginator.PaginateAsync<T, TDestination>(
        IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ProjectToType<TDestination>() // Mapster no requiere ConfigurationProvider
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(total / (double)pageSize); // Corrección para cálculo de páginas

        var pageInfo = new PagedInfo(pageNumber, pageSize, totalPages, total);
        var pagedResult = new PagedResult<List<TDestination>>(pageInfo, items);
        return pagedResult;
    }

    public PagedResult<List<T>> CreatePagedResult<T>(
        List<T> items,
        int pageNumber,
        int pageSize,
        long totalCount)
    where T : class
    {
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var pageInfo = new PagedInfo(pageNumber, pageSize, totalPages, totalCount);
        return new PagedResult<List<T>>(pageInfo, items);
    }
}