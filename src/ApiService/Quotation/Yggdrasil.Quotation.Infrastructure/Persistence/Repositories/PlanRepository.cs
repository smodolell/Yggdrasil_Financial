using Microsoft.EntityFrameworkCore;
using Yggdrasil.Quotation.Application.Repositories;
using Yggdrasil.Quotation.Domain.Entities;

namespace Yggdrasil.Quotation.Infrastructure.Persistence.Repositories;

internal class PlanRepository : Repository<Plan, int>, IPlanRepository
{
    public PlanRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task AddPlanPaymentTermAsync(PlanPaymentTerm planPaymentTerm, CancellationToken cancellationToken = default)
    {
        await _dbContext.PlanPaymentTerms.AddAsync(planPaymentTerm);
    }

    public async Task RemoveAllPlanPaymentTermsAsync(int planId, CancellationToken cancellationToken = default)
    {
        var removeItems = await _dbContext.PlanPaymentTerms
            .Where(r => r.PlanId == planId)
            .ToListAsync();
        _dbContext.PlanPaymentTerms.RemoveRange(removeItems); 
    }
}
