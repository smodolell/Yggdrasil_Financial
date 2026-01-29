using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Yggdrasil.AuthServer.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AccountController(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    // GET: /connect/authorize
    // Este método maneja la solicitud de autorización inicial del cliente.
    [HttpGet("~/connect/authorize")]
    [HttpPost("~/connect/authorize")] // También soporta POST si usas un formulario de consentimiento
    [IgnoreAntiforgeryToken] // Necesario para solicitudes de OpenIddict que no vienen de formularios de tu app
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request == null)
        {
            return BadRequest("Solicitud de OpenIddict no válida.");
        }

        // Si el usuario no está autenticado en el servidor de identidad,
        // redirigirlo a la página de login de ASP.NET Core Identity.
        // La URL de retorno se pasa para que después del login, el usuario
        // sea redirigido de nuevo a este endpoint de autorización con los parámetros originales.
        if (!_signInManager.IsSignedIn(User))
        {
            var returnUrl = $"{Request.PathBase}{Request.Path}{Request.QueryString}";
            return RedirectToPage("/Account/Login", new { returnUrl });
        }

        // Si el usuario ya está autenticado, procesar la solicitud de autorización.
        // Aquí es donde normalmente mostrarías una página de consentimiento al usuario
        // si el cliente solicita nuevos scopes o si el usuario no ha dado consentimiento previamente.
        // Para simplificar, asumiremos consentimiento automático para clientes registrados.

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            // Esto no debería ocurrir si el usuario está autenticado.
            return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        // Crear una nueva identidad de claims para el usuario autenticado.
        var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        // Añadir los claims básicos del usuario.


        identity.AddClaim(new Claim(Claims.Subject, await _userManager.GetUserIdAsync(user) ?? "")
       .SetDestinations(Destinations.AccessToken, Destinations.IdentityToken));

        identity.AddClaim(new Claim(Claims.Email, await _userManager.GetEmailAsync(user) ?? "")
         .SetDestinations(Destinations.AccessToken, Destinations.IdentityToken));

        identity.AddClaim(new Claim(Claims.Name, await _userManager.GetUserNameAsync(user) ?? "")
            .SetDestinations(Destinations.AccessToken, Destinations.IdentityToken));

        identity.AddClaim(new Claim(Claims.Audience, "yggdrasil_app")
        .SetDestinations(Destinations.AccessToken, Destinations.IdentityToken));

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            //identity.AddClaim(OpenIddictConstants.Claims.Role, role);
            // También puedes agregar el rol como un claim de tipo "role" estándar de JWT si lo prefieres,
            // aunque OpenIddictConstants.Claims.Role es la forma recomendada por OpenIddict.
            identity.AddClaim(new Claim(Claims.Role, role)
                .SetDestinations(Destinations.AccessToken, Destinations.IdentityToken));

            //identity.AddClaim(new Claim(ClaimTypes.Role, role));
        }

        // Iterar sobre los scopes solicitados por el cliente y agregarlos a la identidad.
        // Solo agrega los scopes que tu servidor está dispuesto a conceder.
        // Puedes añadir lógica aquí para filtrar scopes o requerir consentimiento específico.
        foreach (var scope in request.GetScopes())
        {
            identity.AddClaim(new Claim(Claims.Scope, scope)
                .SetDestinations(Destinations.AccessToken, Destinations.IdentityToken));
        }

        var principal = new ClaimsPrincipal(identity);

        // Emitir el código de autorización.
        // Esto redirigirá de vuelta a la RedirectUri de tu cliente Blazor Server con el código.
        // El esquema de autenticación OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
        // es el que OpenIddict usa para emitir tokens.
        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    // GET y POST para el endpoint de cierre de sesión
    // Este método maneja la solicitud de cierre de sesión del cliente.
    [HttpGet("~/connect/logout")]
    [HttpPost("~/connect/logout")]
    [IgnoreAntiforgeryToken] // Importante para solicitudes que no vienen de formularios de tu app
    public async Task<IActionResult> Logout()
    {
        // Obtiene la solicitud de OpenIddict.
        // Esto contendrá el post_logout_redirect_uri si el cliente lo envió.
        var request = HttpContext.GetOpenIddictServerRequest();

        // Cierra la sesión del usuario en ASP.NET Core Identity (elimina la cookie de autenticación).
        await _signInManager.SignOutAsync();

        // Le indica a OpenIddict que complete el flujo de cierre de sesión.
        // OpenIddict buscará automáticamente el post_logout_redirect_uri en la solicitud
        // y redirigirá a esa URL si es válida y está registrada para el cliente.
        return SignOut(
            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties
            {
                // Puedes añadir propiedades adicionales si es necesario.
                // OpenIddict se encargará de la redirección al PostLogoutRedirectUri
                // si está presente en la solicitud y es válido.
            });
    }

    // POST: /connect/authorize (para manejar el consentimiento si tuvieras un formulario)
    // Si tuvieras una página de consentimiento real con botones "Aceptar" / "Denegar",
    // este método POST recibiría la respuesta del usuario.
    // Por ahora, el GET maneja la lógica principal asumiendo consentimiento automático.
    [HttpPost("~/connect/authorize/accept")] // Un endpoint de ejemplo para aceptar el consentimiento
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Accept()
    {
        // Lógica para procesar el consentimiento si el usuario lo acepta.
        // Esto es un placeholder; la lógica real dependería de tu UI de consentimiento.
        return await Authorize(); // Por simplicidad, volvemos a la lógica GET
    }

    [HttpPost("~/connect/authorize/deny")] // Un endpoint de ejemplo para denegar el consentimiento
    [IgnoreAntiforgeryToken]
    public IActionResult Deny()
    {
        // Lógica para denegar la solicitud de autorización.
        // Esto enviaría un error al cliente.
        return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}
