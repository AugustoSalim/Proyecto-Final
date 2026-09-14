namespace Backend.Models

{
    public class Rol
    {
        // Clave primaria del rol (mapea a id_rol)
    public int IdRol { get; set; }

    // Nombre del rol (Administrador, Tecnico, Vendedor)
    public string Nombre { get; set; } = string.Empty;

    // Indica si es un rol creado dinámicamente por el usuario o base del sistema
    public bool EsPersonalizado { get; set; }

    // Colección de permisos asociados mediante la tabla intermedia rol_permiso
    public ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();

    // Relación 1:N inversa -> Un rol tiene asociados muchos usuarios
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}