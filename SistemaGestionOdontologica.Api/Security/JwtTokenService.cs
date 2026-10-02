using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SistemaGestionOdontologica.Api.Entities;
using SistemaGestionOdontologica.Api.Services;
using SistemaGestionOdontologica.Api.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SistemaGestionOdontologica.Api.Security
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtTokenService(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
        }

        public JwtTokenResult GenerateToken(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new(
                    JwtRegisteredClaimNames.Sub,
                    usuario.IdUsuario.ToString()),

                new(
                    JwtRegisteredClaimNames.UniqueName,
                    usuario.Username),

                new(
                    ClaimTypes.NameIdentifier,
                    usuario.IdUsuario.ToString()),

                new(
                    ClaimTypes.Name,
                    usuario.Username),

                new(
                    ClaimTypes.Role,
                    usuario.Rol.ToString().ToLowerInvariant()),

                new(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)); // Convienrte la clave secreta en una criptofráfica.

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return new JwtTokenResult
            {
                Token = tokenString,
                ExpiresAt = expiration
            };
        }
    }
}
