using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Yggdrasil.Quotation.Application.Common.Interfaces;
using Yggdrasil.Quotation.Application.Repositories;
using Yggdrasil.Quotation.Infrastructure.Persistence;
using Yggdrasil.Quotation.Infrastructure.Persistence.Repositories;

namespace Yggdrasil.Quotation.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Configuración de Mapster
        MapsterConfig.Configure();
        
        services.AddMapster();


        // 2. Configuración de Base de Datos
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlite(connectionString, b => b.MigrationsAssembly("Yggdrasil.Quotation.Infrastructure"));
        }, ServiceLifetime.Scoped);
       
        services.AddDbContextFactory<ApplicationDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        }, ServiceLifetime.Scoped);


        //Repositorios
        services.AddScoped<IDynamicSorter, DynamicSorter>();
        services.AddScoped<IPaginator, Paginator>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IInterestRateRepository, InterestRateRepository>();
        services.AddScoped<IPaymentTermRepository, PaymentTermRepository>();
        services.AddScoped<ITaxRateRepository, TaxRateRepository>();

        //// 11. Servicios Inicializadores
        //services.AddHostedService<UserInitializer>();
        //services.AddHostedService<JwtBearerInitializer>();
        //services.AddHostedService<SyncPhasesInitializer>();

        return services;
    }
}