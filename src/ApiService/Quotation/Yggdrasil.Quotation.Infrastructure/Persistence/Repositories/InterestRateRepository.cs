using Microsoft.EntityFrameworkCore;
using Yggdrasil.Quotation.Application.Repositories;
using Yggdrasil.Quotation.Domain.Entities;

namespace Yggdrasil.Quotation.Infrastructure.Persistence.Repositories;

internal class InterestRateRepository : Repository<InterestRate, int>, IInterestRateRepository
{
    public InterestRateRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
    public async Task<bool> IsRateUsedInPlansAsync(int interestRateId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PlanPaymentTerms
            .AnyAsync(p => p.InterestRateId == interestRateId, cancellationToken);
    }

}
