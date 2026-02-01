using Microsoft.AspNetCore.OpenApi;
using Scalar.AspNetCore;
using Yggdrasil.Catalog.ApiService.Infrastructure;
using Yggdrasil.Catalog.ApiService;
using Yggdrasil.Catalog.Application;
using Yggdrasil.Catalog.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
builder.AddRabbitMQClient("messaging");

#if (UseAspire)
builder.AddServiceDefaults();
#endif
// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(configuration);
builder.AddWebServices();
builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
else
{
    app.UseHsts();
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
