// Importamos las entidades de la carpeta Models (Usuario, Rol, Permiso, etc.)
using Backend.Models;
// Importamos las herramientas de Entity Framework Core para interactuar con la base de datos
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

// La clase hereda de DbContext: es el puente principal entre el código C# y PostgreSQL
public class ApplicationDbContext : DbContext
{
    // Constructor: recibe la configuración (cadena de conexión, proveedor Npgsql, etc.) y se la pasa a la clase base
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // Representa la tabla "permiso": permite consultar y guardar permisos mediante LINQ
    public DbSet<Permiso> Permisos => Set<Permiso>();

    // Representa la tabla "rol": gestiona los roles disponibles en el sistema
    public DbSet<Rol> Roles => Set<Rol>();

    // Representa la tabla intermedia "rol_permiso": administra la relación muchos a muchos
    public DbSet<RolPermiso> RolPermisos => Set<RolPermiso>();

    // Representa la tabla "usuario": maneja las cuentas, emails y contraseñas hasheadas
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    // Representa la tabla "refresh_token": administra los tokens de renovación de sesión
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    // Este método configura el mapeo exacto entre las clases de C# y las tablas reales de PostgreSQL
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Mantiene las configuraciones base predeterminadas de Entity Framework
        base.OnModelCreating(modelBuilder);

        // ==========================================
        // 1. MAPEADOR DE LA TABLA "permiso"
        // ==========================================
        modelBuilder.Entity<Permiso>(entity =>
        {
            entity.ToTable("permiso");
            entity.HasKey(p => p.IdPermiso);
            entity.Property(p => p.IdPermiso).HasColumnName("id_permiso");
            entity.Property(p => p.Nombre).HasColumnName("nombre");
            entity.Property(p => p.Descripcion).HasColumnName("descripcion");
        });

        // ==========================================
        // 2. MAPEADOR DE LA TABLA "rol"
        // ==========================================
        modelBuilder.Entity<Rol>(entity =>
        {
            // Vincula la clase Rol con la tabla física "rol"
            entity.ToTable("rol");

            // Define "IdRol" como la Clave Primaria
            entity.HasKey(e => e.IdRol);

            // Mapea la propiedad a la columna "id_rol"
            entity.Property(e => e.IdRol).HasColumnName("id_rol");

            // Mapea la columna "nombre", requerida y con longitud máxima de 50 caracteres
            entity.Property(e => e.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();

            // Mapea la columna booleana "es_personalizado"
            entity.Property(e => e.EsPersonalizado).HasColumnName("es_personalizado");

            // Crea restricción UNIQUE para evitar roles duplicados con el mismo nombre
            entity.HasIndex(e => e.Nombre).IsUnique();
        });

        // ==========================================
        // 3. MAPEADOR DE LA TABLA INTERMEDIA "rol_permiso" (Relación N:M)
        // ==========================================
        modelBuilder.Entity<RolPermiso>(entity =>
        {
            entity.ToTable("rol_permiso");

            // Clave primaria compuesta
            entity.HasKey(rp => new { rp.IdRol, rp.IdPermiso });

            entity.Property(rp => rp.IdRol).HasColumnName("id_rol");
            entity.Property(rp => rp.IdPermiso).HasColumnName("id_permiso");

            // Relación con Rol
            entity.HasOne(rp => rp.Rol)
                .WithMany(r => r.RolPermisos)
                .HasForeignKey(rp => rp.IdRol);

            // Relación con Permiso
            entity.HasOne(rp => rp.Permiso)
                .WithMany(p => p.RolPermisos)
                .HasForeignKey(rp => rp.IdPermiso);
        });

        // ==========================================
        // 4. MAPEADOR DE LA TABLA "usuario"
        // ==========================================
        modelBuilder.Entity<Usuario>(entity =>
        {
            // Vincula con la tabla física "usuario"
            entity.ToTable("usuario");

            // Define "IdUsuario" como la Clave Primaria
            entity.HasKey(e => e.IdUsuario);

            // Mapea la propiedad a la columna "id_usuario"
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

            // Mapea "nombre_usuario", obligatorio, máximo 100 caracteres
            entity.Property(e => e.NombreUsuario).HasColumnName("nombre_usuario").HasMaxLength(100).IsRequired();

            // Mapea "mail", obligatorio, máximo 100 caracteres
            entity.Property(e => e.mail).HasColumnName("mail").HasMaxLength(100).IsRequired();

            // Mapea "password_hash" para guardar la contraseña encriptada por BCrypt
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();

            // Mapea la columna de la Clave Foránea que apunta a la tabla rol
            entity.Property(e => e.IdRol).HasColumnName("id_rol");

            // Mapea el estado del usuario (activo / inactivo)
            entity.Property(e => e.Activo).HasColumnName("activo");

            // Mapea la fecha y hora de creación de la cuenta (en formato UTC)
            entity.Property(e => e.FechaCreacion).HasColumnName("fecha_creacion");

            // Restricción UNIQUE para que no existan dos usuarios con el mismo username
            entity.HasIndex(e => e.NombreUsuario).IsUnique();

            // Restricción UNIQUE para que no existan dos cuentas registradas con el mismo mail
            entity.HasIndex(e => e.mail).IsUnique();

            // Relación 1:N entre Rol y Usuario: un Rol tiene muchos Usuarios, un Usuario pertenece a un solo Rol
            entity.HasOne(e => e.Rol)
                  .WithMany(r => r.Usuarios)
                  .HasForeignKey(e => e.IdRol);
        });

        // ==========================================
        // 5. MAPEADOR DE LA TABLA "refresh_token"
        // ==========================================
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            // Vincula con la tabla física "refresh_token"
            entity.ToTable("refresh_token");

            // Define "IdRefreshToken" como Clave Primaria
            entity.HasKey(e => e.IdRefreshToken);

            // Mapea la propiedad a la columna "id_refresh_token"
            entity.Property(e => e.IdRefreshToken).HasColumnName("id_refresh_token");

            // Mapea el texto del token criptográfico, no nulo y con longitud máxima de 255
            entity.Property(e => e.Token).HasColumnName("token").HasMaxLength(255).IsRequired();

            // Mapea la fecha y hora límite de validez del token
            entity.Property(e => e.FechaExpiracion).HasColumnName("fecha_expiracion");

            // Mapea el indicador booleano de revocación (true si se cerró sesión o ya fue rotado)
            entity.Property(e => e.Revocado).HasColumnName("revocado");

            // Mapea la Clave Foránea del usuario dueño de este token
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

            // Restricción UNIQUE para asegurar que un token generado nunca colisione con otro
            entity.HasIndex(e => e.Token).IsUnique();

            // Relación 1:N: un Usuario puede tener múltiples tokens (uno por cada dispositivo donde inició sesión)
            // OnDelete(DeleteBehavior.Cascade): si se borra el usuario, se eliminan todos sus tokens asociados automáticamente
            entity.HasOne(e => e.Usuario)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(e => e.IdUsuario)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}