namespace Yggdrasil.Quotation.Domain.Entities;

public class Plan : BaseEntity<int>
{
    public string Name { get; set; }
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public virtual ICollection<PlanPaymentTerm> PlanTerms { get; set; } = new HashSet<PlanPaymentTerm>();
}
