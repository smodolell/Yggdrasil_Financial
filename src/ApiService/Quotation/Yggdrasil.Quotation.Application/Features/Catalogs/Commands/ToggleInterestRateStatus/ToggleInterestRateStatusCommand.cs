namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands.ToggleInterestRateStatus;

public record ToggleInterestRateStatusCommand(int Id) : ICommand<Result<bool>>;
