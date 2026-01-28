namespace Yggdrasil.Quotation.Application.Features.Settings.DTOs;

public class PlanPaymentTermViewDto
{
    public int PlanId { get; set; }
    // PaymentTerm info
    public int PaymentTermId { get; set; }
    public string PaymentTermName { get; set; } = string.Empty;
    public string PaymentTermCode { get; set; } = string.Empty;
    public int NumberOfPayments { get; set; }

    // InterestRate info
    public int InterestRateId { get; set; }
    public string InterestRateName { get; set; } = string.Empty;
    public decimal InterestRateAnnualPercentage { get; set; }    
    public int Order { get; set; }
}