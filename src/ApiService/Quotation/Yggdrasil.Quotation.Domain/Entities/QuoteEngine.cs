using Yggdrasil.Quotation.Domain.Enums;

namespace Yggdrasil.Quotation.Domain.Entities;

public class QuoteEngine : BaseEntity<Guid>
{
    /// <summary>
    ///  Frecuencia de pagos
    /// </summary>
    public int FrequencyId { get; set; }


    /// <summary>
    /// Plazo (en número de períodos)
    /// </summary>
    public int Term { get; set; }

    /// <summary>
    ///  Tasa de interés
    /// </summary>
    public decimal Rate { get; set; }

    /// <summary>
    /// Tasa de Impuesto (e.g., IVA Rate)
    /// </summary>
    public decimal TaxRate { get; set; }
    public decimal InsuranceRate { get; set; }
    public decimal InsurancePercentage { get; set; }
    public decimal Amount { get; set; }
    public QuoteEngineStatus Status { get; set; }
    public ICollection<AmortizationSchedule> AmortizationSchedules { get; set; } = new HashSet<AmortizationSchedule>();
}
