namespace Backend.Models
{

public class RolPermiso
{
    public int IdRol { get; set; }
    public Rol Rol { get; set; } = null!;

    public int IdPermiso { get; set; }
    public Permiso Permiso { get; set; } = null!;
}
}