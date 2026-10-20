using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaGestionOdontologica.Api.Entities;
using SistemaGestionOdontologica.Api.Security;

namespace SistemaGestionOdontologica.Api.Data.Seed
{
    public class AdministradorInicializador
    {
        public static async Task InicializarAsync(
            IServiceProvider services, IConfiguration configuration, CancellationToken cancellationToken = default)
        {
            using var scope = services.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            var passwordHasher = scope.ServiceProvider
                .GetRequiredService<IPasswordHasher<Usuario>>();

            // No se crea otro administrador si ya existe uno.
            var existeAdministrador = await context.Usuarios
                .AnyAsync(u => u.Rol == Roles.Admin, cancellationToken);

            if (existeAdministrador)
            {
                return;
            }

            var username = configuration["SeedAdmin:Username"]?.Trim();

            var password = configuration["SeedAdmin:Password"];

            // Si no se configuró el proceso inicial, no se crea ningún usuario.
            if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException("SeedAdmin requiere Username y Password.");
            }

            if (username.Length > 50)
            {
                throw new InvalidOperationException(
                    "El nombre de usuario es demasiado largo.");
            }

            if (password.Length < 12)
            {
                throw new InvalidOperationException(
                    "La contraseña inicial debe tener al menos 12 caracteres.");
            }

            var existeUsername = await context.Usuarios
                .AnyAsync(u => u.Username == username, cancellationToken);

            if (existeUsername)
            {
                throw new InvalidOperationException(
                    "El nombre de usuario inicial ya existe.");
            }

            var administrador = new Usuario
            {
                Username = username,
                Rol = Roles.Admin,
                Activo = true,
                IdOdontologo = null,
                FechaCreacion = DateTime.UtcNow
            };

            administrador.PasswordHash = passwordHasher.HashPassword(administrador, password);

            context.Usuarios.Add(administrador);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
