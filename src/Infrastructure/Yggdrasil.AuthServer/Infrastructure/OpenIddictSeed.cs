using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Yggdrasil.AuthServer.Infrastructure;

public static class OpenIddictSeed
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var appManager = serviceProvider.GetRequiredService<IOpenIddictApplicationManager>();

       

        if (await appManager.FindByClientIdAsync("yggdrasil-site") is null)
        {
            await appManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "yggdrasil-site",
                ClientSecret = "591a698f-50f9-4fdc-acf9-d94d2cc8c77d",
                DisplayName = "Yggdrasil.Site",
                RedirectUris = { new Uri("https://localhost:7069/signin-oidc") }, // Ajusta el puerto
                PostLogoutRedirectUris = { new Uri("https://localhost:7069/signout-callback-oidc") },

                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Token,
                    Permissions.Endpoints.EndSession,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Roles,
                    Permissions.Prefixes.Scope + "openid",
                    Permissions.Prefixes.Scope + "api",
                    //Permissions.Prefixes.Audience + "api_yggdrasil_process", // <--- Asegúrate de que esto esté aquí
                    Permissions.Prefixes.Scope + "catalog_service" // <--- Y esto para que la app Blazor pueda solicitarlo
                }
            });

        }


        
        var scopeManager = serviceProvider.GetRequiredService<IOpenIddictScopeManager>();

        if (await scopeManager.FindByNameAsync("catalog_service") is null)
        {
            await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = "catalog_service", // Este es el nombre del scope/recurso
                DisplayName = "CatalogService",
                Description = "MicroServicion Catalog",
                Resources = { "catalog_service" } 
            });
        }

    }
}
