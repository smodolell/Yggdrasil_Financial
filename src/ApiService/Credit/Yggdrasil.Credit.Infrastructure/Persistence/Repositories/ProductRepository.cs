using Yggdrasil.Credit.Application.Interfaces;
using Yggdrasil.Credit.Domain.Entities;

namespace Yggdrasil.Credit.Infrastructure.Persistence.Repositories;

public class ProductRepository : Repository<Product, Guid>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }
}
