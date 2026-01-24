namespace Yggdrasil.Catalog.Application.Features.Frecuencies.DTOs;

public class FrequencyDto
{
    public int FrequencyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DaysInterval { get; set; } 
    public int PeriodsPerYear { get; set; }
}