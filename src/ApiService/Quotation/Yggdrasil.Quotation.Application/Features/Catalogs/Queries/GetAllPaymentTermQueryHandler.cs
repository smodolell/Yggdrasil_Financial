using MapsterMapper;
using Yggdrasil.Quotation.Application.Features.Catalogs.DTOs;
using Yggdrasil.Quotation.Application.Repositories;

namespace Yggdrasil.Quotation.Application.Features.Catalogs.Queries;

internal class GetAllPaymentTermQueryHandler(
    IPaymentTermRepository paymentTermRepository,
    IMapper mapper
) : IQueryHandler<GetAllPaymentTermQuery, List<PaymentTermDto>>
{
    private readonly IPaymentTermRepository _paymentTermRepository = paymentTermRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<List<PaymentTermDto>> HandleAsync(GetAllPaymentTermQuery query, CancellationToken cancellationToken = default)
    {
        var paymentTerms = await _paymentTermRepository.GetAllAsync();
        return _mapper.Map<List<PaymentTermDto>>(paymentTerms);
    }
}
