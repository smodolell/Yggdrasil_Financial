namespace Yggdrasil.Quotation.Application.Features.Catalogs.Queries;

public class GetInterestRatesQueryValidator : AbstractValidator<GetInterestRatesQuery>
{
    public GetInterestRatesQueryValidator()
    {
        // Validación de Paginación
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("La página debe ser mayor o igual a 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("El tamaño de página debe estar entre 1 y 100.");

        // Validación de Rangos de Fechas (Consistencia Lógica)
        RuleFor(x => x.EffectiveDateTo)
            .GreaterThanOrEqualTo(x => x.EffectiveDateFrom)
            .When(x => x.EffectiveDateFrom.HasValue && x.EffectiveDateTo.HasValue)
            .WithMessage("La fecha de vigencia final no puede ser menor a la inicial.");

        RuleFor(x => x.ExpirationDateTo)
            .GreaterThanOrEqualTo(x => x.ExpirationDateFrom)
            .When(x => x.ExpirationDateFrom.HasValue && x.ExpirationDateTo.HasValue)
            .WithMessage("La fecha de expiración final no puede ser menor a la inicial.");

        // Validación de Rangos de Porcentaje
        RuleFor(x => x.AnnualPercentageTo)
            .GreaterThanOrEqualTo(x => x.AnnualPercentageFrom)
            .When(x => x.AnnualPercentageFrom.HasValue && x.AnnualPercentageTo.HasValue)
            .WithMessage("El porcentaje máximo no puede ser menor al mínimo.");

        // Validación de Búsqueda (opcional, por rendimiento)
        RuleFor(x => x.SearchText)
            .MaximumLength(100).WithMessage("El texto de búsqueda es demasiado largo.");
    }
}


