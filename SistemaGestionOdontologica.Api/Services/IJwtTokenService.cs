using SistemaGestionOdontologica.Api.Entities;
using SistemaGestionOdontologica.Api.Security;

namespace SistemaGestionOdontologica.Api.Services
{
    public interface IJwtTokenService
    {
        JwtTokenResult GenerateToken(Usuario usuario);
    }
}
