using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Yggdrasil.Credit.Application.Common.Interfaces;
using Yggdrasil.Credit.Application.Interfaces;
using Yggdrasil.Credit.Infrastructure.Common.Services;
using Yggdrasil.Credit.Infrastructure.Extensions;
using Yggdrasil.Credit.Infrastructure.Persistence;
using Yggdrasil.Credit.Infrastructure.Persistence.Repositories;
using Yggdrasil.Credit.Infrastructure.Persistence.UnitOfWork;


namespace Yggdrasil.Credit.Infrastructure;

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
            options.UseSqlite(connectionString);
        }, ServiceLifetime.Scoped);

        services.AddDbContextFactory<ApplicationDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        }, ServiceLifetime.Scoped);


        services.ConfigureMassTransit(configuration);


        //Repositorios
        services.AddScoped<IDynamicSorter, DynamicSorter>();
        services.AddScoped<IPaginator, Paginator>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();


        services.AddScoped<IProductRepository , ProductRepository>();


        
        // 11. Servicios Inicializadores
        //services.AddHostedService<UserInitializer>();
        //services.AddHostedService<JwtBearerInitializer>();
        //services.AddHostedService<SyncPhasesInitializer>();

        return services;
    }
}