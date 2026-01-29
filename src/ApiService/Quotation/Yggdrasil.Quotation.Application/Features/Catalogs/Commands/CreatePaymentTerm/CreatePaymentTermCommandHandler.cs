using Yggdrasil.Quotation.Application.Repositories;

namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands.CreatePaymentTerm;

internal class CreatePaymentTermCommandHandler(
    IUnitOfWork unitOfWork,
    IPaymentTermRepository paymentTermRepository,
    IValidator<CreatePaymentTermCommand> validator
) : ICommandHandler<CreatePaymentTermCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPaymentTermRepository _paymentTermRepository = paymentTermRepository;
    private readonly IValidator<CreatePaymentTermCommand> _validator = validator;

    public async Task<Result<int>> HandleAsync(CreatePaymentTermCommand command, CancellationToken cancellationToken = default)
    {

        var validationResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        var paymentTermExists = await _paymentTermRepository
            .AnyAsync(pt => pt.NumberOfPayments == command.NumberOfPayments, cancellationToken);

        if (paymentTermExists)
        {
            return Result.Invalid(new ValidationError($"Ya existe un término de pago con {command.NumberOfPayments} pagos."));
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        var newPaymentTerm = new PaymentTerm
        {
            Id = 0,
            Code = GeneratePaymentTermCode(command.NumberOfPayments),
            Name = GeneratePaymentTermName(command.NumberOfPayments),
            NumberOfPayments = command.NumberOfPayments,
            IsActive = command.IsActive
        };
        await _paymentTermRepository.AddAsync(newPaymentTerm);

        await _unitOfWork.CommitAsync(cancellationToken);
        return Result.Created(newPaymentTerm.Id);
    }

    private string GeneratePaymentTermName(int numberOfPayments)
    {
        return numberOfPayments switch
        {
            1 => "Un pago",
            _ => $"{numberOfPayments} pagos"
        };
    }
    private string GeneratePaymentTermCode(int numberOfPayments)
    {
        return $"TERM_{numberOfPayments}";
    }
}
