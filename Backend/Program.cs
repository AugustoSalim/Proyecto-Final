// Importamos el espacio de nombres de Entity Framework y los modelos/datos del proyecto
using Backend.Data;
using Microsoft.EntityFrameworkCore;

// Importamos las librerías para seguridad, tokens JWT y codificación de texto
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// Importamos los modelos clásicos y estables de Swagger / OpenAPI
using Microsoft.OpenApi.Models;
using Backend.Services;

// 1. Inicializamos el constructor del host web de ASP.NET Core
var builder = WebApplication.CreateBuilder(args);

// =============================================================================
// SERVICIOS: INYECCIÓN DE DEPENDENCIAS
// =============================================================================

// Registramos los controladores para exponer los endpoints REST
builder.Services.AddControllers();
// Registramos el servicio de autenticación para ser inyectado en los controladores
builder.Services.AddScoped<IAuthService, AuthService>();

// Configuramos la conexión a PostgreSQL con Npgsql y Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Leemos las claves y parámetros de configuración de JWT desde appsettings.json
var jwtKey = builder.Configuration["Jwt:Key"] 
    ?? throw new InvalidOperationException("La clave secreta de JWT no está configurada en appsettings.json");
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// Registramos el esquema de autenticación por defecto como JWT Bearer
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Definimos las reglas estrictas de validación para cada token entrante
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, // Verifica que la firma coincida con nuestra clave secreta
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = true,           // Verifica que el emisor sea el esperado
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,         // Verifica que la audiencia sea la esperada
        ValidAudience = jwtAudience,
        ValidateLifetime = true,         // Rechaza automáticamente tokens expirados
        ClockSkew = TimeSpan.Zero        // Elimina los 5 minutos de tolerancia por defecto
    };
});

// Registramos el sistema de autorización para proteger endpoints con [Authorize]
builder.Services.AddAuthorization();

// Configuramos Swagger UI con soporte para probar tokens Bearer en la interfaz gráfica
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Gestor de Ventas", Version = "v1" });

    // Definimos el esquema de seguridad Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autenticación JWT. Ingresá 'Bearer' seguido de un espacio y tu token. Ejemplo: 'Bearer eyJhbGciOi...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Aplicamos el requerimiento global para que aparezca el botón 'Authorize' con candado en Swagger
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// =============================================================================
// PIPELINE HTTP (MIDDLEWARES)
// =============================================================================
var app = builder.Build();

// Habilitamos la interfaz de Swagger en entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// IMPORTANTE: El orden de estos dos middlewares es estricto en ASP.NET Core
// 1° Authentication: Identifica al usuario leyendo el token
app.UseAuthentication();
// 2° Authorization: Verifica si el usuario tiene permisos para acceder al recurso
app.UseAuthorization();

// Mapea las rutas definidas en los Controllers
app.MapControllers();

// Inicia el servidor web
app.Run();