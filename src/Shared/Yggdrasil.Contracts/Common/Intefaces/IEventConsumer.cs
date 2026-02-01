namespace Yggdrasil.Contracts.Common.Intefaces;

public interface IEventConsumer<TEvent> where TEvent : class
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}

