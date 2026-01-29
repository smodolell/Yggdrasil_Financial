using Yggdrasil.Quotation.Application.Repositories;

namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands.CreateInterestRate;

internal class CreateInterestRateCommandHandler(
    IUnitOfWork unitOfWork,
    IInterestRateRepository interestRateRepository,
    IValidator<CreateInterestRateCommand> validator
) : ICommandHandler<CreateInterestRateCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IInterestRateRepository _interestRateRepository = interestRateRepository;
    private readonly IValidator<CreateInterestRateCommand> _validator = validator;

    public async Task<Result<int>> HandleAsync(CreateInterestRateCommand command, CancellationToken cancellationToken = default)
    {
        // 1. Validación con FluentValidation
        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        // 2. Validación de negocio: Verificar solapamiento de fechas
        var overlappingRate = await _interestRateRepository
            .AnyAsync(rate =>
                rate.RateName == command.RateName &&
                rate.IsActive &&
                (rate.ExpirationDate == null || rate.ExpirationDate >= command.EffectiveDate) &&
                rate.EffectiveDate <= (command.ExpirationDate ?? DateTime.MaxValue),
                cancellationToken);

        if (overlappingRate)
        {
            return Result.Error(
                $"Ya existe una tasa activa '{command.RateName}' en el rango de fechas especificado.");
        }

        // 3. Validación: No permitir múltiples tasas activas sin expiración
        if (command.IsActive && !command.ExpirationDate.HasValue)
        {
            var existingOpenRate = await _interestRateRepository
                .AnyAsync(rate =>
                    rate.RateName == command.RateName &&
                    rate.IsActive &&
                    rate.ExpirationDate == null,
                    cancellationToken);

            if (existingOpenRate)
            {
                return Result.Error($"Ya existe una tasa '{command.RateName}' activa sin fecha de expiración.");
            }
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {   
            // 4. Crear la entidad
            var interestRate = new InterestRate
            {
                RateName = command.RateName,
                AnnualPercentage = command.AnnualPercentage,
                EffectiveDate = command.EffectiveDate,
                ExpirationDate = command.ExpirationDate,
                IsActive = command.IsActive
            };

            // 5. Guardar
            await _interestRateRepository.AddAsync(interestRate, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // 6. Retornar resultado
            return Result.Success(
                interestRate.Id,
                $"Tasa de interés '{command.RateName}' creada exitosamente.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);

            // Log el error aquí (usando ILogger si lo inyectas)
            return Result.CriticalError(
                $"Error al crear la tasa de interés: {ex.Message}. " +
                "Por favor, contacte al administrador del sistema.");
        }
    }

}