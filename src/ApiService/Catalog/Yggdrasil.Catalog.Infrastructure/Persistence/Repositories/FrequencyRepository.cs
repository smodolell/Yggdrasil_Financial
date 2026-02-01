using Yggdrasil.Catalog.Application.Interfaces;
using Yggdrasil.Catalog.Domain.Entities;
using Yggdrasil.Catalog.Infrastructure.Persistence;

namespace Yggdrasil.Catalog.Infrastructure.Persistence.Repositories;

public class FrequencyRepository : Repository<PaymentFrequency, int>, IFrequencyRepository
{
    public FrequencyRepository(ApplicationDbContext context) : base(context)
    {
    }
}