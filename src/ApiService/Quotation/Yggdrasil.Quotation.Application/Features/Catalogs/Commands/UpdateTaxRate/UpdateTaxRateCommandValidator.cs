using Yggdrasil.Quotation.Application.Repositories;

namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands.UpdateTaxRate;

public class UpdateTaxRateCommandValidator : AbstractValidator<UpdateTaxRateCommand>
{
    private readonly ITaxRateRepository _taxRateRepository;

    public UpdateTaxRateCommandValidator(ITaxRateRepository taxRateRepository)
    {
        _taxRateRepository = taxRateRepository;

        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("El ID debe ser mayor a 0");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres")
            .Matches(@"^[a-zA-Z0-9\s\-]+$").WithMessage("El nombre solo puede contener letras, números, espacios y guiones");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El código es requerido")
            .MaximumLength(20).WithMessage("El código no puede exceder 20 caracteres")
            .Matches(@"^[A-Z0-9]+$").WithMessage("El código solo puede contener letras mayúsculas y números")
            .MustAsync(BeUniqueCodeForOtherId).WithMessage("El código ya existe para otra tasa impositiva");

        RuleFor(x => x.Percentage)
            .GreaterThan(0).WithMessage("El porcentaje debe ser mayor a 0")
            .LessThanOrEqualTo(100).WithMessage("El porcentaje no puede ser mayor a 100")
            .PrecisionScale(2, 5, true).WithMessage("El porcentaje debe tener máximo 2 decimales y 5 dígitos en total");

        RuleFor(x => x.EffectiveDate)
            .NotEmpty().WithMessage("La fecha de efecto es requerida");

        RuleFor(x => x.ExpirationDate)
            .GreaterThan(x => x.EffectiveDate).When(x => x.ExpirationDate.HasValue)
            .WithMessage("La fecha de expiración debe ser posterior a la fecha de efecto");

        RuleFor(x => x)
            .MustAsync(HaveValidDateRange).WithMessage("Existe un solapamiento de fechas con otra tasa impositiva activa")
            .Must(HaveValidBusinessRules).WithMessage("No se puede desactivar una tasa con fechas futuras activas")
            .MustAsync(TaxRateExists).WithMessage("La tasa impositiva no existe");
    }

    private async Task<bool> BeUniqueCodeForOtherId(UpdateTaxRateCommand command, string code, CancellationToken cancellationToken)
    {
        return !await _taxRateRepository.CodeExistsForOtherIdAsync(command.Id, code, cancellationToken);
    }

    private async Task<bool> HaveValidDateRange(UpdateTaxRateCommand command, CancellationToken cancellationToken)
    {
        var overlappingTaxes = await _taxRateRepository.GetOverlappingTaxRatesAsync(
            command.EffectiveDate,
            command.ExpirationDate,
            command.Id, // Excluir la tasa actual
            cancellationToken);

        return !overlappingTaxes.Any();
    }

    private bool HaveValidBusinessRules(UpdateTaxRateCommand command)
    {
        // Validación de reglas de negocio
        // 1. No se puede desactivar una tasa si tiene fechas futuras
        if (!command.IsActive)
        {
            var today = DateTime.Today;
            var hasFutureDates = command.EffectiveDate > today ||
                                (command.ExpirationDate.HasValue && command.ExpirationDate.Value > today);

            if (hasFutureDates)
            {
                return false;
            }
        }

        // 2. Si la tasa está activa, la fecha de efecto no puede ser en el pasado (a menos que sea una corrección)
        if (command.IsActive && command.EffectiveDate < DateTime.Today)
        {
            // Podríamos querer permitir esto para correcciones históricas
            // Depende de los requisitos de negocio
        }

        return true;
    }

    private async Task<bool> TaxRateExists(UpdateTaxRateCommand command, CancellationToken cancellationToken)
    {
        return await _taxRateRepository.ExistsAsync(command.Id, cancellationToken);
    }
}
