using Yggdrasil.Quotation.Application.Common.Interfaces;
using Yggdrasil.Quotation.Application.Features.Catalogs.DTOs;
using Yggdrasil.Quotation.Application.Repositories;
using Yggdrasil.Quotation.Application.Specifications;

namespace Yggdrasil.Quotation.Application.Features.Catalogs.Queries;

using Ardalis.Result;

//public class GetInterestRatesQuery : IQuery<PagedResult<List<InterestRateDto>>>
//{
//    private static readonly HashSet<string> _validSortColumns = new()
//    {
//        nameof(InterestRateDto.RateName),
//        nameof(InterestRateDto.AnnualPercentage),
//    };

//    private int _page = 1;
//    private int _pageSize = 10;
//    private string _sortColumn = nameof(InterestRateDto.RateName);

//    public int Page
//    {
//        get => _page;
//        set => _page = value < 1 ? 1 : value;
//    }

//    public int PageSize
//    {
//        get => _pageSize;
//        set => _pageSize = value switch
//        {
//            < 1 => 10,
//            > 100 => 100,
//            _ => value
//        };
//    }

//    public string SortColumn
//    {
//        get => _sortColumn;
//        set => _sortColumn = _validSortColumns.Contains(value) ? value : nameof(InterestRateDto.RateName);
//    }

//    public bool SortDescending { get; set; }
//    public string? SearchText { get; set; }
//    public DateTime? EffectiveDateFrom { get; set; }
//    public DateTime? EffectiveDateTo { get; set; }
//    public DateTime? ExpirationDateFrom { get; set; }
//    public DateTime? ExpirationDateTo { get; set; }
//    public decimal? AnnualPercentageFrom { get; set; }
//    public decimal? AnnualPercentageTo { get; set; }
//    public bool? IsActive { get; set; }
//}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
/// <summary>
/// Consulta paginada para obtener el listado de tasas de interés con filtros avanzados.
/// </summary>
public class GetInterestRatesQuery : IQuery<PagedResult<List<InterestRateDto>>>
{
    private static readonly HashSet<string> _validSortColumns = new()
    {
        nameof(InterestRateDto.RateName),
        nameof(InterestRateDto.AnnualPercentage),
    };

    private int _page = 1;
    private int _pageSize = 10;
    private string _sortColumn = nameof(InterestRateDto.RateName);

    /// <summary>
    /// Número de página actual.
    /// </summary>
    /// <example>1</example>
    [DefaultValue(1)]
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Cantidad de registros por página (Mín: 1, Máx: 100).
    /// </summary>
    /// <example>10</example>
    [Range(1, 100)]
    [DefaultValue(10)]
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value switch
        {
            < 1 => 10,
            > 100 => 100,
            _ => value
        };
    }

    /// <summary>
    /// Columna por la cual ordenar los resultados. 
    /// Valores permitidos: RateName, AnnualPercentage.
    /// </summary>
    /// <example>RateName</example>
    [DefaultValue("RateName")]
    public string SortColumn
    {
        get => _sortColumn;
        set => _sortColumn = _validSortColumns.Contains(value) ? value : nameof(InterestRateDto.RateName);
    }

    /// <summary>
    /// Indica si el ordenamiento es descendente.
    /// </summary>
    /// <example>false</example>
    public bool SortDescending { get; set; } 

    /// <summary>
    /// Texto para buscar coincidencias en el nombre de la tasa.
    /// </summary>
    /// <example>Tasa Variable</example>
    public string? SearchText { get; set; }

    /// <summary>Rango inicial de fecha de vigencia.</summary>
    public DateTime? EffectiveDateFrom { get; set; }

    /// <summary>Rango final de fecha de vigencia.</summary>
    public DateTime? EffectiveDateTo { get; set; }

    /// <summary>
    /// Límite inferior para filtrar por fecha de expiración.
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? ExpirationDateFrom { get; set; }

    /// <summary>
    /// Límite superior para filtrar por fecha de expiración.
    /// </summary>
    [DataType(DataType.Date)]
    public DateTime? ExpirationDateTo { get; set; }

    /// <summary>Estado de la tasa (Activa/Inactiva).</summary>
    public bool? IsActive { get; set; }

    // Puedes agrupar campos similares con summaries breves
    /// <summary>Porcentaje anual mínimo para filtrar.</summary>
    public decimal? AnnualPercentageFrom { get; set; }

    /// <summary>Porcentaje anual máximo para filtrar.</summary>
    public decimal? AnnualPercentageTo { get; set; }
}

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
internal class GetInterestRatesQueryHandler(
    IInterestRateRepository interestRateRepository,
    IPaginator paginator,
    IDynamicSorter sorter
) : IQueryHandler<GetInterestRatesQuery, PagedResult<List<InterestRateDto>>>
{
    private readonly IInterestRateRepository _interestRateRepository = interestRateRepository;
    private readonly IPaginator _paginator = paginator;
    private readonly IDynamicSorter _sorter = sorter;

    public async Task<PagedResult<List<InterestRateDto>>> HandleAsync(GetInterestRatesQuery message, CancellationToken cancellationToken = default)
    {
       

        var spec = new InterestRateSpec(
            message.SearchText,
            message.IsActive,
            message.EffectiveDateFrom,
            message.EffectiveDateTo,
            message.ExpirationDateFrom, message.ExpirationDateTo,
            message.AnnualPercentageFrom, message.AnnualPercentageTo
        );

        var query = _interestRateRepository.ApplySpecification(spec);

        query = _sorter.ApplySort(query, message.SortColumn, message.SortDescending);

        return await _paginator.PaginateAsync<InterestRate, InterestRateDto>(
            query,
            message.Page,
            message.PageSize,
            cancellationToken
        );
    }
}


