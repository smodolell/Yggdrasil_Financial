using Yggdrasil.Quotation.Application.Repositories;

namespace Yggdrasil.Quotation.Application.Features.Catalogs.Commands;

internal class TogglePaymentTermStatusCommandHandler(
    IUnitOfWork unitOfWork,
    IPaymentTermRepository paymentTermRepository
) : ICommandHandler<TogglePaymentTermStatusCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPaymentTermRepository _paymentTermRepository = paymentTermRepository;

    

    public async Task<Result<bool>> HandleAsync(TogglePaymentTermStatusCommand command, CancellationToken cancellationToken = default)
    {
        
        var paymentTerm = await _paymentTermRepository.GetByIdAsync(command.PaymentTermId, cancellationToken);
        if (paymentTerm == null)
        {
            return Result.NotFound($"No se encontró el término de pago con ID {command.PaymentTermId}.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            paymentTerm.IsActive = !paymentTerm.IsActive;
            var newStatus = paymentTerm.IsActive;

            await _paymentTermRepository.UpdateAsync(paymentTerm);
            await _unitOfWork.CommitAsync(cancellationToken);

            var statusText = newStatus ? "activado" : "desactivado";
            return Result.Success(newStatus, $"Término de pago {statusText} correctamente.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result<bool>.Error($"Error al actualizar el término de pago: {ex.Message}");
        }
    }
}