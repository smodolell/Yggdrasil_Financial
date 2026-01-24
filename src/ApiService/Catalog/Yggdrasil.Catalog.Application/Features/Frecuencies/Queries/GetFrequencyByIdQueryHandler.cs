using MapsterMapper;
using Yggdrasil.Catalog.Application.Features.Frecuencies.DTOs;
using Yggdrasil.Catalog.Application.Interfaces;

namespace Yggdrasil.Catalog.Application.Features.Frecuencies.Queries;

internal class GetFrequencyByIdQueryHandler : IQueryHandler<GetFrequencyByIdQuery, Result<FrequencyDto>>
{
    private readonly IFrequencyRepository _frequencyRepository;
    private readonly IMapper _mapper;
    public GetFrequencyByIdQueryHandler(
        IFrequencyRepository frequencyRepository,
        IMapper mapper)
    {
        _frequencyRepository = frequencyRepository;
        _mapper = mapper;
    }
    public async Task<Result<FrequencyDto>> HandleAsync(GetFrequencyByIdQuery query, CancellationToken cancellationToken = default)
    {
        var frequency = await _frequencyRepository.GetByIdAsync(query.FrequencyId, cancellationToken);
        if (frequency == null)
        {
            return Result.NotFound($"No se encontró una frecuencia con Id {query.FrequencyId}.");
        }
        var frequencyDto = _mapper.Map<FrequencyDto>(frequency);
        return Result.Success(frequencyDto);
    }
}