namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands.TogglePaymentTermStatus;

public record TogglePaymentTermStatusCommand : ICommand<Result<bool>>
{
    public int PaymentTermId { get; set; }
}
