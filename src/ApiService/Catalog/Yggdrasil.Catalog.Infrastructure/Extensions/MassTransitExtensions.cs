using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Yggdrasil.Catalog.Infrastructure.Messaging;
using Yggdrasil.Contracts.Common.Intefaces;
using Yggdrasil.Contracts.IntegrationEvents.Catalog;

namespace Yggdrasil.Catalog.Infrastructure.Extensions;

public static class MassTransitExtensions
{
    public static IServiceCollection ConfigureMassTransit(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ======================
        // 1. REGISTRAR CONSUMIDORES
        // ======================

        // Registrar consumidores de aplicación (IEventConsumer<T>)
        //services.AddScoped<IEventConsumer<OrderCreatedEvent>, OrderCreatedEventConsumer>();
        //services.AddScoped<IEventConsumer<PaymentProcessedEvent>, PaymentProcessedEventConsumer>();
        

        // Registrar adaptadores MassTransitEventConsumer<T>
        //services.AddScoped<MassTransitEventConsumer<OrderCreatedEvent>>();
        //services.AddScoped<MassTransitEventConsumer<PaymentProcessedEvent>>();
        // ... agregar más adaptadores aquí

        // ======================
        // 2. REGISTRAR PUBLICADORES
        // ======================

        // Opción A: Publisher genérico
        services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

        // Opción B: Publishers específicos por tipo de evento
        services.AddScoped<IEventPublisher<ProductCreatedEvent>, MassTransitEventPublisher<ProductCreatedEvent>>();
        // ... agregar más publishers específicos aquí

        // ======================
        // 3. CONFIGURAR MASS TRANSIT
        // ======================

        services.AddMassTransit(x =>
        {
            // Registrar consumidores MassTransit
            //x.AddConsumer<MassTransitEventConsumer<OrderCreatedEvent>>();
            //x.AddConsumer<MassTransitEventConsumer<PaymentProcessedEvent>>();
            // ... registrar más consumidores aquí

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration.GetConnectionString("messaging"));

                // ======================
                // 4. CONFIGURAR ENDPOINTS (COLAS)
                // ======================

                // Endpoints para RECIBIR eventos (consumidores)
                //cfg.ReceiveEndpoint("catalog-order-created", e =>
                //{
                //    e.ConfigureConsumer<MassTransitEventConsumer<OrderCreatedEvent>>(context);
                //    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                //});

                //cfg.ReceiveEndpoint("catalog-payment-processed", e =>
                //{
                //    e.ConfigureConsumer<MassTransitEventConsumer<PaymentProcessedEvent>>(context);
                //    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                //});
                // ... más endpoints de consumo

                // NOTA: No necesitas configurar endpoints para ENVIAR eventos.
                // MassTransit se encarga automáticamente cuando usas Publish.
            });
        });

        return services;
    }
}