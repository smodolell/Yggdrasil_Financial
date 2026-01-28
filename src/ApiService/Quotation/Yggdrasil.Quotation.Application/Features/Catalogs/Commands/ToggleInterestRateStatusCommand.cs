namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands;

public record ToggleInterestRateStatusCommand(int Id) : ICommand<Result<bool>>;
