using MapsterMapper;
using Yggdrasil.Quotation.Application.Features.Settings.DTOs;
using Yggdrasil.Quotation.Application.Repositories;
using Yggdrasil.Quotation.Application.Specifications;

namespace Yggdrasil.Quotation.Application.Features.Settings.Queries.GetPlanById;

internal class GetPlanByIdQueryHandler(
    IPlanRepository planRepository,
    IMapper mapper

) : IQueryHandler<GetPlanByIdQuery, Result<PlanViewDto>>
{
    private readonly IPlanRepository _planRepository = planRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<PlanViewDto>> HandleAsync(GetPlanByIdQuery message, CancellationToken cancellationToken = default)
    {
        var plan = await _planRepository.GetBySpecAsync(new PlanByIdWithDetail(message.PlanId));
        if (plan == null)
        {
            return Result.NotFound("Plan no encontrado");
        }

        var result = _mapper.Map<PlanViewDto>(plan);
        return Result.Success(result);
    }
}
