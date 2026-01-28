namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands;

public class UpdateInterestRateCommandValidator : AbstractValidator<UpdateInterestRateCommand>
{
    public UpdateInterestRateCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("El ID de la tasa es inválido.");

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
            .WithMessage("La fecha efectiva es requerida.");

        RuleFor(x => x)
            .Must(x => x.ExpirationDate == null || x.ExpirationDate > x.EffectiveDate)
            .WithMessage("La fecha de expiración debe ser posterior a la fecha efectiva.");
    }
}
