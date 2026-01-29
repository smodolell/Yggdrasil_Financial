using Yggdrasil.Quotation.Application.Repositories;

namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands.UpdateTaxRate;

internal class UpdateTaxRateCommandHandler(
    IUnitOfWork unitOfWork,
    ITaxRateRepository taxRateRepository,
    IValidator<UpdateTaxRateCommand> validator,
    ILogger<UpdateTaxRateCommandHandler> logger
) : ICommandHandler<UpdateTaxRateCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ITaxRateRepository _taxRateRepository = taxRateRepository;
    private readonly IValidator<UpdateTaxRateCommand> _validator = validator;
    private readonly ILogger<UpdateTaxRateCommandHandler> _logger = logger;

    public async Task<Result> HandleAsync(UpdateTaxRateCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar el comando
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }

            // Obtener la entidad existente
            var taxRate = await _taxRateRepository.GetByIdAsync(command.Id, cancellationToken);
            if (taxRate == null)
            {
                return Result.NotFound($"Tasa impositiva con ID {command.Id} no encontrada");
            }

            // Registrar información de auditoría antes de cambios
            var originalCode = taxRate.Code;
            var originalPercentage = taxRate.Percentage;
            var originalIsActive = taxRate.IsActive;

            // Aplicar los cambios
            taxRate.Name = command.Name.Trim();
            taxRate.Code = command.Code.ToUpper();
            taxRate.Percentage = command.Percentage;
            taxRate.EffectiveDate = command.EffectiveDate;
            taxRate.ExpirationDate = command.ExpirationDate;
            taxRate.IsActive = command.IsActive;


            // Validaciones de negocio adicionales
            await ValidateBusinessRulesAsync(taxRate, originalIsActive, cancellationToken);

            // Actualizar en el repositorio
            await _taxRateRepository.UpdateAsync(taxRate, cancellationToken);

            // Guardar cambios
            await _unitOfWork.CommitAsync(cancellationToken);

            // Log de cambios importantes
            LogImportantChanges(taxRate, originalCode, originalPercentage, originalIsActive);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la tasa impositiva con ID {Id}", command.Id);
            return Result.Error($"Error al actualizar la tasa impositiva: {ex.Message}");
        }
    }

    private async Task ValidateBusinessRulesAsync(TaxRate taxRate, bool originalIsActive, CancellationToken cancellationToken)
    {
        // 1. Verificar si hubo cambio en el código y validar unicidad
        var existingByCode = await _taxRateRepository.GetByCodeAsync(taxRate.Code, cancellationToken);
        if (existingByCode != null && existingByCode.Id != taxRate.Id)
        {
            throw new InvalidOperationException($"Ya existe otra tasa impositiva con el código '{taxRate.Code}'");
        }

        // 2. Validar que no haya solapamientos (ya hecho en validador, pero por si acaso)
        var overlappingTaxes = await _taxRateRepository.GetOverlappingTaxRatesAsync(
            taxRate.EffectiveDate,
            taxRate.ExpirationDate,
            taxRate.Id,
            cancellationToken);

        if (overlappingTaxes.Any())
        {
            var overlappingCode = overlappingTaxes.First().Code;
            throw new InvalidOperationException($"La tasa se solapa con la tasa '{overlappingCode}'");
        }

        // 3. Si se está desactivando, verificar que no esté siendo usada en cotizaciones activas
        if (originalIsActive && !taxRate.IsActive)
        {
            // Aquí deberías verificar si la tasa está siendo usada
            // Por ejemplo: await _quotationRepository.IsTaxRateUsedAsync(taxRate.Id, cancellationToken);
            // if (isUsed) throw new InvalidOperationException("No se puede desactivar una tasa en uso");
        }

        // 4. Validar cambios en porcentaje para tasas históricas
        var today = DateTime.Today;
        if (taxRate.EffectiveDate < today && taxRate.Percentage != taxRate.Percentage)
        {
            _logger.LogWarning("Cambio de porcentaje en tasa histórica {Code}", taxRate.Code);
            // Podrías querer notificar a alguien o requerir aprobación
        }

        // 5. Validar que si tiene fecha de expiración, no sea anterior a hoy si está activa
        if (taxRate.IsActive && taxRate.ExpirationDate.HasValue && taxRate.ExpirationDate.Value < DateTime.Today)
        {
            throw new InvalidOperationException("Una tasa activa no puede tener fecha de expiración en el pasado");
        }
    }

    private void LogImportantChanges(TaxRate taxRate, string originalCode, decimal originalPercentage, bool originalIsActive)
    {
        var changes = new List<string>();

        if (originalCode != taxRate.Code)
            changes.Add($"Código: {originalCode} -> {taxRate.Code}");

        if (originalPercentage != taxRate.Percentage)
            changes.Add($"Porcentaje: {originalPercentage}% -> {taxRate.Percentage}%");

        if (originalIsActive != taxRate.IsActive)
            changes.Add($"Estado: {(originalIsActive ? "Activo" : "Inactivo")} -> {(taxRate.IsActive ? "Activo" : "Inactivo")}");

        if (changes.Any())
        {
            _logger.LogInformation("Tasa impositiva {Id} actualizada. Cambios: {Changes}",
                taxRate.Id, string.Join(", ", changes));
        }
    }
}
