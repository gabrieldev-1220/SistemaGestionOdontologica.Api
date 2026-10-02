using Microsoft.EntityFrameworkCore;
using SistemaGestionOdontologica.Api.Data;
using SistemaGestionOdontologica.Api.DTOs.Auth;

namespace SistemaGestionOdontologica.Api.Services
{
    public class AuthService : IAuthService
    {
        // Inyección de dependencias.
        private readonly ApplicationDbContext _context;
        private readonly IPasswordService _passwordService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService (ApplicationDbContext context, IPasswordService passwordService, IJwtTokenService jwtTokenService)
        {
            _context = context;
            _passwordService = passwordService;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            // Busca al usuario utilizando EF Core.
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (usuario is null)
            {
                return null;
            }

            if (!usuario.Activo) // Impide que un usuario desactivado pueda autenticarse aún conociendo su contraseña.
            {
                return null;
            }

            // Verificación de la contraseña.
            var passwordValida = _passwordService.VerifyPassword(usuario, usuario.PasswordHash, request.Password);

            if (!passwordValida)
            {
                return null;
            }

            usuario.UltimoAcceso = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Genera el JWT.
            var tokenResult = _jwtTokenService.GenerateToken(usuario);

            return new LoginResponseDto
            {
                Token = tokenResult.Token,
                TokenType = "Bearer",
                ExpiresAt = tokenResult.ExpiresAt,
                IdUsuario = usuario.IdUsuario,
                UserName = usuario.Username,
                Rol = usuario.Rol.ToString().ToLowerInvariant(),
            };

        }
    }
}
