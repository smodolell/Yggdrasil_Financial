using MassTransit;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Yggdrasil.Contracts.Common.Intefaces;

namespace Yggdrasil.Catalog.Infrastructure.Messaging;

public class MassTransitEventConsumer<TEvent> : IConsumer<TEvent>
    where TEvent : class
{
    private readonly IEventConsumer<TEvent> _innerConsumer;
    private readonly ILogger<MassTransitEventConsumer<TEvent>> _logger;
    private readonly ActivitySource _activitySource;

    public MassTransitEventConsumer(
        IEventConsumer<TEvent> innerConsumer,
        ILogger<MassTransitEventConsumer<TEvent>> logger)
    {
        _innerConsumer = innerConsumer ?? throw new ArgumentNullException(nameof(innerConsumer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _activitySource = new ActivitySource($"EventConsumer.{typeof(TEvent).Name}");
    }

    public async Task Consume(ConsumeContext<TEvent> context)
    {
        using var activity = _activitySource.StartActivity($"Consume_{typeof(TEvent).Name}");
        activity?.SetTag("message_id", context.MessageId);
        activity?.SetTag("event_type", typeof(TEvent).FullName);

        var eventName = typeof(TEvent).Name;
        var messageId = context.MessageId?.ToString() ?? "unknown";

        _logger.LogDebug(
            "Processing event {EventName} with ID {MessageId}",
            eventName, messageId);

        try
        {
            var stopwatch = Stopwatch.StartNew();

            await _innerConsumer.HandleAsync(context.Message, context.CancellationToken)
                .ConfigureAwait(false);

            stopwatch.Stop();

            _logger.LogInformation(
                "Successfully processed event {EventName} with ID {MessageId} in {ElapsedMilliseconds}ms",
                eventName, messageId, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex,
                "Error processing event {EventName} with ID {MessageId}",
                eventName, messageId);

            // Re-lanzar la excepción para que MassTransit maneje el reintento
            throw new ConsumerException($"Error consuming event {eventName}", ex);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
                "Event processing cancelled for {EventName} with ID {MessageId}",
                eventName, messageId);
            throw;
        }
    }
}
