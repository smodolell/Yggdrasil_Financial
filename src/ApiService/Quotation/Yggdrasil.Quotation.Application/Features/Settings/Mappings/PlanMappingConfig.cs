using Mapster;
using Yggdrasil.Quotation.Application.Features.Settings.DTOs;

namespace Yggdrasil.Quotation.Application.Features.Settings.Mappings;

public class PlanMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Plan, PlanViewDto>()
            .Map(dest => dest.PaymentTerms, src => src.PlanPaymentTerms);

        config.NewConfig<PlanPaymentTerm, PlanPaymentTermViewDto>()
            .Map(dest => dest.PaymentTermName, src => src.PaymentTerm.Name)
            .Map(dest => dest.PaymentTermCode, src => src.PaymentTerm.Code)
            .Map(dest => dest.NumberOfPayments, src => src.PaymentTerm.NumberOfPayments)
            .Map(dest => dest.InterestRateName, src => src.InterestRate.RateName)
            .Map(dest => dest.InterestRateAnnualPercentage, src => src.InterestRate.AnnualPercentage);
    }
}