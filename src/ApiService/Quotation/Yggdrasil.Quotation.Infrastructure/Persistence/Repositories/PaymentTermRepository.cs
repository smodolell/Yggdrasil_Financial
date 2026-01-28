using Microsoft.EntityFrameworkCore;
using Yggdrasil.Quotation.Application.Repositories;
using Yggdrasil.Quotation.Domain.Entities;

namespace Yggdrasil.Quotation.Infrastructure.Persistence.Repositories;

internal class PaymentTermRepository : Repository<PaymentTerm, int> ,IPaymentTermRepository
{
    public PaymentTermRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

}
