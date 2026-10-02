using SistemaGestionOdontologica.Api.Entities;

namespace SistemaGestionOdontologica.Api.Services
{
    public interface IPasswordService
    {
        string HashPassword(Usuario usuario, string password); // Se utiliza cuando se crea o cambia una contraseña.
        bool VerifyPassword(Usuario usuario, string passwordHash, string providedPassword); // Seutiliza durante el login.
    }
}
