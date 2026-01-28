namespace Yggdrasil.Quotation.Application.Specifications;

public class InterestRateExpired : Specification<InterestRate>
{
    public InterestRateExpired()
    {
        Query.Where(rate => rate.IsActive && rate.ExpirationDate.HasValue && rate.ExpirationDate < DateTime.Today);
    }
}
