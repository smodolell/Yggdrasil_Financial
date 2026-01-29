namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands.CreateTaxRate;

public class CreateTaxRateCommand : ICommand<Result<int>>
{
    public string Name { get; set; } = string.Empty; // "IVA", "IGV", "GST"
    public string Code { get; set; } = string.Empty; // "IVA16", "IGV18"
    public decimal Percentage { get; set; } // 16.0, 18.0
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsActive { get; set; } = true;
}
