using Yggdrasil.Quotation.Application.Repositories;
using Yggdrasil.Quotation.Domain.Entities;

namespace Yggdrasil.Quotation.Infrastructure.Persistence.Repositories;

internal class TaxRateRepository : Repository<TaxRate, int> , ITaxRateRepository
{
    public TaxRateRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

}
