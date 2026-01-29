using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace Yggdrasil.Site.Infrastructure;

public class AccessTokenDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AccessTokenDelegatingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext != null)
        {
            // Obtener el access_token de las propiedades de autenticación del usuario.
            // Esto asume que 'SaveTokens = true' está configurado en tu AddOpenIdConnect.
            var accessToken = await httpContext.GetTokenAsync("access_token");

            if (!string.IsNullOrEmpty(accessToken))
            {
                // Adjuntar el token al encabezado de la solicitud.
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            else
            {
                // Opcional: Manejar el caso en que no hay token.
                // Podrías redirigir al login o lanzar una excepción, dependiendo de tu lógica.
                // Console.WriteLine("Warning: No access token found in session for API call.");
            }
        }

        // Continuar con el pipeline de la solicitud HTTP.
        return await base.SendAsync(request, cancellationToken);
    }
}
