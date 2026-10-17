/// Esta interfaz define 3 operaciones:
/// 1. Generar un JWT firmado.
/// 2. Generar un refresh token criptográfico aleatorio.
/// 3. Calcular el hash del refresh token para almacenarlo en SQL Server sin guardar el token original.

using SistemaGestionOdontologica.Api.Entities;

namespace SistemaGestionOdontologica.Api.Interfaces
{
    public interface ITokenService
    {
        string GenerarAccessToken(Usuario usuario);
        string GenerarRefreshToken();
        string CalcularHashRefreshToken(string refreshToken);
    }
}
