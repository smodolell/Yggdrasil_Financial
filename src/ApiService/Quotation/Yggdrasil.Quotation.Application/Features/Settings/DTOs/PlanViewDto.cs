namespace Yggdrasil.Quotation.Application.Features.Settings.DTOs;

public class PlanViewDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<PlanPaymentTermViewDto> PaymentTerms { get; set; } = new();
}
