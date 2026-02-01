using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Yggdrasil.Contracts.Common.Intefaces;
using Yggdrasil.Contracts.IntegrationEvents.Catalog;
using Yggdrasil.Credit.Application.Consumers.Catalog;
using Yggdrasil.Credit.Infrastructure.Messaging;

namespace Yggdrasil.Credit.Infrastructure.Extensions;

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

        services.AddScoped<IEventConsumer<ProductCreatedEvent>, ProductCreatedEventHandler>();

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

                cfg.ReceiveEndpoint("credit-product-created", e =>
                {
                    e.ConfigureConsumer<MassTransitEventConsumer<ProductCreatedEvent>>(context);
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                    // Configuración específica de la cola
                    e.Durable = true;
                    e.AutoDelete = false;
                    e.PrefetchCount = 10;

                    // Si necesitas dead letter queue
                    e.SetQueueArgument("x-dead-letter-exchange", "dead-letter-exchange");
                    e.SetQueueArgument("x-dead-letter-routing-key", "credit-product-created.error");
                });

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