namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands.CreateInterestRate;

public class CreateInterestRateCommandValidator :AbstractValidator<CreateInterestRateCommand>
{
    public CreateInterestRateCommandValidator()
    {
        RuleFor(x => x.RateName)
            .NotEmpty()
            .WithMessage("El nombre de la tasa es requerido.")
            .MaximumLength(100)
            .WithMessage("El nombre no puede exceder 100 caracteres.");

        RuleFor(x => x.AnnualPercentage)
            .GreaterThan(0)
            .WithMessage("El porcentaje anual debe ser mayor a 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("El porcentaje anual no puede exceder 100%.");

        RuleFor(x => x.EffectiveDate)
            .NotEmpty()
            .WithMessage("La fecha efectiva es requerida.")
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("La fecha efectiva no puede ser en el pasado.");

        RuleFor(x => x)
            .Must(x => x.ExpirationDate == null || x.ExpirationDate > x.EffectiveDate)
            .WithMessage("La fecha de expiración debe ser posterior a la fecha efectiva.");

        RuleFor(x => x.ExpirationDate)
            .GreaterThanOrEqualTo(x => x.EffectiveDate.AddDays(1))
            .When(x => x.ExpirationDate.HasValue)
            .WithMessage("La fecha de expiración debe ser al menos un día después de la fecha efectiva.");
    }
}
