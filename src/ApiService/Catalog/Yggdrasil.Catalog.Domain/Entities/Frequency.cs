namespace Yggdrasil.Catalog.Domain.Entities;
public class Frequency : BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DaysInterval { get; set; } // Ej: 30 para mensual
    public int PeriodsPerYear { get; set; } // Ej: 12 para mensual

    public bool IsActive { get; set; }
}

