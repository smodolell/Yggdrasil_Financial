namespace Yggdrasil.Quotation.Application.Specifications;

public class InterestRateSpec : Specification<InterestRate>
{
    public InterestRateSpec(
        string? searchText,
        bool? isActive,
        DateTime? effectiveDateFrom,
        DateTime? effectiveDateTo,
        DateTime? expirationDateFrom,
        DateTime? expirationDateTo,
        decimal? annualPercentageFrom,
        decimal? annualPercentageTo)
    {
        // Búsqueda por texto
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            searchText = searchText.ToLower();
            Query.Where(ir => ir.RateName.ToLower().Contains(searchText));
        }

        // Filtro por estado activo/inactivo
        if (isActive.HasValue)
        {
            Query.Where(ir => ir.IsActive == isActive.Value);
        }

        // Filtro por fecha efectiva desde
        if (effectiveDateFrom.HasValue)
        {
            Query.Where(ir => ir.EffectiveDate >= effectiveDateFrom.Value);
        }

        // Filtro por fecha efectiva hasta
        if (effectiveDateTo.HasValue)
        {
            // Incluir todo el día
            var endOfDay = effectiveDateTo.Value.AddDays(1).AddSeconds(-1);
            Query.Where(ir => ir.EffectiveDate <= endOfDay);
        }

        // Filtro por fecha de expiración desde
        if (expirationDateFrom.HasValue)
        {
            Query.Where(ir =>
                ir.ExpirationDate.HasValue &&
                ir.ExpirationDate >= expirationDateFrom.Value);
        }

        // Filtro por fecha de expiración hasta
        if (expirationDateTo.HasValue)
        {
            var endOfDay = expirationDateTo.Value.AddDays(1).AddSeconds(-1);
            Query.Where(ir =>
                ir.ExpirationDate.HasValue &&
                ir.ExpirationDate <= endOfDay);
        }

        // Filtro por porcentaje anual mínimo
        if (annualPercentageFrom.HasValue)
        {
            Query.Where(ir => ir.AnnualPercentage >= annualPercentageFrom.Value);
        }

        // Filtro por porcentaje anual máximo
        if (annualPercentageTo.HasValue)
        {
            Query.Where(ir => ir.AnnualPercentage <= annualPercentageTo.Value);
        }

        // Filtro para tasas vigentes (opcional - comúnmente usado)
        // Query.Where(ir => ir.IsActive && 
        //     ir.EffectiveDate <= DateTime.Now && 
        //     (!ir.ExpirationDate.HasValue || ir.ExpirationDate >= DateTime.Now));

        // Incluir relaciones si es necesario
        Query.Include(ir => ir.PlanPaymentTerms);

        // Orden por defecto: por fecha efectiva descendente
        Query.OrderByDescending(ir => ir.EffectiveDate)
             .ThenByDescending(ir => ir.ExpirationDate);
    }

    // Constructor adicional para obtener tasas activas vigentes
    public InterestRateSpec(bool activeRatesOnly = true)
    {
        if (activeRatesOnly)
        {
            Query.Where(ir => ir.IsActive);
        }
    }

    // Constructor para buscar por nombre exacto
    public InterestRateSpec(string rateName)
    {
        Query.Where(ir => ir.RateName == rateName);
    }

    // Constructor para tasas vigentes en una fecha específica
    public InterestRateSpec(DateTime specificDate)
    {
        Query.Where(ir =>
            ir.IsActive &&
            ir.EffectiveDate <= specificDate &&
            (!ir.ExpirationDate.HasValue || ir.ExpirationDate >= specificDate));
    }

    // Constructor para tasas por rango de porcentaje
    public InterestRateSpec(decimal minPercentage, decimal maxPercentage)
    {
        Query.Where(ir =>
            ir.AnnualPercentage >= minPercentage &&
            ir.AnnualPercentage <= maxPercentage);
    }
}