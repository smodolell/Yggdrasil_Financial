using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Yggdrasil.Catalog.Application.Features.Products.DTOs;


namespace Yggdrasil.Catalog.Application.Features.Products.Queries;

public class GetProductsQuery : IQuery<PagedResult<List<ProductDto>>>
{
    private static readonly HashSet<string> _validSortColumns = new()
    {
        nameof(ProductDto.Name),
    };

    private int _page = 1;
    private int _pageSize = 10;
    private string _sortColumn = nameof(ProductDto.Name);

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
    /// Valores permitidos: Name,
    /// </summary>
    /// <example>Name</example>
    [DefaultValue("Name")]
    public string SortColumn
    {
        get => _sortColumn;
        set => _sortColumn = _validSortColumns.Contains(value) ? value : nameof(Product.Name);
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
}
