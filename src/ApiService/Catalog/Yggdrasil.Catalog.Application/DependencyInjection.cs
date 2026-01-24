using FluentValidation;
using LiteBus.Commands;
using LiteBus.Extensions.Microsoft.DependencyInjection;
using LiteBus.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Yggdrasil.Catalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<Dummy>();

        services.AddLiteBus(configuration =>
        {
            configuration.AddCommandModule(m => m.RegisterFromAssembly(typeof(DependencyInjection).Assembly)); 
            configuration.AddQueryModule(m => m.RegisterFromAssembly(typeof(DependencyInjection).Assembly));
        });

        //services.AddScoped<ISelectListService, SelectListService>();
        //services.AddScoped<ILocalizerService, LocalizerService>();

        //services.AddScoped<IInterestRateService, InterestRateService>();
        //services.AddScoped<IFrequencyService, FrequencyService>();
        //services.AddScoped<ITaxRateService, TaxRateService>();
        //services.AddScoped<IPaymentTermService, PaymentTermService>();

        return services;
    }


}
class Dummy { }
