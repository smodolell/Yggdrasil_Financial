using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using Yggdrasil.AuthServer.Constants;
using Yggdrasil.AuthServer.Models;

namespace Yggdrasil.AuthServer.Controllers;

[Route("api/users")]
[ApiController]
[IgnoreAntiforgeryToken]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class UsersController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        UserManager<IdentityUser> userManager,
        ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Registra un nuevo usuario a través de una solicitud API.
    /// </summary>
    [HttpPost("register")] 
    public async Task<IActionResult> Register([FromBody] RegisterViewModel model) // [FromBody] para deserializar JSON
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new IdentityUser { UserName = model.Email, Email = model.Email };

        // Intenta crear el usuario en la base de datos usando UserManager
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("Usuario creó una nueva cuenta vía API con email: {Email}", model.Email);

            // Opcional: Asignar roles por defecto, enviar correo de confirmación, etc.
            // Ejemplo: Asignar un rol "User" por defecto
            // await _userManager.AddToRoleAsync(user, "User");

            // Si el registro es exitoso, devuelve un 201 Created o 200 OK
            // Puedes devolver un objeto con el ID del usuario o un mensaje de éxito.
            return StatusCode(201, new { Message = "Usuario registrado exitosamente.", UserId = user.Id });
        }

        // Si falla la creación, devuelve un 400 Bad Request con los errores de Identity
        var errors = result.Errors.Select(e => e.Description);
        _logger.LogWarning("Fallo al registrar usuario {Email} vía API: {Errors}", model.Email, string.Join(", ", errors));
        return BadRequest(new { Errors = errors });
    }

    [HttpGet]
    [Authorize(Roles = Roles.Administrator)] 
    public async Task<IActionResult> GetAllUsers()
    {
        // Obtiene todos los usuarios de la base de datos
        var users = _userManager.Users.Select(u => new UserDto
        {
            Id = u.Id,
            Email = u.Email ?? string.Empty,
            UserName = u.UserName ?? string.Empty,
            EmailConfirmed = u.EmailConfirmed
            // Mapea otras propiedades si las tienes en ApplicationUser
        }).ToList();

        _logger.LogInformation("Listando todos los usuarios.");
        return Ok(users);
    }
    [HttpGet("{id}")] // Ruta: api/users/{id}
    [Authorize(Roles = Roles.Administrator)] // Solo administradores pueden ver detalles de usuarios
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            _logger.LogWarning("Intento de obtener usuario con ID {UserId} falló: Usuario no encontrado.", id);
            return NotFound(new { Message = $"Usuario con ID {id} no encontrado." });
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            EmailConfirmed = user.EmailConfirmed
            // Mapea otras propiedades
        };

        _logger.LogInformation("Usuario con ID {UserId} obtenido exitosamente.", id);
        return Ok(userDto);
    }

    [HttpPut("{id}")] // Ruta: api/users/{id}
    [Authorize(Roles = Roles.Administrator)]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserViewModel model)
    {
        // Valida que el ID en la ruta coincida con el ID en el modelo
        if (id != model.Id)
        {
            return BadRequest(new { Message = "El ID del usuario en la ruta no coincide con el ID en el cuerpo de la solicitud." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null)
        {
            _logger.LogWarning("Intento de actualizar usuario con ID {UserId} falló: Usuario no encontrado.", model.Id);
            return NotFound(new { Message = $"Usuario con ID {model.Id} no encontrado." });
        }

        // Actualiza las propiedades básicas
        user.Email = model.Email;
        user.UserName = model.Email; // Opcional: Si UserName siempre es igual al Email

        // Actualiza otras propiedades personalizadas si las tienes en ApplicationUser
        // user.FirstName = model.FirstName;
        // user.LastName = model.LastName;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            var errors = updateResult.Errors.Select(e => e.Description);
            _logger.LogWarning("Fallo al actualizar el usuario {UserId} (Email: {Email}): {Errors}", user.Id, user.Email, string.Join(", ", errors));
            return BadRequest(new { Errors = errors });
        }

        // Si se proporciona una nueva contraseña, intentar cambiarla
        if (!string.IsNullOrEmpty(model.NewPassword))
        {
            // Identity requiere que el usuario no tenga una contraseña antes de usar AddPasswordAsync
            // o que se use ChangePasswordAsync con la contraseña actual.
            // Para fines administrativos (un administrador cambiando la contraseña de otro usuario),
            // se usa RemovePasswordAsync y luego AddPasswordAsync.
            // ¡CUIDADO! Esto invalida la contraseña actual del usuario.

            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
            {
                var errors = removePasswordResult.Errors.Select(e => e.Description);
                _logger.LogWarning("Fallo al remover la contraseña del usuario {UserId}: {Errors}", user.Id, string.Join(", ", errors));
                return BadRequest(new { Message = "No se pudo actualizar la contraseña. Posiblemente la contraseña actual no es válida o la política lo impide.", Errors = errors });
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                var errors = addPasswordResult.Errors.Select(e => e.Description);
                _logger.LogWarning("Fallo al añadir nueva contraseña para el usuario {UserId}: {Errors}", user.Id, string.Join(", ", errors));
                return BadRequest(new { Message = "No se pudo establecer la nueva contraseña.", Errors = errors });
            }
        }

        _logger.LogInformation("Usuario con ID {UserId} actualizado exitosamente.", user.Id);
        return Ok(new { Message = "Usuario actualizado exitosamente.", UserId = user.Id });
    }

    [HttpDelete("{id}")] // Ruta: api/users/{id}
    [Authorize(Roles = Roles.Administrator)] 
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("Intento de eliminar usuario con ID {UserId} falló: Usuario no encontrado.", id);
            return NotFound(new { Message = $"Usuario con ID {id} no encontrado." });
        }

        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
            _logger.LogInformation("Usuario con ID {UserId} eliminado exitosamente.", id);
            return NoContent(); // 204 No Content para eliminación exitosa sin devolver contenido
        }

        var errors = result.Errors.Select(e => e.Description);
        _logger.LogWarning("Fallo al eliminar el usuario {UserId}: {Errors}", id, string.Join(", ", errors));
        return BadRequest(new { Errors = errors });
    }
}
