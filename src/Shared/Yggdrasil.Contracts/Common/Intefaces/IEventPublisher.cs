namespace Yggdrasil.Contracts.Common.Intefaces;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class;

}



public interface IEventPublisher<TEvent> where TEvent : class
{
    Task PublishAsync(TEvent @event, CancellationToken cancellationToken = default);
}