using Microsoft.AspNetCore.Identity;
using SistemaGestionOdontologica.Api.Entities;
using SistemaGestionOdontologica.Api.Services;

namespace SistemaGestionOdontologica.Api.Security
{
    public class PasswordService : IPasswordService
    {
        private readonly PasswordHasher<Usuario> _passwordHasher;

        public PasswordService()
        {
            _passwordHasher = new PasswordHasher<Usuario>();
        }

        public string HashPassword(Usuario usuario, string password)
        {
            return _passwordHasher.HashPassword(usuario, password);
        }

        public bool VerifyPassword(Usuario usuario, string passwordHash, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(usuario, passwordHash, providedPassword);

            return result != PasswordVerificationResult.Failed;
        }
    }
}
