using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PruebaController : ControllerBase
    {
        // ==========================================
        // 1. ENDPOINT PÚBLICO
        // Cualquiera puede entrar sin enviar ningún token
        // ==========================================
        [HttpGet("publico")]
        public IActionResult Publico()
        {
            return Ok(new { mensaje = "Este endpoint es público, no requiere autenticación." });
        }

        // ==========================================
        // 2. ENDPOINT PROTEGIDO BÁSICO
        // Requiere sí o sí un token JWT válido y no expirado.
        // Si no mandás token o mandás uno trucho -> Error 401 Unauthorized automático.
        // ==========================================
        [HttpGet("protegido")]
        [Authorize]
        public IActionResult Protegido()
        {
            // Leemos los claims que desempaquetó el middleware en memoria
            var usuario = User.FindFirst(ClaimTypes.Name)?.Value;
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                mensaje = "¡Acceso concedido al endpoint protegido!",
                usuarioAutenticado = usuario,
                rolAsignado = rol
            });
        }

        // ==========================================
        // 3. ENDPOINT SOLO PARA ADMINISTRADORES
        // Además de tener token, el claim de Rol debe ser 'Administrador'.
        // Si entra un 'Vendedor' -> Error 403 Forbidden.
        // ==========================================
        [HttpGet("solo-admin")]
        [Authorize(Roles = "Administrador")]
        public IActionResult SoloAdmin()
        {
            return Ok(new { mensaje = "Bienvenido, Administrador. Tenés acceso a esta zona crítica." });
        }
    }
}