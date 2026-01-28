using Yggdrasil.Quotation.Application.Features.Settings.DTOs;

namespace Yggdrasil.Quotation.Application.Features.Settings.Commands.CreatePlan;

public record CreatePlanCommand(PlanEditDto Model) : ICommand<Result<int>>;
