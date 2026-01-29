using Yggdrasil.Quotation.Application.Repositories;

namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands.CreateTaxRate;

internal class CreateTaxRateCommandHandler(
    IUnitOfWork unitOfWork,
    ITaxRateRepository taxRateRepository,
    IValidator<CreateTaxRateCommand> validator,
    ILogger<CreateTaxRateCommandHandler> logger
) : ICommandHandler<CreateTaxRateCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ITaxRateRepository _taxRateRepository = taxRateRepository;
    private readonly IValidator<CreateTaxRateCommand> _validator = validator;
    private readonly ILogger<CreateTaxRateCommandHandler> _logger = logger;

    public async Task<Result<int>> HandleAsync(CreateTaxRateCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Invalid(validationResult.AsErrors());
            }
            var taxRate = new TaxRate
            {
                Name = command.Name.Trim(),
                Code = command.Code.ToUpper(),
                Percentage = command.Percentage,
                EffectiveDate = command.EffectiveDate,
                ExpirationDate = command.ExpirationDate,
                IsActive = command.IsActive,
            };
            await ValidateBusinessRulesAsync(taxRate, cancellationToken);

            await _taxRateRepository.AddAsync(taxRate, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);
            return Result.Created(taxRate.Id); 
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al crear la tasa impositiva: {ex.Message}");
            return Result.Error($"Error al crear la tasa impositiva: {ex.Message}");
        }
    }
    private async Task ValidateBusinessRulesAsync(TaxRate taxRate, CancellationToken cancellationToken)
    {
        // Verificar que no haya tasas activas con el mismo código
        var existingTaxRate = await _taxRateRepository.GetByCodeAsync(taxRate.Code, cancellationToken);
        if (existingTaxRate != null && existingTaxRate.IsActive)
        {
            throw new InvalidOperationException($"Ya existe una tasa impositiva activa con el código '{taxRate.Code}'");
        }

        // Verificar que no haya tasas con el mismo nombre en el mismo período
        var taxesWithSameName = await _taxRateRepository.GetByNameAsync(taxRate.Name, cancellationToken);
        var overlappingTaxes = taxesWithSameName.Where(t =>
            t.IsActive &&
            t.EffectiveDate <= (taxRate.ExpirationDate ?? DateTime.MaxValue) &&
            (t.ExpirationDate ?? DateTime.MaxValue) >= taxRate.EffectiveDate
        );

        if (overlappingTaxes.Any())
        {
            throw new InvalidOperationException($"Ya existe una tasa impositiva con el nombre '{taxRate.Name}' en el período especificado");
        }

        // Validar que el porcentaje sea razonable para el tipo de impuesto
        if (taxRate.Name.Contains("IVA") && (taxRate.Percentage < 5 || taxRate.Percentage > 27))
        {
            throw new InvalidOperationException($"El porcentaje para IVA debe estar entre 5% y 27%");
        }

        if (taxRate.Name.Contains("IGV") && (taxRate.Percentage < 16 || taxRate.Percentage > 20))
        {
            throw new InvalidOperationException($"El porcentaje para IGV debe estar entre 16% y 20%");
        }
    }
}
