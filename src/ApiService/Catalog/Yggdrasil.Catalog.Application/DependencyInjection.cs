using LiteBus.Commands;
using LiteBus.Extensions.Microsoft.DependencyInjection;
using LiteBus.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Yggdrasil.Catalog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<AssemblyReference>();

        services.AddLiteBus(configuration =>
        {
            configuration.AddCommandModule(m => m.RegisterFromAssembly(typeof(AssemblyReference).Assembly)); 
            configuration.AddQueryModule(m => m.RegisterFromAssembly(typeof(AssemblyReference).Assembly));
        });
        
        return services;
    }


}
public class AssemblyReference { }
