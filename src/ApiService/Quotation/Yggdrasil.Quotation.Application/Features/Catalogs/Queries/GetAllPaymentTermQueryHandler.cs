using MapsterMapper;
using Yggdrasil.Quotation.Application.Features.Catalogs.DTOs;
using Yggdrasil.Quotation.Application.Repositories;

namespace Yggdrasil.Quotation.Application.Features.Catalogs.Queries;

internal class GetAllPaymentTermQueryHandler(
    IPaymentTermRepository paymentTermRepository,
    IMapper mapper
) : IQueryHandler<GetAllPaymentTermQuery, Result<List<PaymentTermDto>>>
{
    private readonly IPaymentTermRepository _paymentTermRepository = paymentTermRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<List<PaymentTermDto>>> HandleAsync(GetAllPaymentTermQuery query, CancellationToken cancellationToken = default)
    {
        var paymentTerms = await _paymentTermRepository.GetAllAsync();
        var result = _mapper.Map<List<PaymentTermDto>>(paymentTerms);
        return Result.Success(result);
    }
}
