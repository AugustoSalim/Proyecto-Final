namespace Backend.Models
{
    public class Permiso
    {
        public int IdPermiso { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        public ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}