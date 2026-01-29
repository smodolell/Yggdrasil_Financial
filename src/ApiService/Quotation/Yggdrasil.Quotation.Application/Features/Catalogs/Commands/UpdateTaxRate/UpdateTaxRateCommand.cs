namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands.UpdateTaxRate;

public class UpdateTaxRateCommand : ICommand<Result>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsActive { get; set; }
}
