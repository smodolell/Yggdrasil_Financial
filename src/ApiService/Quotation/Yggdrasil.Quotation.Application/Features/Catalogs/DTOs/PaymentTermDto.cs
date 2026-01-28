namespace Yggdrasil.Quotation.Application.Features.Catalogs.DTOs;
public class PaymentTermDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty; 
    public string Code { get; set; } = string.Empty; 
    public int NumberOfPayments { get; set; } 
    public bool IsActive { get; set; }
}
