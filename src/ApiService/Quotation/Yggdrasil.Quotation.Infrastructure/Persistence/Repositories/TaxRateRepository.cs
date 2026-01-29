using Microsoft.EntityFrameworkCore;
using Yggdrasil.Quotation.Application.Repositories;
using Yggdrasil.Quotation.Domain.Entities;

namespace Yggdrasil.Quotation.Infrastructure.Persistence.Repositories;

internal class TaxRateRepository : Repository<TaxRate, int> , ITaxRateRepository
{
    public TaxRateRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
                .AsNoTracking()
                .AnyAsync(x => x.Code == code.ToUpper(), cancellationToken);
    }

    public async Task<TaxRate?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
               .AsNoTracking()
               .FirstOrDefaultAsync(x => x.Code == code.ToUpper(), cancellationToken);
    }


    public async Task<IEnumerable<TaxRate>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(x => x.Name == name)
            .ToListAsync(cancellationToken);
    }
    public async Task<bool> CodeExistsForOtherIdAsync(int id, string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(x => x.Code == code.ToUpper() && x.Id != id, cancellationToken);
    }
    public async Task<IEnumerable<TaxRate>> GetOverlappingTaxRatesAsync(
      DateTime effectiveDate,
      DateTime? expirationDate,
      int? excludeId = null,
      CancellationToken cancellationToken = default)
    {
        // Si expirationDate es null, lo tratamos como DateTime.MaxValue
        var maxExpiration = expirationDate ?? DateTime.MaxValue;

        var query = _dbSet
            .AsNoTracking()
            .Where(x => x.IsActive) // Solo tasas activas
            .Where(x => !excludeId.HasValue || x.Id != excludeId.Value) // Excluir un ID específico si se proporciona
            .Where(x =>
                // Caso 1: La nueva tasa empieza durante una tasa existente
                (effectiveDate >= x.EffectiveDate && effectiveDate <= (x.ExpirationDate ?? DateTime.MaxValue)) ||

                // Caso 2: La nueva tasa termina durante una tasa existente
                (maxExpiration >= x.EffectiveDate && maxExpiration <= (x.ExpirationDate ?? DateTime.MaxValue)) ||

                // Caso 3: La nueva tasa cubre completamente una tasa existente
                (effectiveDate <= x.EffectiveDate && maxExpiration >= (x.ExpirationDate ?? DateTime.MaxValue)) ||

                // Caso 4: La nueva tasa está completamente dentro de una tasa existente
                (effectiveDate >= x.EffectiveDate && maxExpiration <= (x.ExpirationDate ?? DateTime.MaxValue))
            );

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(x => x.Id == id, cancellationToken);
    }
}
