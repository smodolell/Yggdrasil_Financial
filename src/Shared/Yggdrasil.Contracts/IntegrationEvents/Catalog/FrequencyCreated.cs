namespace Yggdrasil.Contracts.IntegrationEvents.Catalog;

public record FrequencyCreatedEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;

    public int FrequencyId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int DaysInterval { get; init; }
    public int PeriodsPerYear { get; init; }
    public bool IsActive { get; init; }

    // Metadata
    public string EventType { get; init; } = nameof(FrequencyCreatedEvent);
    public string Source { get; init; } = "Catalog.Service";
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}


public record FrequencyUpdatedEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;

    public int FrequencyId { get; init; }
    public string OldName { get; init; } = string.Empty;
    public string NewName { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public int DaysInterval { get; init; }
    public bool IsActive { get; init; }

    // Campos que cambiaron
    public List<string> ChangedFields { get; init; } = new();
}
