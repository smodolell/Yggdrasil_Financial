using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Yggdrasil.Site.Extensions.Options;

namespace Yggdrasil.Site.Extensions;

public static class OpenIdConnectExtensions
{
    public static IServiceCollection AddYggdrasilOpenIdConnect(this IServiceCollection services,
        IConfiguration configuration, string configSection = "OpenIdConnect")
    {
        services.Configure<OpenIdConnectSettings>(configuration.GetSection(configSection));

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddOpenIdConnect(options =>
        {
            var settings = services.BuildServiceProvider()
                .GetRequiredService<IOptions<OpenIdConnectSettings>>().Value;

            ConfigureOpenIdConnect(options, settings);
        });

        return services;
    }

    private static void ConfigureOpenIdConnect(OpenIdConnectOptions options, OpenIdConnectSettings settings)
    {
        options.Authority = settings.Authority;
        options.ClientId = settings.ClientId;
        options.ClientSecret = settings.ClientSecret;
        options.ResponseType = settings.ResponseType;
        options.UsePkce = settings.UsePkce;

        foreach (var scope in settings.Scopes)
        {
            options.Scope.Add(scope);
        }


        options.SaveTokens = settings.SaveTokens;
        options.GetClaimsFromUserInfoEndpoint = settings.GetClaimsFromUserInfoEndpoint;

        // Esto evita que .NET "filtre" o "renombre" ciertos claims.
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new()
        {
            NameClaimType = settings.NameClaimType,
            RoleClaimType = settings.RoleClaimType,

        };

        options.CallbackPath = settings.CallbackPath;

        options.Events = new OpenIdConnectEvents
        {
            OnRemoteFailure = context =>
            {
                Console.WriteLine($"Error de autenticación remota: {context.Failure?.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validado exitosamente.");
                return Task.CompletedTask;
            },
            OnAccessDenied = context =>
            {
                Console.WriteLine($"Acceso denegado: {context.Result?.Failure?.Message}");
                return Task.CompletedTask;
            }
        };
    }
}
