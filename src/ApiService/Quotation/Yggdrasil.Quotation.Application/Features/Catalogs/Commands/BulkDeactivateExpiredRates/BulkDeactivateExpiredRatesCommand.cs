namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands.BulkDeactivateExpiredRates;

public record BulkDeactivateExpiredRatesCommand : ICommand<Result<int>>;
