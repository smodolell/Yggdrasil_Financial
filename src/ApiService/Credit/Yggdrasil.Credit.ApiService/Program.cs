using Scalar.AspNetCore;
using Yggdrasil.Credit.ApiService.Infrastructure;
using Yggdrasil.Credit.Application;
using Yggdrasil.Credit.Infrastructure;
using Yggdrasil.Credit.ApiService;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.AddRabbitMQClient("messaging");

builder.AddServiceDefaults();

// Add services to the container.
#if (UseAspire)
builder.AddServiceDefaults();
#endif
// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(configuration);
builder.AddWebServices();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Yggdrasil API Documentation");
        options.WithTheme(ScalarTheme.Saturn);
        options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        options.HideSearch = true;// Habilita/Deshabilita el buscador (Ctrl+K)
        options.ShowSidebar = true; // Muestra u oculta la barra lateral
        options.DarkMode = true;
    });
}

#if (!UseAspire)
app.UseHealthChecks("/health");
#endif
app.UseExceptionHandler(options => { });

app.UseHttpsRedirection();
app.UseStaticFiles();


app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options.WithTitle("Yggdrasil API Documentation");
    options.WithTheme(ScalarTheme.DeepSpace);
    options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    options.HideSearch = true;// Habilita/Deshabilita el buscador (Ctrl+K)
    options.ShowSidebar = true; // Muestra u oculta la barra lateral
    options.DarkMode = false;
});
app.UseRouting();

app.Map("/", () => Results.Redirect("/scalar"));

#if (UseAspire)
app.MapDefaultEndpoints();

#endif

app.MapEndpoints();

app.MapControllers();


app.Run();
