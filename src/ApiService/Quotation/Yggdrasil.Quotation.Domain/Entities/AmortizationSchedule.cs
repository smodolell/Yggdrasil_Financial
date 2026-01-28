namespace Yggdrasil.Quotation.Domain.Entities;

public class AmortizationSchedule : BaseEntity<Guid>
{
    public Guid QuoteEngineId { get; set; }
    /// <summary>
    /// Numero de Período
    /// </summary>
    public int PeriodNumber { get; set; }
    /// <summary>
    /// Fecha de Vencimiento
    /// </summary>
    public DateTime DueDate { get; set; }
    /// <summary>
    /// Capital a amortizar
    /// </summary>
    public decimal Principal { get; set; }
    public decimal Interest { get; set; }
    public decimal InterestTax { get; set; }

    /// <summary>
    /// Cuota programada total (Principal + Interest + InterestTax)
    /// </summary>
    public decimal TotalDue { get; set; }

    /// <summary>
    /// Saldos Pendientes (Balances)
    /// </summary>
    public decimal PrincipalBalance { get; set; }
    public decimal InterestBalance { get; set; }
    public decimal InterestTaxBalance { get; set; }
    /// <summary>
    /// Saldo total pendiente del crédito
    /// </summary>
    public decimal TotalBalance { get; set; }

    public virtual QuoteEngine QuoteEngine { get; set; } = null!;
}
