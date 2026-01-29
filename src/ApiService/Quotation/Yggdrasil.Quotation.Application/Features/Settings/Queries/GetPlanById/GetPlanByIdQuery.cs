using Yggdrasil.Quotation.Application.Features.Settings.DTOs;

namespace Yggdrasil.Quotation.Application.Features.Settings.Queries.GetPlanById;

public record GetPlanByIdQuery(int PlanId) : IQuery<Result<PlanViewDto>>;
