namespace Yggdrasil.Quotation.Application.Features.Settings.DTOs;

public class PlanPaymentTermEditDto
{
    public int PlanId { get; set; }
    public int PaymentTermId { get; set; }
    public int InterestRateId { get; set; }

    public int Order { get; set; }
}
