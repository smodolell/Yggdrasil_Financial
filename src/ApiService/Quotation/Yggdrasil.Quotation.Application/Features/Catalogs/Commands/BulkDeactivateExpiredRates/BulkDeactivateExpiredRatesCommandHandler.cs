using Yggdrasil.Quotation.Application.Repositories;
using Yggdrasil.Quotation.Application.Specifications;

namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands.BulkDeactivateExpiredRates;

internal class BulkDeactivateExpiredRatesCommandHandler(
    IUnitOfWork unitOfWork,
    IInterestRateRepository interestRateRepository
) : ICommandHandler<BulkDeactivateExpiredRatesCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IInterestRateRepository _interestRateRepository = interestRateRepository;

    public async Task<Result<int>> HandleAsync(
        BulkDeactivateExpiredRatesCommand command,
        CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Buscar tasas expiradas que aún estén activas
            var expiredRates = await _interestRateRepository.GetListAsync(new InterestRateExpired(),cancellationToken);

            foreach (var rate in expiredRates)
            {
                rate.IsActive = false;
                await _interestRateRepository.UpdateAsync(rate, cancellationToken);
            }

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<int>.Success(
                expiredRates.Count,
                $"{expiredRates.Count} tasas expiradas fueron desactivadas.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);

            return Result<int>.CriticalError(
                $"Error al desactivar tasas expiradas: {ex.Message}");
        }
    }
}