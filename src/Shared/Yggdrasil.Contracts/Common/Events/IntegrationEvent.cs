namespace Yggdrasil.Contracts.Common.Events;

public abstract record IntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;

    // Metadatos adicionales
    public string Source { get; init; } = string.Empty;   // Microservicio origen
    public string TenantId { get; init; } = string.Empty; // Multitenant
    public string CorrelationId { get; init; } = string.Empty; // Para trazabilidad
}
