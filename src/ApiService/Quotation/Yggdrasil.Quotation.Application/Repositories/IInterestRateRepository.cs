
namespace Yggdrasil.Quotation.Application.Repositories;

public interface IInterestRateRepository : IRepository<InterestRate, int>
{

    Task<bool> IsRateUsedInPlansAsync(int interestRateId, CancellationToken cancellationToken = default);
}