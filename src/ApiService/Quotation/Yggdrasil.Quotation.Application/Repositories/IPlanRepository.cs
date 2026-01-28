namespace Yggdrasil.Quotation.Application.Repositories;

public interface IPlanRepository : IRepository<Plan, int>
{
    Task AddPlanPaymentTermAsync(PlanPaymentTerm planPaymentTerm, CancellationToken cancellationToken = default);
    Task RemoveAllPlanPaymentTermsAsync(int planId , CancellationToken cancellationToken = default);
}
