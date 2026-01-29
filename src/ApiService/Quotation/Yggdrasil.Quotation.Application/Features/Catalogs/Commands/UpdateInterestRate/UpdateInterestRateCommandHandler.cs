using Yggdrasil.Quotation.Application.Repositories;

namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands.UpdateInterestRate;

internal class UpdateInterestRateCommandHandler(
    IUnitOfWork unitOfWork,
    IInterestRateRepository interestRateRepository,
    IValidator<UpdateInterestRateCommand> validator
) : ICommandHandler<UpdateInterestRateCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IInterestRateRepository _interestRateRepository = interestRateRepository;
    private readonly IValidator<UpdateInterestRateCommand> _validator = validator;

    public async Task<Result> HandleAsync(
        UpdateInterestRateCommand command,
        CancellationToken cancellationToken = default)
    {
        // 1. Validación con FluentValidation
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        // 2. Buscar la tasa existente
        var existingRate = await _interestRateRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existingRate == null)
        {
            return Result.Error($"No se encontró la tasa de interés con ID {command.Id}.");
        }

        // 3. Validación de negocio: Verificar solapamiento de fechas (excluyendo la actual)
        var overlappingRate = await _interestRateRepository
            .AnyAsync(rate =>
                rate.Id != command.Id &&
                rate.RateName == command.RateName &&
                rate.IsActive &&
                (rate.ExpirationDate == null || rate.ExpirationDate >= command.EffectiveDate) &&
                rate.EffectiveDate <= (command.ExpirationDate ?? DateTime.MaxValue),
                cancellationToken);

        if (overlappingRate)
        {
            return Result.Error(
                $"Ya existe otra tasa activa '{command.RateName}' en el rango de fechas especificado.");
        }

        // 4. Validación: No permitir múltiples tasas activas sin expiración
        if (command.IsActive && !command.ExpirationDate.HasValue)
        {
            var existingOpenRate = await _interestRateRepository
                .AnyAsync(rate =>
                    rate.Id != command.Id &&
                    rate.RateName == command.RateName &&
                    rate.IsActive &&
                    rate.ExpirationDate == null,
                    cancellationToken);

            if (existingOpenRate)
            {
                return Result.Error(
                    $"Ya existe otra tasa '{command.RateName}' activa sin fecha de expiración.");
            }
        }

        // 5. Verificar si la tasa está siendo usada en PlanPaymentTerms
        var isUsedInPlans = await _interestRateRepository
            .IsRateUsedInPlansAsync(command.Id, cancellationToken);

        if (isUsedInPlans && !command.IsActive)
        {
            return Result.Error($"No se puede desactivar la tasa '{existingRate.RateName}' porque está siendo utilizada en planes de pago.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // 6. Actualizar la entidad
            existingRate.RateName = command.RateName;
            existingRate.AnnualPercentage = command.AnnualPercentage;
            existingRate.EffectiveDate = command.EffectiveDate;
            existingRate.ExpirationDate = command.ExpirationDate;
            existingRate.IsActive = command.IsActive;

            // 7. Guardar cambios
            await _interestRateRepository.UpdateAsync(existingRate, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // 8. Retornar resultado
            return Result.SuccessWithMessage($"Tasa de interés '{command.RateName}' actualizada exitosamente.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);

            return Result.CriticalError(
                $"Error al actualizar la tasa de interés: {ex.Message}");
        }
    }
}
