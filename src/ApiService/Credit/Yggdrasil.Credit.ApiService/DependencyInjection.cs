using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Yggdrasil.Credit.ApiService.Infrastructure;
using Yggdrasil.Credit.Infrastructure.Persistence;

namespace Yggdrasil.Credit.ApiService;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddHttpContextAccessor();

#if (!UseAspire)
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();
#endif

        builder.Services.AddExceptionHandler<CustomExceptionHandler>();

        // Customise default API behaviour
        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info.Contact = new() { Name = "Soporte Técnico", Email = "soporte@yggdrasil.com" };
                document.Info.License = new() { Name = "MIT" };
                return Task.CompletedTask;
            });
        });
        builder.Services.AddSwaggerGen(c =>
        {
            // 1. Documentación de la propia API
            var apiXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, apiXml));

            // 2. Documentación de la Capa de Aplicación
            // Usamos CreateInterestRateCommand para encontrar la ruta del ensamblado de Aplicación
            var appXml = $"{typeof(Application.DependencyInjection).Assembly.GetName().Name}.xml";
            var appPath = Path.Combine(AppContext.BaseDirectory, appXml);

            if (File.Exists(appPath))
            {
                c.IncludeXmlComments(appPath);
            }
        });
    }

    //public static void AddKeyVaultIfConfigured(this IHostApplicationBuilder builder)
    //{
    //    var keyVaultUri = builder.Configuration["AZURE_KEY_VAULT_ENDPOINT"];
    //    if (!string.IsNullOrWhiteSpace(keyVaultUri))
    //    {
    //        builder.Configuration.AddAzureKeyVault(
    //            new Uri(keyVaultUri),
    //            new DefaultAzureCredential());
    //    }
    //}
}
