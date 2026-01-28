namespace Yggdrasil.Quotation.Domain.Entities;

public class InterestRate : BaseEntity<int>
{
    public string RateName { get; set; } = string.Empty; // "Tasa Normal", "Tasa Promocional"
    public decimal AnnualPercentage { get; set; } // 16.5% se guarda como 16.5
    public decimal MonthlyPercentage => AnnualPercentage / 12;
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<PlanPaymentTerm> PlanPaymentTerms { get; set; } = new HashSet<PlanPaymentTerm>();
}

