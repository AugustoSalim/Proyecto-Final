using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Services;

public class AuthService : IAuthService
{
    // Inyección del DbContext para interactuar con PostgreSQL
    private readonly ApplicationDbContext _context;
    // Inyección de la configuración para leer las claves de appsettings.json
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        // 1. Verificamos que el nombre de usuario no esté en uso
        var existeUsuario = await _context.Usuarios.AnyAsync(u => u.NombreUsuario == dto.NombreUsuario);
        if (existeUsuario)
            throw new Exception("El nombre de usuario ya está registrado.");

        // 2. Verificamos que el correo no esté duplicado
        var existeMail = await _context.Usuarios.AnyAsync(u => u.mail == dto.Mail);
        if (existeMail)
            throw new Exception("El correo electrónico ya está registrado.");

        // 3. Verificamos que el rol exista
        var rol = await _context.Roles.FindAsync(dto.IdRol);
        if (rol == null)
            throw new Exception("El rol especificado no existe.");

        // 4. Hasheamos la contraseña con BCrypt (salt automático)
        string hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        // 5. Creamos la entidad Usuario
        var usuario = new Usuario
        {
            NombreUsuario = dto.NombreUsuario,
            mail = dto.Mail,
            PasswordHash = hash,
            IdRol = dto.IdRol,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        // 6. Generamos el token y retornamos la respuesta
        return await GenerarRespuestaAuth(usuario);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        // 1. Buscamos el usuario por su nombre de usuario e incluimos su Rol
        var usuario = await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.NombreUsuario == dto.NombreUsuario);

        // 2. Si no existe o está inactivo, rechazamos el login
        if (usuario == null || !usuario.Activo)
            throw new Exception("Credenciales incorrectas o usuario inactivo.");

        // 3. Verificamos el hash con BCrypt
        bool passwordValida = BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash);
        if (!passwordValida)
            throw new Exception("Credenciales incorrectas o usuario inactivo.");

        // 4. Generamos token con sus permisos
        return await GenerarRespuestaAuth(usuario);
    }

    private async Task<AuthResponseDto> GenerarRespuestaAuth(Usuario usuario)
    {
        // Traemos los permisos del rol mediante la tabla intermedia rol_permiso
        var permisos = await _context.RolPermisos
            .Where(rp => rp.IdRol == usuario.IdRol)
            .Select(rp => rp.Permiso.Nombre)
            .ToListAsync();

        // Creamos la lista de Claims (declaraciones de identidad) dentro del JWT
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new Claim(ClaimTypes.Name, usuario.NombreUsuario),
            new Claim(ClaimTypes.Email, usuario.mail),
            new Claim(ClaimTypes.Role, usuario.Rol?.Nombre ?? "SinRol")
        };

        // Agregamos cada permiso como un claim individual
        foreach (var permiso in permisos)
        {
            claims.Add(new Claim("permiso", permiso));
        }

        // Leemos la clave secreta configurada en appsettings.json
        var jwtKey = _configuration["Jwt:Key"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Definimos tiempo de expiración (15 minutos por defecto)
        var expirationMinutes = double.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15");

        // Creamos el descriptor del token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = creds
        };

        // Firmamos y serializamos el token a string
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new AuthResponseDto
        {
            Token = tokenHandler.WriteToken(token),
            NombreUsuario = usuario.NombreUsuario,
            Rol = usuario.Rol?.Nombre ?? "SinRol",
            Permisos = permisos
        };
    }
}