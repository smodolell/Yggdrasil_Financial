namespace Yggdrasil.Catalog.Application.Features.Frecuencies.Commands;

public class CreateOrUpdateFrequencyCommand : ICommand<Result<int>>
{
    public int FrequencyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DaysInterval { get; set; } // Ej: 30 para mensual
    public int PeriodsPerYear { get; set; } // Ej: 12 para mensual
}
