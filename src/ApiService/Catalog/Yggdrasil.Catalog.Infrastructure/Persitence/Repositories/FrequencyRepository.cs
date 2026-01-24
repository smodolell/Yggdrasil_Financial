using Yggdrasil.Catalog.Application.Interfaces;
using Yggdrasil.Catalog.Domain.Entities;

namespace Yggdrasil.Catalog.Infrastructure.Persitence.Repositories;

public class FrequencyRepository : Repository<Frequency, int>, IFrequencyRepository
{
    public FrequencyRepository(ApplicationDbContext context) : base(context)
    {
    }
}