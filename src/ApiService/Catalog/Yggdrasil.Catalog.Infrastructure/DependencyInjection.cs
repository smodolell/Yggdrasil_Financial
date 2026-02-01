using Mapster;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Metadata;
using Yggdrasil.Catalog.Application.Common.Interfaces;
using Yggdrasil.Catalog.Application.Interfaces;
using Yggdrasil.Catalog.Infrastructure.Common.Services;
using Yggdrasil.Catalog.Infrastructure.Extensions;
using Yggdrasil.Catalog.Infrastructure.Messaging;
using Yggdrasil.Catalog.Infrastructure.Persistence;
using Yggdrasil.Catalog.Infrastructure.Persistence.Repositories;
using Yggdrasil.Catalog.Infrastructure.Persistence.UnitOfWork;
using Yggdrasil.Contracts.Common.Intefaces;

namespace Yggdrasil.Catalog.Infrastructure;

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
        services.AddScoped<IFrequencyRepository, FrequencyRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();


        
        // 11. Servicios Inicializadores
        //services.AddHostedService<UserInitializer>();
        //services.AddHostedService<JwtBearerInitializer>();
        //services.AddHostedService<SyncPhasesInitializer>();

        return services;
    }
}