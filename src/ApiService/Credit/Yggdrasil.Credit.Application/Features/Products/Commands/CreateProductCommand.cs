using Yggdrasil.Contracts.Common.Intefaces;
using Yggdrasil.Contracts.IntegrationEvents.Catalog;
using Yggdrasil.Credit.Application.Interfaces;

namespace Yggdrasil.Credit.Application.Features.Products.Commands;

public class CreateProductCommand : ICommand<Result<Guid>>
{
    public Guid ProductId { get; set; }

    public string Name { get; set; } = string.Empty;

}

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {

    }
}



public class CreateProductCommandHandler(
    IUnitOfWork unitOfWork,
    IProductRepository productRepository,
    IEventPublisher eventPublisher,
    IValidator<CreateProductCommand> validator
) : ICommandHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IEventPublisher _eventPublisher = eventPublisher;
    private readonly IValidator<CreateProductCommand> _validator = validator;

    public async Task<Result<Guid>> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken = default)
    {

        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        await _unitOfWork.BeginTransactionAsync();

        var product = Product.Create(command.ProductId, command.Name);

        await _productRepository.AddAsync(product);

        await _unitOfWork.CommitAsync();

        await _eventPublisher.PublishAsync(new ProductCreatedEvent
        {
            Id = product.Id,
            Name = product.Name,
        });

        return Result.Success();
    }
}