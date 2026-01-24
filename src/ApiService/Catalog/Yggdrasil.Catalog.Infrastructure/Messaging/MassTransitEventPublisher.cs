using MassTransit;
using Microsoft.Extensions.Logging;
using Yggdrasil.Catalog.Application.Interfaces;

namespace Yggdrasil.Catalog.Infrastructure.Messaging;


public class MassTransitEventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly ILogger<MassTransitEventPublisher> _logger;

    public MassTransitEventPublisher(
        IPublishEndpoint publishEndpoint,
        ISendEndpointProvider sendEndpointProvider,
        ILogger<MassTransitEventPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _sendEndpointProvider = sendEndpointProvider;
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        try
        {
            // Usa MassTransit internamente
            await _publishEndpoint.Publish(@event, cancellationToken);

            _logger.LogDebug(
                "Event published via MassTransit: {EventType}",
                typeof(TEvent).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing event: {EventType}", typeof(TEvent).Name);
            throw;
        }
    }

    public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class
    {
        try
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(
                new Uri($"queue:{typeof(TCommand).Name}"));

            await endpoint.Send(command, cancellationToken);

            _logger.LogDebug(
                "Command sent via MassTransit: {CommandType}",
                typeof(TCommand).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending command: {CommandType}", typeof(TCommand).Name);
            throw;
        }
    }
}