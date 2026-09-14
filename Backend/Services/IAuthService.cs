using Backend.DTOs;

namespace Backend.Services;

// Contrato de la interfaz que expone los métodos de autenticación
public interface IAuthService
{
    // Registra un nuevo usuario en la base de datos
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);

    // Valida credenciales, comprueba BCrypt y emite el token
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}