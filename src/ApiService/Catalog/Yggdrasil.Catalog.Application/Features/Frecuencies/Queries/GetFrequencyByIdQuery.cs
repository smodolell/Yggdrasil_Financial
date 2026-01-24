using Yggdrasil.Catalog.Application.Features.Frecuencies.DTOs;

namespace Yggdrasil.Catalog.Application.Features.Frecuencies.Queries;

public record GetFrequencyByIdQuery(int FrequencyId) : IQuery<Result<FrequencyDto>>;
