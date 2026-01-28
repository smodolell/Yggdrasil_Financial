namespace Yggdrasil.Quotation.Domain.Entities;

public class PlanPaymentTerm : BaseEntity<int>
{
    public int PlanId { get; set; }
    public int PaymentTermId { get; set; }
    public int InterestRateId { get; set; }

    public int Order { get; set; }
    public Plan QuotationPlan { get; set; } = null!;
    public PaymentTerm PaymentTerm { get; set; } = null!;
    public InterestRate InterestRate { get; set; } = null!;
}
