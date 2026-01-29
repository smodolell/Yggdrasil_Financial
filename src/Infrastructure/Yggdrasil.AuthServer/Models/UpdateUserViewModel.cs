using System.ComponentModel.DataAnnotations;

namespace Yggdrasil.AuthServer.Models;

public class UpdateUserViewModel
{
    [Required]
    public string Id { get; set; } = string.Empty; // El ID del usuario a actualizar

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    // Opcional: Si quieres permitir cambiar la contraseña
    [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y un máximo de {1} caracteres de longitud.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva Contraseña")]
    public string? NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Nueva Contraseña")]
    [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la confirmación no coinciden.")]
    public string? ConfirmNewPassword { get; set; }

    // Puedes añadir aquí propiedades personalizadas de tu ApplicationUser que quieras actualizar
    // public string? FirstName { get; set; }
    // public string? LastName { get; set; }
}

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    // Puedes añadir más propiedades aquí si las tienes en tu ApplicationUser
    // y quieres exponerlas (ej. PhoneNumber, FirstName, LastName)
}