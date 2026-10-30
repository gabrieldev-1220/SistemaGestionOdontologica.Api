using SistemaGestionOdontologica.Api.Middleware;
using SistemaGestionOdontologica.Api.Security;
using SistemaGestionOdontologica.Api.Data.Seed;
using Microsoft.AspNetCore.Identity;
using SistemaGestionOdontologica.Api.Entities;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SistemaGestionOdontologica.Api.Data;
using SistemaGestionOdontologica.Api.Interfaces;
using SistemaGestionOdontologica.Api.Options;
using SistemaGestionOdontologica.Api.Services;

namespace SistemaGestionOdontologica.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Registrar controladores
            builder.Services.AddControllers();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });

            // Entity Framework Core + SQL Server.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                /// En "DefaultConnection" se coloca el nombre que se le pone a la cadena de conexión configurada en appsettings.Development.json
                /// Si se coloca como nombre CadenaConexion en lugar de DefaultConnection en appsettings.Development.json, entonces ese mismo nombre
                /// se coloca aquí en lugar de "DefaultConnection"
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // Opciones JWT
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

            // Validacion de configuración JWT.
            var jwtOptions = builder.Configuration
                .GetSection(JwtOptions.SectionName)
                .Get<JwtOptions>()
                ?? throw new InvalidOperationException("No se encontró la configuración JWT.");

            if (string.IsNullOrWhiteSpace(jwtOptions.SecretKey))
            {
                throw new InvalidOperationException("No se configuró Jwt:SecretKey en User Secrets.");
            }

            if (string.IsNullOrWhiteSpace(jwtOptions.Issuer) || string.IsNullOrWhiteSpace(jwtOptions.Audience))
            {
                throw new InvalidOperationException("Jwt:Issuer son obligatorios");
            }

            if (jwtOptions.AccessTokenMinutes <= 0 || jwtOptions.RefreshTokenDays <= 0)
            {
                throw new InvalidOperationException("Las duraciones de los tokens deben ser positivas.");
            }

            byte[] jwtKeyBytes = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);

            if (jwtKeyBytes.Length < 32)
            {
                throw new InvalidOperationException("La clave JWT debe tener al menos 32 bytes.");
            }

            // Autenticación JWT.
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(jwtKeyBytes),

                        ValidateLifetime = true,

                        RequireExpirationTime = true,
                        RequireSignedTokens = true,

                        ClockSkew = TimeSpan.Zero,

                        NameClaimType = ClaimTypes.Name,
                        RoleClaimType = ClaimTypes.Role
                    };

                    // Validaciones adicionales después de verificar el JWT.
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var principal = context.Principal;

                            if (principal?.Identity?.IsAuthenticated != true)
                            {
                                context.Fail("No se pudo verificar la identidad.");
                                return;
                            }

                            // Obtener el ID del usuario desde el JWT.
                            var idUsuarioTexto = principal.FindFirstValue(
                                ClaimTypes.NameIdentifier);

                            if (!int.TryParse(idUsuarioTexto, out int idUsuario) ||
                                idUsuario <= 0)
                            {
                                context.Fail("Identificador de usuario inválido.");
                                return;
                            }

                            // Obtener el rol declarado en el JWT.
                            var rolToken = principal.FindFirstValue(
                                ClaimTypes.Role);

                            if (string.IsNullOrWhiteSpace(rolToken))
                            {
                                context.Fail("El token no contiene un rol válido.");
                                return;
                            }

                            // Resolver un DbContext para esta solicitud.
                            var dbContext = context.HttpContext.RequestServices
                                .GetRequiredService<ApplicationDbContext>();

                            // Consultar el estado actual del usuario.
                            var usuario = await dbContext.Usuarios
                                .AsNoTracking()
                                .Where(u => u.IdUsuario == idUsuario)
                                .Select(u => new
                                {
                                    u.Activo,
                                    u.Rol,
                                    u.IdOdontologo
                                })
                                .FirstOrDefaultAsync(
                                    context.HttpContext.RequestAborted);

                            if (usuario is null || !usuario.Activo)
                            {
                                context.Fail(
                                    "El usuario no existe o está inactivo.");
                                return;
                            }

                            // Comprobar que el rol no haya cambiado.
                            if (usuario.Rol != rolToken)
                            {
                                context.Fail(
                                    "Los permisos del usuario han cambiado.");
                                return;
                            }

                            // Aceptar únicamente los roles conocidos.
                            if (usuario.Rol != Roles.Admin &&
                                usuario.Rol != Roles.Odontologo &&
                                usuario.Rol != Roles.Asistente)
                            {
                                context.Fail("Rol de usuario no permitido.");
                                return;
                            }

                            if (usuario.Rol == Roles.Odontologo)
                            {
                                if (!usuario.IdOdontologo.HasValue)
                                {
                                    context.Fail(
                                        "El usuario no tiene un odontólogo asociado.");
                                    return;
                                }

                                bool odontologoActivo = await dbContext.Odontologos
                                    .AsNoTracking()
                                    .AnyAsync(
                                        o => o.IdOdontologo ==
                                             usuario.IdOdontologo.Value &&
                                             o.Activo,
                                        context.HttpContext.RequestAborted);

                                if (!odontologoActivo)
                                {
                                    context.Fail(
                                        "El odontólogo asociado no existe o está inactivo.");
                                    return;
                                }
                            }
                            else if (usuario.IdOdontologo.HasValue)
                            {
                                context.Fail(
                                    "La asociación del usuario no corresponde a su rol.");
                                return;
                            }
                        }
                    };
                });

            // Autorización
            builder.Services.AddAuthorization(options =>
            {
                // Todos los endpoints requieren autenticación salvo aquellos marcados explícitamente con [AllowAnonymous].
                options.FallbackPolicy = new Microsoft.AspNetCore.Authorization
                    .AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            // SECCIÓN SERVICIOS.
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<IPacienteService, PacienteService>();

            builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();

            builder.Services.AddScoped<IOdontologoService, OdontologoService>();

            builder.Services.AddScoped<ITurnoService, TurnoService>();

            builder.Services.AddScoped<IUsuarioService, UsuarioService>();

            builder.Services.AddScoped<IHistorialClinicoService, HistorialClinicoService>();

            var app = builder.Build();

            // Registración de Middleware
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // Inicialización controlada del administrador
                await AdministradorInicializador.InicializarAsync(app.Services, app.Configuration);                

                /// Colocar .AllowAnonymous() para permitir el acceso anónimo al documento OpenAPI que Swagger necesita poder cargar
                /// porque se agregó la politica global options.FallbackPolicy por lo que los endpoints requieren autenticación.
                app.MapOpenApi().AllowAnonymous();

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint(
                        "/openapi/v1.json",
                        "Sistema de Gestión Odontológico API v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
