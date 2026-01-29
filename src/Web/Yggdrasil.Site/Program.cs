using BitzArt.Blazor.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using MudBlazor.Services;
using System.Globalization;
using Yggdrasil.Site;
using Yggdrasil.Site.Components;
using Yggdrasil.Site.Extensions;
using Yggdrasil.Site.Infrastructure;
using Yggdrasil.Site.Services;
using Yggdrasil.Site.States;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();


builder.Services.AddMudServices();
builder.Services.AddScoped<ILayoutService, LayoutService>();
builder.Services.AddScoped<ThemeState>();
builder.Services.AddScoped<LayoutState>();

var cultureInfo = new CultureInfo("es-AR")
{
    NumberFormat =
        {
            NumberDecimalSeparator = ",",
            NumberGroupSeparator = ".",
            CurrencySymbol = "$"
        },
    DateTimeFormat =
        {
            ShortDatePattern = "dd/MM/yyyy",
            LongDatePattern = "dddd, d 'de' MMMM 'de' yyyy"
        }
};
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
            cultureInfo,
            new CultureInfo("es"),
            new CultureInfo("en")
        };

    options.DefaultRequestCulture = new RequestCulture("es-AR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    // Opcional: proveedores de cultura
    options.RequestCultureProviders = new List<IRequestCultureProvider>
        {
            new QueryStringRequestCultureProvider(),
            new CookieRequestCultureProvider(),
            new AcceptLanguageHeaderRequestCultureProvider()
        };
});


builder.Services.AddYggdrasilOpenIdConnect(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<AccessTokenDelegatingHandler>();

// Add services to the container.
builder.AddBlazorCookies();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();

builder.Services.AddHttpClient<WeatherApiClient>(client =>
    {
        // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
        // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
        client.BaseAddress = new("https+http://apiservice");
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.UseHttpsRedirection();

app.UseRouting();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
