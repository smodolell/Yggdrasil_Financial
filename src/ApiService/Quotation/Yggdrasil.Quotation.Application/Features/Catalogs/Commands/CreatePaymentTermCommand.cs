namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands;

public record CreatePaymentTermCommand : ICommand<Result<int>>
{
    public int NumberOfPayments { get; set; }
    public bool IsActive { get; set; }

}
