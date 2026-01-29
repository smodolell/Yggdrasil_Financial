using Yggdrasil.Quotation.Application.Repositories;

namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands.ToggleInterestRateStatus;

internal class ToggleInterestRateStatusCommandHandler(
    IUnitOfWork unitOfWork,
    IInterestRateRepository interestRateRepository
) : ICommandHandler<ToggleInterestRateStatusCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IInterestRateRepository _interestRateRepository = interestRateRepository;

    public async Task<Result<bool>> HandleAsync(
        ToggleInterestRateStatusCommand command,
        CancellationToken cancellationToken = default)
    {

        // 1. Buscar la tasa
        var existingRate = await _interestRateRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existingRate == null)
        {
            return Result<bool>.Error($"No se encontró la tasa de interés con ID {command.Id}.");
        }

        // 1. Si intenta desactivar, verificar que no esté siendo usada
        if (existingRate.IsActive)
        {
            var isUsedInPlans = await _interestRateRepository
                .IsRateUsedInPlansAsync(command.Id, cancellationToken);

            if (isUsedInPlans)
            {
                return Result<bool>.Error(
                    $"No se puede desactivar la tasa '{existingRate.RateName}' porque está siendo utilizada en planes de pago.");
            }
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // 3. Cambiar estado
            existingRate.IsActive = !existingRate.IsActive;
            var newStatus = existingRate.IsActive;

            await _interestRateRepository.UpdateAsync(existingRate, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // 4. Retornar resultado
            var statusText = newStatus ? "activada" : "desactivada";
            return Result<bool>.Success(
                newStatus,
                $"Tasa de interés '{existingRate.RateName}' {statusText} exitosamente.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);

            return Result<bool>.CriticalError(
                $"Error al cambiar el estado de la tasa: {ex.Message}");
        }
    }
}