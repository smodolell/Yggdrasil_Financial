using Yggdrasil.Quotation.Application.Features.Catalogs.DTOs;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Yggdrasil.Quotation.Application.Features.Catalogs.Queries;

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


