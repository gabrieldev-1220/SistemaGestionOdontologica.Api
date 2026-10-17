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
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Registrar controladores
            builder.Services.AddControllers();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

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

            // Servicios.
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint(
                        "/openapi/v1.json",
                        "Sistema de Gestión Odontológico API v1");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
