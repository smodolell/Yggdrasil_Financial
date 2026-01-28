namespace Yggdrasil.Quotation.Application.Features.Catalogs.DTOs;

public class InterestRateDto
{
    public int Id { get; set; }
    public string RateName { get; set; } = string.Empty; 
    public decimal AnnualPercentage { get; set; } 
    public decimal MonthlyPercentage => AnnualPercentage / 12;
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsActive { get; set; } = true;
}
