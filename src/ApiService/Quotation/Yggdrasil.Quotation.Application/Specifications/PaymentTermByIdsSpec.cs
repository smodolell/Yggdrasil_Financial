namespace Yggdrasil.Quotation.Application.Specifications;

public class PaymentTermByIdsSpec : Specification<PaymentTerm>
{
    public PaymentTermByIdsSpec(List<int> ids)
    {
        Query.Where(p => ids.Contains(p.Id));
    }
}
