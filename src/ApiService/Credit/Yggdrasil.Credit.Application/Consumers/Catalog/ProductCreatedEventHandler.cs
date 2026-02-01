using Yggdrasil.Contracts.Common.Intefaces;
using Yggdrasil.Contracts.IntegrationEvents.Catalog;
using Yggdrasil.Credit.Application.Features.Products.Commands;

namespace Yggdrasil.Credit.Application.Consumers.Catalog;


public class ProductCreatedEventHandler(
    ICommandMediator commandMediator
) : IEventConsumer<ProductCreatedEvent>
{
    private readonly ICommandMediator _commandMediator = commandMediator;

    public async Task HandleAsync(ProductCreatedEvent @event, CancellationToken cancellationToken = default)
    {

        var command = new CreateProductCommand
        {

            ProductId = @event.Id,
            Name = @event.Name,
        };

        await _commandMediator.SendAsync(command, cancellationToken);

    }
}