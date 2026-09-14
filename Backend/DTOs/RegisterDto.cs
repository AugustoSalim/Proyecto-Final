// Importamos atributos de validación de datos
using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;

// Objeto de transferencia para recibir los datos de registro de un nuevo usuario
public class RegisterDto
{
    // Campo obligatorio con límite de caracteres
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [MaxLength(100)]
    public string NombreUsuario { get; set; } = string.Empty;

    // Validación de formato de correo electrónico
    [Required(ErrorMessage = "El correo electrónico es requerido")]
    [EmailAddress(ErrorMessage = "El formato de correo no es válido")]
    [MaxLength(100)]
    public string Mail { get; set; } = string.Empty;

    // Contraseña en texto plano que envía el cliente (mínimo 6 caracteres)
    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Password { get; set; } = string.Empty;

    // Identificador del rol asignado (por ejemplo, 1 para Administrador)
    [Required(ErrorMessage = "El rol es requerido")]
    public int IdRol { get; set; }
}