namespace Yggdrasil.Quotation.Application.Features.Settings.DTOs;

public class PlanEditDtoValidator : AbstractValidator<PlanEditDto>
{
    public PlanEditDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del plan es requerido")
            .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.MinAmount)
            .GreaterThanOrEqualTo(0)
                .WithMessage("El monto mínimo debe ser mayor o igual a 0")
            .LessThan(x => x.MaxAmount)
                .WithMessage("El monto mínimo debe ser menor que el monto máximo");

        RuleFor(x => x.MaxAmount)
            .GreaterThan(0)
                .WithMessage("El monto máximo debe ser mayor a 0");

        RuleFor(x => x.MinAge)
            .GreaterThanOrEqualTo(18)
                .WithMessage("La edad mínima debe ser 18 años o más")
            .LessThan(x => x.MaxAge)
                .WithMessage("La edad mínima debe ser menor que la edad máxima");

        RuleFor(x => x.MaxAge)
            .GreaterThan(0)
                .WithMessage("La edad máxima debe ser mayor a 0")
            .LessThanOrEqualTo(120)
                .WithMessage("La edad máxima no puede exceder 120 años");

        RuleFor(x => x.StartDate)
            .NotEmpty()
                .WithMessage("La fecha de inicio es requerida")
            .LessThan(x => x.EndDate)
                .WithMessage("La fecha de inicio debe ser anterior a la fecha de fin");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("La fecha de fin es requerida")
            .GreaterThan(DateTime.Today).WithMessage("La fecha de fin debe ser futura");

        RuleFor(x => x.PaymentTerms)
            .NotEmpty()
                .WithMessage("El plan debe tener al menos un término de pago");

        RuleForEach(x => x.PaymentTerms)
            .ChildRules(paymentTerm =>
            {
                paymentTerm.RuleFor(pt => pt.PaymentTermId)
                    .GreaterThan(0).WithMessage("El ID del término de pago es requerido");

                paymentTerm.RuleFor(pt => pt.InterestRateId)
                    .GreaterThan(0).WithMessage("El ID de la tasa de interés es requerido");

                paymentTerm.RuleFor(pt => pt.Order)
                    .GreaterThan(0).WithMessage("El orden debe ser mayor a 0");
            });
    }
}
