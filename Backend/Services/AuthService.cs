using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services
{
    public class AuthService : IAuthService
    {
        // Inyectamos el DbContext para hablar con PostgreSQL y Configuration para leer appsettings.json
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ==========================================
        // 1. REGISTRO DE USUARIOS
        // ==========================================
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            // Validaciones previas: evitamos duplicados antes de tocar la base de datos
            if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == dto.NombreUsuario))
                throw new Exception("El nombre de usuario ya está registrado.");

            if (await _context.Usuarios.AnyAsync(u => u.mail == dto.Mail))
                throw new Exception("El correo electrónico ya está registrado.");

            var rolExiste = await _context.Roles.AnyAsync(r => r.IdRol == dto.IdRol);
            if (!rolExiste)
                throw new Exception("El rol especificado no existe.");

            // Hasheo de seguridad: nunca guardamos la contraseña plana, usamos BCrypt con salt automático
            var nuevoUsuario = new Usuario
            {
                NombreUsuario = dto.NombreUsuario,
                mail = dto.Mail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                IdRol = dto.IdRol,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            // EF Core genera el comando INSERT en PostgreSQL
            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            // Emitimos el JWT y el primer Refresh Token para dejar la sesión iniciada
            return await GenerarRespuestaAuth(nuevoUsuario);
        }

        // ==========================================
        // 2. INICIO DE SESIÓN (LOGIN)
        // ==========================================
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            // Buscamos el usuario e incluimos con LINQ toda la cadena relacional:
            // Usuario -> Rol -> RolPermiso -> Permiso (así traemos sus permisos de una sola consulta)
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                    .ThenInclude(r => r.RolPermisos)
                        .ThenInclude(rp => rp.Permiso)
                .FirstOrDefaultAsync(u => u.NombreUsuario == dto.NombreUsuario);

            // Verificamos si existe y si la contraseña coincide con el hash almacenado
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
                throw new Exception("Credenciales incorrectas.");

            if (!usuario.Activo)
                throw new Exception("El usuario se encuentra inactivo.");

            // Si las credenciales son válidas, generamos tokens
            return await GenerarRespuestaAuth(usuario);
        }

        // ==========================================
        // 3. RENOVACIÓN DE TOKENS (REFRESH)
        // ==========================================
        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshRequestDto dto)
        {
            // Buscamos en PostgreSQL el refresh token que nos mandó el cliente
            var tokenEntity = await _context.RefreshTokens
                .Include(rt => rt.Usuario)
                    .ThenInclude(u => u.Rol)
                        .ThenInclude(r => r.RolPermisos)
                            .ThenInclude(rp => rp.Permiso)
                .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken);

            // Comprobamos validez, revocación y vigencia
            if (tokenEntity == null)
                throw new Exception("Refresh token inexistente.");

            if (tokenEntity.Revocado)
                throw new Exception("El refresh token ha sido revocado.");

            if (tokenEntity.FechaExpiracion < DateTime.UtcNow)
                throw new Exception("El refresh token ha expirado. Por favor, inicie sesión nuevamente.");

            // ROTACIÓN DE TOKENS: quemamos el token usado para que nadie pueda reutilizarlo
            tokenEntity.Revocado = true;
            await _context.SaveChangesAsync();

            // Generamos un par completamente nuevo (nuevo JWT + nuevo Refresh Token)
            return await GenerarRespuestaAuth(tokenEntity.Usuario);
        }

        // ==========================================
        // 4. CIERRE DE SESIÓN (LOGOUT)
        // ==========================================
        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            // Al hacer logout, buscamos el token y lo marcamos como revocado en PostgreSQL
            var tokenEntity = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (tokenEntity == null || tokenEntity.Revocado)
                return false;

            tokenEntity.Revocado = true;
            await _context.SaveChangesAsync();
            return true;
        }

        // ==========================================
        // 5. MOTOR PRIVADO DE EMISIÓN DE TOKENS
        // ==========================================
        private async Task<AuthResponseDto> GenerarRespuestaAuth(Usuario usuario)
        {
            // Si el usuario no trajo cargado su rol por navegación previa, lo cargamos explícitamente
            if (usuario.Rol == null)
            {
                usuario = await _context.Usuarios
                    .Include(u => u.Rol)
                        .ThenInclude(r => r.RolPermisos)
                            .ThenInclude(rp => rp.Permiso)
                    .FirstAsync(u => u.IdUsuario == usuario.IdUsuario);
            }

            // Extraemos los nombres de los permisos en una lista plana de strings
            var permisos = usuario.Rol.RolPermisos
                .Select(rp => rp.Permiso.Nombre)
                .ToList();

            // Leemos los secretos y configuración del appsettings.json
            var jwtKey = _configuration["Jwt:Key"] 
                ?? throw new InvalidOperationException("Clave JWT no configurada.");
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            // Empaquetamos los claims (la identidad y permisos que viajarán dentro del JWT)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Email, usuario.mail),
                new Claim(ClaimTypes.Role, usuario.Rol.Nombre)
            };

            foreach (var permiso in permisos)
            {
                claims.Add(new Claim("permiso", permiso));
            }

            // Firmamos criptográficamente el JWT con HMAC-SHA256 y definimos 15 min de expiración
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtString = tokenHandler.WriteToken(token);

            // Generamos una cadena aleatoria criptográfica para el Refresh Token y la guardamos en PostgreSQL (7 días)
            var nuevoRefreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                FechaExpiracion = DateTime.UtcNow.AddDays(7),
                Revocado = false,
                IdUsuario = usuario.IdUsuario
            };

            _context.RefreshTokens.Add(nuevoRefreshToken);
            await _context.SaveChangesAsync();

            // Devolvemos el DTO final al controlador
            return new AuthResponseDto
            {
                Token = jwtString,
                RefreshToken = nuevoRefreshToken.Token,
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol.Nombre,
                Permisos = permisos
            };
        }
    }
}