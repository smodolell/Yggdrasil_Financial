using Yggdrasil.Quotation.Application.Features.Catalogs.DTOs;

namespace Yggdrasil.Quotation.Application.Features.Catalogs.Queries;

public record GetAllPaymentTermQuery : IQuery<List<PaymentTermDto>>;
