using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

// Objeto para recibir las credenciales al iniciar sesión
public class LoginDto
{
    // Permite loguearse con el nombre de usuario
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    public string NombreUsuario { get; set; } = string.Empty;

    // Contraseña en texto plano enviada para verificación contra BCrypt
    [Required(ErrorMessage = "La contraseña es requerida")]
    public string Password { get; set; } = string.Empty;
}