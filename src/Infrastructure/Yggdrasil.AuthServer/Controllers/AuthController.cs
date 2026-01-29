using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Yggdrasil.AuthServer.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("~/connect/token"), IgnoreAntiforgeryToken]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        if (request == null)
        {
            // Si la solicitud de OpenIddict es nula, algo está mal.
            return BadRequest(new OpenIddictResponse { Error = Errors.InvalidRequest, ErrorDescription = "Solicitud de OpenIddict nula." });
        }

        // --- Manejo del flujo de código de autorización (authorization_code) ---
        // Este es el flujo que tu Blazor Server App está usando con PKCE.
        if (request.IsAuthorizationCodeGrantType())
        {
            // Recupera el principal de autenticación asociado con el código de autorización.
            // OpenIddict ya ha validado el código, el code_verifier, client_id, etc.
            var principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;

            if (principal == null)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Código de autorización inválido o expirado."
                    }));
            }

            // Recupera el usuario de Identity usando el Subject claim del principal.
            var userId = principal.GetClaim(Claims.Subject);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Usuario asociado al código no encontrado."
                    }));
            }

            // Asegúrate de que el principal contenga los claims necesarios para los tokens.
            // Puedes añadir claims adicionales aquí si es necesario.
            var identity = (ClaimsIdentity)principal.Identity;

            // Asegura que los claims de nombre y email estén presentes si se solicitan.
            if (!identity.HasClaim(Claims.Name) && !string.IsNullOrEmpty(user.UserName))
            {
                identity.AddClaim(new Claim(Claims.Name, user.UserName).SetDestinations(Destinations.AccessToken, Destinations.IdentityToken));
            }
            if (!identity.HasClaim(Claims.Email) && !string.IsNullOrEmpty(user.Email))
            {
                identity.AddClaim(new Claim(Claims.Email, user.Email).SetDestinations(Destinations.AccessToken, Destinations.IdentityToken));
            }

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        // --- Manejo del flujo de credenciales de contraseña (password) ---
        // Solo si tu servidor OpenIddict también soporta este flujo.
        // Este flujo es menos seguro y no se recomienda para aplicaciones públicas como Blazor Server.
        if (request.IsPasswordGrantType())
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Invalid username or password"
                    }));
            }

            var identity = new ClaimsIdentity(
                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                nameType: Claims.Name,
                roleType: Claims.Role);

            identity.AddClaim(new Claim(Claims.Subject, user.Id ?? string.Empty)
                .SetDestinations(Destinations.AccessToken, Destinations.IdentityToken));

            identity.AddClaim(new Claim(Claims.Name, user.UserName ?? string.Empty)
                .SetDestinations(Destinations.AccessToken, Destinations.IdentityToken));

            // Asegúrate de que el Audience sea correcto si lo necesitas en el token de acceso
            identity.AddClaim(new Claim(Claims.Audience, "yggdrasil_app")
                .SetDestinations(Destinations.AccessToken));

            var email = await _userManager.GetEmailAsync(user);
            if (!string.IsNullOrEmpty(email))
            {
                identity.AddClaim(new Claim(Claims.Email, email)
                    .SetDestinations(Destinations.AccessToken, Destinations.IdentityToken));
            }

            var principal = new ClaimsPrincipal(identity);
            principal.SetScopes(request.GetScopes()); // Asegúrate de establecer los scopes solicitados
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        // Si el grant type no es reconocido o no es soportado, devuelve un error.
        return BadRequest(new OpenIddictResponse
        {
            Error = Errors.UnsupportedGrantType,
            ErrorDescription = "El tipo de concesión solicitado no es compatible con este endpoint."
        });
    }

    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [HttpGet("~/connect/userinfo")]
    public async Task<IActionResult> Userinfo()
    {
        // Debug: Imprime todos los claims
        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value; // Fallback para OpenID

        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(new { error = "Token inválido: falta claim 'sub'", claims });
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound(new { error = $"Usuario no encontrado para ID: {userId}", claims });
        }

        return Ok(new
        {
            sub = userId,
            email = user.Email,
            username = user.UserName
        });
    }

    //[HttpPost("~/connect/logout")]
    //public async Task<IActionResult> CustomLogout()
    //{
    //    var request = HttpContext.GetOpenIddictServerRequest();
    //    if (request == null)
    //    {
    //        return BadRequest();
    //    }

    //    // Validación manual del id_token_hint
    //    if (!string.IsNullOrEmpty(request.IdTokenHint))
    //    {
    //        try
    //        {
    //            //var principal = await httpcontext.validatetokenasync(request.idtokenhint);
    //            // Lógica adicional con el usuario
    //        }
    //        catch (SecurityTokenInvalidTypeException)
    //        {
    //            return BadRequest(new { error = "Invalid ID token type" });
    //        }
    //    }

    //    await HttpContext.SignOutAsync();
    //    return SignOut(
    //        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
    //        properties: new AuthenticationProperties { RedirectUri = "/" });
    //}


}


//using Microsoft.AspNetCore;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;
//using OpenIddict.Abstractions;
//using OpenIddict.Server.AspNetCore;
//using OpenIddict.Validation.AspNetCore;
//using System.Security.Claims;
//using static OpenIddict.Abstractions.OpenIddictConstants;

//namespace AuthServer.Controllers;

//[ApiController]
//public class AuthController : ControllerBase
//{
//    private readonly UserManager<IdentityUser> _userManager;
//    private readonly SignInManager<IdentityUser> _signInManager;

//    public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
//    {
//        _userManager = userManager;
//        _signInManager = signInManager;
//    }
//    [HttpPost("~/connect/token"), IgnoreAntiforgeryToken]
//    public async Task<IActionResult> Exchange()
//    {
//        var request = HttpContext.GetOpenIddictServerRequest();
//        if (request == null) return BadRequest();

//        if (request == null || !request.IsPasswordGrantType())
//            throw new InvalidOperationException("Invalid grant type.");

//        if (request.GrantType == GrantTypes.Password)
//        {
//            var user = await _userManager.FindByNameAsync(request.Username);
//            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
//                return Forbid(
//                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
//                    properties: new AuthenticationProperties(new Dictionary<string, string?>
//                    {
//                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
//                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "Invalid username or password"
//                    }));

//            var identity = new ClaimsIdentity(
//                authenticationType: TokenValidationParameters.DefaultAuthenticationType,
//                nameType: Claims.Name,
//                roleType: Claims.Role);

//            identity.AddClaim(new Claim(Claims.Subject, user.Id ?? string.Empty)
//                .SetDestinations(Destinations.AccessToken));

//            identity.AddClaim(new Claim(Claims.Name, user.UserName ?? string.Empty)
//                .SetDestinations(Destinations.AccessToken));

//            identity.AddClaim(new Claim(Claims.Audience, "yggdrasil_app")
//                .SetDestinations(Destinations.AccessToken));

//            var email = await _userManager.GetEmailAsync(user);
//            if (!string.IsNullOrEmpty(email))
//            {
//                identity.AddClaim(new Claim(Claims.Email, email)
//                    .SetDestinations(Destinations.AccessToken));
//            }

//            var principal = new ClaimsPrincipal(identity);
//            principal.SetScopes(request.GetScopes());
//            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
//        }

//        return BadRequest();
//    }

//    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
//    [HttpGet("~/connect/userinfo")]
//    public async Task<IActionResult> Userinfo()
//    {
//        // Debug: Imprime todos los claims
//        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

//        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
//                  ?? User.FindFirst("sub")?.Value; // Fallback para OpenID

//        if (string.IsNullOrEmpty(userId))
//        {
//            return BadRequest(new { error = "Token inválido: falta claim 'sub'", claims });
//        }

//        var user = await _userManager.FindByIdAsync(userId);
//        if (user == null)
//        {
//            return NotFound(new { error = $"Usuario no encontrado para ID: {userId}", claims });
//        }

//        return Ok(new
//        {
//            sub = userId,
//            email = user.Email,
//            username = user.UserName
//        });
//    }

//    [HttpPost("~/connect/logout")]
//    public async Task<IActionResult> CustomLogout()
//    {
//        var request = HttpContext.GetOpenIddictServerRequest();
//        if (request == null)
//        {
//            return BadRequest();
//        }

//        // Validación manual del id_token_hint
//        if (!string.IsNullOrEmpty(request.IdTokenHint))
//        {
//            try
//            {
//                //var principal = await httpcontext.validatetokenasync(request.idtokenhint);
//                // Lógica adicional con el usuario
//            }
//            catch (SecurityTokenInvalidTypeException)
//            {
//                return BadRequest(new { error = "Invalid ID token type" });
//            }
//        }

//        await HttpContext.SignOutAsync();
//        return SignOut(
//            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
//            properties: new AuthenticationProperties { RedirectUri = "/" });
//    }
//}
