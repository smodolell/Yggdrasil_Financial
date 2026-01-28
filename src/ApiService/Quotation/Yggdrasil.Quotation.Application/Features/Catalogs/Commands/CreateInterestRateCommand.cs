namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands;


/// <summary>
/// Comando para crear una tasa de interés.
/// </summary>
/// <param name="AnnualPercentage">Porcentaje anual (Ej: 15.5). <example>15.5</example></param>
/// <param name="EffectiveDate">Fecha de vigencia. <example>2024-05-20</example></param>
/// <param name="ExpirationDate">Fecha de vencimiento opcional. <example>2025-05-20</example></param>
public record CreateInterestRateCommand(
    decimal AnnualPercentage,
    DateTime EffectiveDate,
    DateTime? ExpirationDate
) : ICommand<Result<int>>
{
    /// <summary>Nombre de la tasa.</summary>
    /// <example>Tasa VIP</example>
    public string RateName { get; set; } = string.Empty;

    /// <summary>Indica si está activa.</summary>
    /// <example>true</example>
    public bool IsActive { get; set; } = true;
}