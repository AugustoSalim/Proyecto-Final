namespace Backend.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string mail { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public int IdRol { get; set; }
        public Rol Rol { get; set; } = null!;
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }

    // Para la relacion 1:N con los refresh tokens, un usuario puede tener muchos refresh tokens
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}