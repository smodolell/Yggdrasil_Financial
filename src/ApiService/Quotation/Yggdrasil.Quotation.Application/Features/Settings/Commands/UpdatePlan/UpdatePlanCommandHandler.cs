using Yggdrasil.Quotation.Application.Features.Settings.DTOs;
using Yggdrasil.Quotation.Application.Repositories;
using Yggdrasil.Quotation.Application.Specifications;

namespace Yggdrasil.Quotation.Application.Features.Settings.Commands.UpdatePlan;

internal class UpdatePlanCommandHandler(
    IUnitOfWork unitOfWork,
    IPlanRepository planRepository,
    IPaymentTermRepository paymentTermRepository,
    IInterestRateRepository interestRateRepository,
    IValidator<PlanEditDto> validator
) : ICommandHandler<UpdatePlanCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPlanRepository _planRepository = planRepository;
    private readonly IPaymentTermRepository _paymentTermRepository = paymentTermRepository;
    private readonly IInterestRateRepository _interestRateRepository = interestRateRepository;
    private readonly IValidator<PlanEditDto> _validator = validator;

    public async Task<Result> HandleAsync(UpdatePlanCommand command, CancellationToken cancellationToken = default)
    {
        var model = command.Model;

        // Validar el modelo
        var validationResult = await _validator.ValidateAsync(model, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        // Buscar el plan existente
        var existingPlan = await _planRepository.GetByIdAsync(command.PlanId, cancellationToken);
        if (existingPlan == null)
        {
            return Result.NotFound($"No se encontró el plan con ID {command.PlanId}");
        }

        // Validar que el plan no esté siendo usado en cotizaciones activas
        var isPlanInUse = await IsPlanUsedInActiveQuotationsAsync(command.PlanId, cancellationToken);
        if (isPlanInUse)
        {
            return Result.Invalid(new ValidationError("No se puede modificar un plan que está siendo usado en cotizaciones activas."));
        }

        // Validar que el nuevo nombre no exista (excluyendo el plan actual)
        var nameExists = await _planRepository
            .AnyAsync(p => p.Id != command.PlanId &&
                          p.Name == model.Name &&
                          p.StartDate <= model.EndDate &&
                          p.EndDate >= model.StartDate,
                     cancellationToken);

        if (nameExists)
        {
            return Result.Invalid(new ValidationError($"Ya existe otro plan con el nombre '{model.Name}' en el rango de fechas especificado."));
        }

        // Validar que los PaymentTerms e InterestRates existen
        var paymentTermIds = model.PaymentTerms
            .Select(pt => pt.PaymentTermId)
            .Distinct()
            .ToList();
        var interestRateIds = model.PaymentTerms
            .Select(pt => pt.InterestRateId)
            .Distinct()
            .ToList();

        var existingPaymentTerms = await _paymentTermRepository
            .CountAsync(new PaymentTermByIdsSpec(paymentTermIds), cancellationToken);

        var existingInterestRates = await _interestRateRepository
            .CountAsync(new InterestRatesByIdsSpec(interestRateIds), cancellationToken);

        if (existingPaymentTerms != paymentTermIds.Count)
        {
            return Result.Invalid(new ValidationError("Uno o más términos de pago no existen en el sistema."));
        }

        if (existingInterestRates != interestRateIds.Count)
        {
            return Result.Invalid(new ValidationError("Uno o más tasas de interés no existen en el sistema."));
        }

        // Validar que no haya órdenes duplicados
        var duplicateOrders = model.PaymentTerms
            .GroupBy(pt => pt.Order)
            .Any(g => g.Count() > 1);

        if (duplicateOrders)
        {
            return Result.Invalid(new ValidationError("No puede haber órdenes duplicados en los términos de pago."));
        }

        // Validar que no haya PaymentTermId duplicados
        var duplicatePaymentTerms = model.PaymentTerms
            .GroupBy(pt => pt.PaymentTermId)
            .Any(g => g.Count() > 1);

        if (duplicatePaymentTerms)
        {
            return Result.Invalid(new ValidationError("No puede haber términos de pago duplicados en el mismo plan."));
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // Actualizar el plan
            existingPlan.Name = model.Name;
            existingPlan.MinAmount = model.MinAmount;
            existingPlan.MaxAmount = model.MaxAmount;
            existingPlan.MinAge = model.MinAge;
            existingPlan.MaxAge = model.MaxAge;
            existingPlan.StartDate = model.StartDate;
            existingPlan.EndDate = model.EndDate;

            await _planRepository.UpdateAsync(existingPlan);

            // Eliminar términos de pago existentes
            await _planRepository.RemoveAllPlanPaymentTermsAsync(command.PlanId, cancellationToken);

            // Crear nuevos términos de pago
            var planPaymentTerms = model.PaymentTerms.Select(dto => new PlanPaymentTerm
            {
                Id = 0,
                PlanId = command.PlanId,
                PaymentTermId = dto.PaymentTermId,
                InterestRateId = dto.InterestRateId,
                Order = dto.Order
            }).ToList();

            foreach (var planPaymentTerm in planPaymentTerms)
            {
                await _planRepository.AddPlanPaymentTermAsync(planPaymentTerm, cancellationToken);
            }

            await _unitOfWork.CommitAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result.Error($"Error al actualizar el plan: {ex.Message}");
        }
    }

    private async Task<bool> IsPlanUsedInActiveQuotationsAsync(int planId, CancellationToken cancellationToken)
    {
        // Implementar según tu lógica de negocio
        // Ejemplo: verificar si hay cotizaciones activas usando este plan
        // return await _quotationRepository.AnyAsync(q => q.PlanId == planId && q.IsActive, cancellationToken);

        // Por ahora retornamos false - ajustar según tus necesidades
        return false;
    }

    private string GetCurrentUserId()
    {
        // Implementar según tu sistema de autenticación
        // Ejemplo: return _currentUserService.GetUserId();
        return "System"; // Temporal
    }
}