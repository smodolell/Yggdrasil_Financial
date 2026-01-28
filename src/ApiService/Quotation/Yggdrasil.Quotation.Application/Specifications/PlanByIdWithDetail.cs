namespace Yggdrasil.Quotation.Application.Specifications;

public class PlanByIdWithDetail : Specification<Plan>
{
    public PlanByIdWithDetail(int id)
    {
        Query.Where(p => p.Id == id);
        Query.Include(p => p.PlanPaymentTerms)
                .ThenInclude(ppt => ppt.PaymentTerm)
            .Include(p => p.PlanPaymentTerms)
               .ThenInclude(ppt => ppt.InterestRate)
           .AsNoTracking();
    }
}
