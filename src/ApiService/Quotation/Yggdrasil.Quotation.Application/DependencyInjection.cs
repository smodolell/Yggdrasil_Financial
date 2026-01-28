using FluentValidation;
using LiteBus.Commands;
using LiteBus.Events;
using LiteBus.Extensions.Microsoft.DependencyInjection;
using LiteBus.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Yggdrasil.Quotation.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<Dummy>();

        services.AddLiteBus(configuration =>
        {
            var assembly = typeof(DependencyInjection).Assembly;

            configuration.AddCommandModule(m => m.RegisterFromAssembly(assembly)); 
            configuration.AddQueryModule(m => m.RegisterFromAssembly(assembly));
            configuration.AddEventModule(m => m.RegisterFromAssembly(assembly));
            
        });
        //services.AddTransient(typeof(IQueryPreHandler<>), typeof(QueryValidationPreHandler<>));
        //services.AddScoped<ISelectListService, SelectListService>();
     

        return services;
    }


}
class Dummy { }
