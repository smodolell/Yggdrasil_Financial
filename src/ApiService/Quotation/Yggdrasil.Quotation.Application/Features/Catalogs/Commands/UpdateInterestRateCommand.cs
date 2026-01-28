namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands;

public class UpdateInterestRateCommand : ICommand<Result>
{
    public int Id { get; set; }
    public string RateName { get; set; } = string.Empty;
    public decimal AnnualPercentage { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsActive { get; set; }
}
