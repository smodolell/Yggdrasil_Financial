namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands;

public record TogglePaymentTermStatusCommand : ICommand<Result<bool>>
{
    public int PaymentTermId { get; set; }
}
