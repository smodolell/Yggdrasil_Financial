namespace Yggdrasil.Contracts.IntegrationEvents.Catalog;

public record ProductCreatedEvent : IntegrationEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}