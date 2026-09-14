namespace Backend.DTOs;

// Objeto de respuesta que devuelve el servidor al loguearse o registrarse
public class AuthResponseDto
{
    // Token JWT firmado que el cliente enviará en la cabecera Authorization: Bearer
    public string Token { get; set; } = string.Empty;

    // Nombre del usuario autenticado para mostrar en la interfaz
    public string NombreUsuario { get; set; } = string.Empty;

    // Nombre del rol del usuario (para autorizaciones visuales en frontend)
    public string Rol { get; set; } = string.Empty;

    // Lista de nombres de permisos que posee (ej: "crear_venta", "anular_factura")
    public List<string> Permisos { get; set; } = new List<string>();
}