using Yggdrasil.Quotation.Application.Features.Quotation.Dtos;
using Yggdrasil.Quotation.Application.Repositories;

namespace Yggdrasil.Quotation.Application.Features.Quotation.Queries.Quote;

public class QuoteQuery : IQuery<Result<QuoteDto>>
{
    public int PlanId { get; set; }


}

//internal class QuoteQueryHandler(IPlanRepository planRepository) : IQueryHandler<QuoteQuery, Result<QuoteDto>>
//{
//    private readonly IPlanRepository _planRepository = planRepository;

//    public async Task<Result<QuoteDto>> HandleAsync(QuoteQuery message, CancellationToken cancellationToken = default)
//    {
//        var plan = await _planRepository.GetByIdAsync(message.PlanId);


        
//    }
//}
