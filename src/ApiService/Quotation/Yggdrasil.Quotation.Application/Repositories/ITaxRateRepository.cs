
namespace Yggdrasil.Quotation.Application.Repositories;

public interface ITaxRateRepository : IRepository<TaxRate, int>
{

    Task<TaxRate?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaxRate>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> CodeExistsForOtherIdAsync(int id, string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaxRate>> GetOverlappingTaxRatesAsync(
         DateTime effectiveDate,
         DateTime? expirationDate,
         int? excludeId = null,
         CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
}
