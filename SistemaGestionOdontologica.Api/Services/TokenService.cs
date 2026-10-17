using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SistemaGestionOdontologica.Api.Entities;
using SistemaGestionOdontologica.Api.Interfaces;
using SistemaGestionOdontologica.Api.Options;

namespace SistemaGestionOdontologica.Api.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly byte[] _secretKey;

        public TokenService(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;

            if (string.IsNullOrWhiteSpace(_jwtOptions.Issuer))
            {
                throw new InvalidOperationException("No se configuró Jwt:Issuer.");
            }

            if (string.IsNullOrWhiteSpace(_jwtOptions.Audience))
            {
                throw new InvalidOperationException("No se configuró Jwt:Audience");
            }

            if (string.IsNullOrWhiteSpace(_jwtOptions.SecretKey))
            {
                throw new InvalidOperationException("No se configuró Jwt:SecretKey.");
            }

            // En esta implementación la clave se interpreta como texto UTF-8.
            _secretKey = Encoding.UTF8.GetBytes(_jwtOptions.SecretKey);

            if (_secretKey.Length < 32)
            {
                throw new InvalidOperationException("La clave JWT debe contener al menos 32 bytes.");
            }

            if (_jwtOptions.AccessTokenMinutes <= 0 || _jwtOptions.RefreshTokenDays <= 0)
            {
                throw new InvalidOperationException("La duración de los tokens debe ser mayor que cero.");
            }
        }

        public string GenerarAccessToken(Usuario usuario)
        {
            ArgumentNullException.ThrowIfNull(usuario);
            
            var ahora = DateTime.UtcNow;

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
                
                new(JwtRegisteredClaimNames.UniqueName, usuario.Username),

                new(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),

                new(ClaimTypes.Name, usuario.Username),

                new(ClaimTypes.Role, usuario.Rol),

                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            };

            if (usuario.IdOdontologo.HasValue)
            {
                claims.Add(new Claim("id_odontologo", usuario.IdOdontologo.Value.ToString()));
            }

            var signingKey = new SymmetricSecurityKey(_secretKey);

            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var expiracion = ahora.AddMinutes(_jwtOptions.AccessTokenMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: ahora,
                expires: expiracion,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        public string GenerarRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);

            return Convert.ToBase64String(bytes);
        }

        public string CalcularHashRefreshToken(string refreshToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);

            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));

            return Convert.ToHexString(hash);
        }
    }
}