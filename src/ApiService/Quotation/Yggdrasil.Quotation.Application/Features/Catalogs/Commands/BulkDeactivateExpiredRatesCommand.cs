namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands;

public record BulkDeactivateExpiredRatesCommand : ICommand<Result<int>>;
