using Yggdrasil.Quotation.Application.Repositories;

namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands.CreateTaxRate;

public class CreateTaxRateCommandValidator : AbstractValidator<CreateTaxRateCommand>
{
    private readonly ITaxRateRepository _taxRateRepository;

    public CreateTaxRateCommandValidator(ITaxRateRepository taxRateRepository)
    {
        _taxRateRepository = taxRateRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres")
            .Matches(@"^[a-zA-Z0-9\s\-]+$").WithMessage("El nombre solo puede contener letras, números, espacios y guiones");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El código es requerido")
            .MaximumLength(20).WithMessage("El código no puede exceder 20 caracteres")
            .Matches(@"^[A-Z0-9]+$").WithMessage("El código solo puede contener letras mayúsculas y números")
            .MustAsync(BeUniqueCode).WithMessage("El código ya existe");

        RuleFor(x => x.Percentage)
            .GreaterThan(0)
                .WithMessage("El porcentaje debe ser mayor a 0")
            .LessThanOrEqualTo(100)
                .WithMessage("El porcentaje no puede ser mayor a 100")
            .PrecisionScale(2, 5,true)
                .WithMessage("El porcentaje debe tener máximo 2 decimales y 5 dígitos en total");

        RuleFor(x => x.EffectiveDate)
            .NotEmpty().WithMessage("La fecha de efecto es requerida")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("La fecha de efecto no puede ser en el pasado");

        RuleFor(x => x.ExpirationDate)
            .GreaterThan(x => x.EffectiveDate).When(x => x.ExpirationDate.HasValue)
            .WithMessage("La fecha de expiración debe ser posterior a la fecha de efecto");

        RuleFor(x => x)
            .Must(HaveValidDateRange).WithMessage("Existe un solapamiento de fechas con otra tasa impositiva activa")
            .Must(HaveValidActiveStatus).WithMessage("Una tasa impositiva no puede estar inactiva al crearse");
    }

    private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
    {
        return !await _taxRateRepository.CodeExistsAsync(code, cancellationToken);
    }

    private bool HaveValidDateRange(CreateTaxRateCommand command)
    {
        // Verificar que no existan tasas activas en el mismo rango de fechas
        var overlappingTaxes = _taxRateRepository.GetOverlappingTaxRatesAsync(
            command.EffectiveDate,
            command.ExpirationDate,
            cancellationToken: default).Result;

        return !overlappingTaxes.Any();
    }

    private bool HaveValidActiveStatus(CreateTaxRateCommand command)
    {
        // Una nueva tasa impositiva debe crearse como activa
        return command.IsActive;
    }
}
