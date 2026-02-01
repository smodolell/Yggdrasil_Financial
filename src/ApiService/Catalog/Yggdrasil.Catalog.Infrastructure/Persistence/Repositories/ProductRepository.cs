using Yggdrasil.Catalog.Application.Interfaces;
using Yggdrasil.Catalog.Domain.Entities;

namespace Yggdrasil.Catalog.Infrastructure.Persistence.Repositories;

public class ProductRepository : Repository<Product, Guid>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }
}
