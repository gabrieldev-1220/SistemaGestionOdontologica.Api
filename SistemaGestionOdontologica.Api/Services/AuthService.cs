using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SistemaGestionOdontologica.Api.Data;
using SistemaGestionOdontologica.Api.DTOs.Auth;
using SistemaGestionOdontologica.Api.Entities;
using SistemaGestionOdontologica.Api.Interfaces;
using SistemaGestionOdontologica.Api.Options;

namespace SistemaGestionOdontologica.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        private readonly JwtOptions _jwtOptions;

        public AuthService(ApplicationDbContext context, ITokenService tokenService, IPasswordHasher<Usuario> passwordHasher,
            IOptions<JwtOptions> jwtOptions)
        {
            _context = context;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
        {
            var username = request.Username.Trim();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);

            if (usuario is null || !usuario.Activo)
            {
                return null;
            }

            var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, request.Password);

            if (resultado == PasswordVerificationResult.Failed)
            {
                return null;
            }

            if (resultado == PasswordVerificationResult.SuccessRehashNeeded)
            {
                usuario.PasswordHash = _passwordHasher.HashPassword(usuario, request.Password);
            }

            var ahora = DateTime.UtcNow;

            var refreshToken = _tokenService.GenerarRefreshToken();

            var entidadRefreshToken = new RefreshToken
            {
                IdUsuario = usuario.IdUsuario,

                TokenHash = _tokenService
                    .CalcularHashRefreshToken(refreshToken),

                FechaCreacion = ahora,

                FechaExpiracion = ahora.AddDays(_jwtOptions.RefreshTokenDays)
            };

            _context.RefreshTokens.Add(entidadRefreshToken);

            usuario.UltimoAcceso = ahora;

            await _context.SaveChangesAsync(cancellationToken);

            return CrearRespuesta(usuario, refreshToken, ahora);
        }

        public async Task<LoginResponseDto?> RenovarTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return null;
            }

            var tokenHash = _tokenService
                .CalcularHashRefreshToken(request.RefreshToken);

            var tokenGuardado = await _context.RefreshTokens
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

            if (tokenGuardado is null)
            {
                return null;
            }

            var ahora = DateTime.UtcNow;

            if (tokenGuardado.FechaRevocacion.HasValue || tokenGuardado.FechaExpiracion <= ahora || !tokenGuardado.Usuario.Activo)
            {
                return null;
            }

            // Invalidamos el refresh token anterior.
            tokenGuardado.FechaRevocacion = ahora;

            // Se genera uno nuevo.
            var nuevoRefreshToken = _tokenService.GenerarRefreshToken();

            var nuevoToken = new RefreshToken
            {
                IdUsuario = tokenGuardado.IdUsuario,

                TokenHash = _tokenService
                    .CalcularHashRefreshToken(nuevoRefreshToken),

                FechaCreacion =  ahora,

                FechaExpiracion = ahora.AddDays(_jwtOptions.RefreshTokenDays)
            };

            _context.RefreshTokens.Add(nuevoToken);

            await _context.SaveChangesAsync(cancellationToken);

            return CrearRespuesta(tokenGuardado.Usuario, nuevoRefreshToken, ahora);
        }

        public async Task<bool> CerrarSesionAsync(string refreshToken, int idUsuario, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return false;
            }

            var tokenHash = _tokenService
                .CalcularHashRefreshToken(refreshToken);

            var tokenGuardado = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash && t.IdUsuario == idUsuario, cancellationToken);

            if (tokenGuardado is null || tokenGuardado.FechaRevocacion.HasValue)
            {
                return false;
            }

            tokenGuardado.FechaRevocacion = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<UsuarioAutenticadoDto?> ObtenerUsuarioAsync(int idUsuario, CancellationToken cancellationToken = default)
        {
            return await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.IdUsuario == idUsuario && u.Activo)
                .Select(u => new UsuarioAutenticadoDto
                {
                    IdUsuario = u.IdUsuario,
                    Username = u.Username,
                    Rol = u.Rol,
                    IdOdontologo = u.IdOdontologo
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        private LoginResponseDto CrearRespuesta(Usuario usuario, string refreshToken, DateTime fechaEmisionUtc)
        {
            return new LoginResponseDto
            {
                AccessToken = _tokenService
                    .GenerarAccessToken(usuario),

                RefreshToken = refreshToken,

                TokenType = "Bearer",

                ExpiresAtUtc = fechaEmisionUtc.AddMinutes(_jwtOptions.AccessTokenMinutes),

                IdUsuario = usuario.IdUsuario,

                Username = usuario.Username,

                Rol = usuario.Rol
            };
        }
    }
}
