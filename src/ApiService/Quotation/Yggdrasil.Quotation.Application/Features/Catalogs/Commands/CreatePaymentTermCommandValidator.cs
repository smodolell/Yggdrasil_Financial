namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands;

public class CreatePaymentTermCommandValidator : AbstractValidator<CreatePaymentTermCommand>
{
    public CreatePaymentTermCommandValidator()
    {
        RuleFor(x => x.NumberOfPayments)
          .GreaterThan(0)
          .WithMessage("El número de pagos debe ser mayor a cero.")
          .LessThanOrEqualTo(60)
          .WithMessage("El número de pagos no puede exceder {ComparisonValue}.");

    }
}
