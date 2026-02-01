using MassTransit;
using Microsoft.Extensions.Logging;
using Yggdrasil.Contracts.Common.Intefaces;

namespace Yggdrasil.Catalog.Infrastructure.Messaging;


public class MassTransitEventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<MassTransitEventPublisher> _logger;

    public MassTransitEventPublisher(
        IPublishEndpoint publishEndpoint,
        ILogger<MassTransitEventPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        try
        {
            _logger.LogDebug("Publishing event {EventType}", typeof(TEvent).Name);
            await _publishEndpoint.Publish(@event, cancellationToken);
            _logger.LogDebug("Successfully published event {EventType}", typeof(TEvent).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing event {EventType}", typeof(TEvent).Name);
            throw;
        }
    }
}

public class MassTransitEventPublisher<TEvent> : IEventPublisher<TEvent>
    where TEvent : class
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<MassTransitEventPublisher<TEvent>> _logger;

    public MassTransitEventPublisher(
        IPublishEndpoint publishEndpoint,
        ILogger<MassTransitEventPublisher<TEvent>> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task PublishAsync(TEvent @event, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Publishing event {EventType}", typeof(TEvent).Name);
            await _publishEndpoint.Publish(@event, cancellationToken);
            _logger.LogDebug("Successfully published event {EventType}", typeof(TEvent).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing event {EventType}", typeof(TEvent).Name);
            throw;
        }
    }
}