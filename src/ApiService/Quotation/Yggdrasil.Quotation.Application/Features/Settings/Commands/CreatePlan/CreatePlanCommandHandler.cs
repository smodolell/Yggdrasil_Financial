using Yggdrasil.Quotation.Application.Features.Settings.DTOs;
using Yggdrasil.Quotation.Application.Repositories;
using Yggdrasil.Quotation.Application.Specifications;

namespace Yggdrasil.Quotation.Application.Features.Settings.Commands.CreatePlan;

internal class CreatePlanCommandHandler(
    IUnitOfWork unitOfWork,
    IPlanRepository planRepository,
    IPaymentTermRepository paymentTermRepository,
    IInterestRateRepository interestRateRepository,
    IValidator<PlanEditDto> validator
) : ICommandHandler<CreatePlanCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPlanRepository _planRepository = planRepository;
    private readonly IPaymentTermRepository _paymentTermRepository = paymentTermRepository;
    private readonly IInterestRateRepository _interestRateRepository = interestRateRepository;
    private readonly IValidator<PlanEditDto> _validator = validator;

    public async Task<Result<int>> HandleAsync(CreatePlanCommand command, CancellationToken cancellationToken = default)
    {
        var model = command.Model;

        // Validación del modelo
        var validationResult = await _validator.ValidateAsync(model, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }

        // Validar que el nombre del plan no exista
        var planExists = await _planRepository
            .AnyAsync(p => p.Name == model.Name &&
                          p.StartDate <= model.EndDate &&
                          p.EndDate >= model.StartDate, cancellationToken);

        if (planExists)
        {
            return Result.Invalid(new ValidationError($"Ya existe un plan con el nombre '{model.Name}' en el rango de fechas especificado."));
        }

        // Validar que los PaymentTerms e InterestRates existen
        var paymentTermIds = model.PaymentTerms.Select(pt => pt.PaymentTermId).Distinct().ToList();
        var interestRateIds = model.PaymentTerms.Select(pt => pt.InterestRateId).Distinct().ToList();

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

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // Crear el plan
            var newPlan = new Plan
            {
                Id = 0,
                Name = model.Name,
                MinAmount = model.MinAmount,
                MaxAmount = model.MaxAmount,
                MinAge = model.MinAge,
                MaxAge = model.MaxAge,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
            };

            await _planRepository.AddAsync(newPlan, cancellationToken);

            // Guardar para obtener el ID generado
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Crear los términos de pago del plan
            var planPaymentTerms = model.PaymentTerms.Select(dto => new PlanPaymentTerm
            {
                Id = 0,
                PlanId = newPlan.Id,
                PaymentTermId = dto.PaymentTermId,
                InterestRateId = dto.InterestRateId,
                Order = dto.Order                
            }).ToList();

            foreach (var planPaymentTerm in planPaymentTerms)
            {
                await _planRepository.AddPlanPaymentTermAsync(planPaymentTerm, cancellationToken);
            }

            await _unitOfWork.CommitAsync(cancellationToken);
            return Result.Created(newPlan.Id);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            // Log del error (deberías tener un servicio de logging)
            return Result.Error($"Error al crear el plan: {ex.Message}");
        }
    }
}