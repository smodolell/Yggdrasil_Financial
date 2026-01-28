namespace Yggdrasil.Quotation.Domain.Entities;

public class PaymentTerm : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty; // "3 Pagos", "6 Pagos", "12 Pagos"
    public string Code { get; set; } = string.Empty; // "TERM_3", "TERM_6", "TERM_12"
    public int NumberOfPayments { get; set; } // 3, 6, 12, 24 (número de pagos)
    public bool IsActive { get; set; }


}
