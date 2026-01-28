using Yggdrasil.Quotation.Application.Features.Settings.DTOs;

namespace Yggdrasil.Quotation.Application.Features.Settings.Commands.UpdatePlan;

public record UpdatePlanCommand(int PlanId, PlanEditDto Model) : ICommand<Result>;
